using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Highlight;
using Lucene.Net.Store;
using LQ = Lucene.Net.QueryParsers;

namespace Graffiti.Core
{
	/// <summary>
	/// Provides an in memory search index for all the posts in a Graffiti Site.
	/// </summary>
	public class SearchIndex
	{
		#region static

		private static readonly Regex _andRegex = new Regex(@"\sAND\s", RegexOptions.IgnoreCase);
		private static readonly Regex _orRegex = new Regex(@"\sOR\s", RegexOptions.IgnoreCase);
		private static readonly Regex _notRegex = new Regex(@"\sNOT\s", RegexOptions.IgnoreCase);
		private static readonly Regex _escape = new Regex("([" + "\\+,\\-,\\&,\\|,\\!,\\(,\\),\\{,\\},\\[,\\],\\^,\\\",\\~,\\*,\\?,\\:,\\\\" + "])", RegexOptions.Compiled);


		#endregion

		#region private
		ReaderWriterLock lck = new ReaderWriterLock();
		RAMDirectory rd = null;
		#endregion

		private static List<Post> ItemsToIndex()
		{
			DataBuddy.Query q = PostCollection.DefaultQuery();
			PostCollection pc = new PostCollection();
			pc.LoadAndCloseReader(q.ExecuteReader());

			return pc;
		}

		private static Document CreateDocument(Post t)
		{
			Document doc = new Document();
			doc.Add(new Field("postid", t.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("title", t.Title, Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field("body", RemoveHtml(t.Body, 0), Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field("exact", RemoveHtml(t.Body, 0), Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.YES));
			doc.Add(new Field("rawbody", t.Body, Field.Store.YES, Field.Index.NO));
			doc.Add(new Field("category", t.Category.Name, Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("categoryid", t.Category.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("username", t.UserName, Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("modifiedby", t.ModifiedBy, Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("createdby", t.CreatedBy, Field.Store.YES, Field.Index.NOT_ANALYZED));
			foreach (string tag in Util.ConvertStringToList(t.TagList))
			{
				doc.Add(new Field("tag", tag, Field.Store.YES, Field.Index.ANALYZED));
			}
			doc.Add(new Field("author", t.CreatedBy, Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("date", DateTools.DateToString(t.Published, DateTools.Resolution.HOUR), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("createddate", DateTools.DateToString(t.CreatedOn, DateTools.Resolution.HOUR), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("modifieddate", DateTools.DateToString(t.ModifiedOn, DateTools.Resolution.HOUR), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("name", t.Name, Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("image", t.ImageUrl ?? string.Empty, Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("commentcount", t.CommentCount.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("pendingcommentcount", t.PendingCommentCount.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("propertykeys", t.PropertyKeys, Field.Store.YES, Field.Index.NO));
			doc.Add(new Field("propertyvalues", t.PropertyValues, Field.Store.YES, Field.Index.NO));
			return doc;
		}

		private static Post CreateFromDocument(Document doc, Analyzer analyzer, Highlighter hl)
		{
			Post p = new Post();
			p.Id = Int32.Parse(doc.GetField("postid").StringValue());
			p.UserName = doc.GetField("username").StringValue();
			p.CreatedBy = doc.GetField("createdby").StringValue();
			p.ModifiedBy = doc.GetField("modifiedby").StringValue();
			p.Title = doc.GetField("title").StringValue();
			p.CategoryId = Int32.Parse(doc.GetField("categoryid").StringValue());
			p.CreatedBy = doc.GetField("author").StringValue();
			p.Published = DateTools.StringToDate(doc.GetField("date").StringValue());
			p.CreatedOn = DateTools.StringToDate(doc.GetField("createddate").StringValue());
			p.ModifiedOn = DateTools.StringToDate(doc.GetField("modifieddate").StringValue());
			p.Name = doc.GetField("name").StringValue();
			p.PostBody = (analyzer == null || hl == null) ? doc.GetField("rawbody").StringValue() : BestMatch(doc.GetField("body").StringValue(), "body", analyzer, hl);
			p.ExtendedBody = "<!--hidden-->";
			p.CommentCount = Int32.Parse(doc.GetField("commentcount").StringValue());
			p.PendingCommentCount = Int32.Parse(doc.GetField("pendingcommentcount").StringValue());
			p.ImageUrl = doc.GetField("image").StringValue();
			p.PropertyKeys = doc.GetField("propertykeys").StringValue();
			p.PropertyValues = doc.GetField("propertyvalues").StringValue();


			string[] sa = doc.GetValues("tag");
			if (sa != null)
				p.TagList = string.Join(",", sa);

			p.DeserializeCustomFields();

			return p;

		}

		private static QueryParser GetQueryParser(Analyzer a)
		{
			return new MultiFieldQueryParser(new string[] { "body", "title" }, a);
		}


		#region Virtual

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sq">The query to base our sort on</param>
		/// <returns></returns>
		protected virtual Sort GetSort(SearchQuery sq)
		{
			return new Sort();
		}

		protected virtual Query GetFilterQuery(SearchQuery sq)
		{
			return null;
		}

		protected virtual Analyzer GetAnalyzer()
		{
			return new StandardAnalyzer();
		}

		protected virtual int ReaderTimeOut { get { return 15000; } }
		protected virtual int WriterTimeOut { get { return 30000; } }

		#endregion

		#region Internal Implementation

		protected virtual SearchResultSet<Post> Search(SearchQuery sq, bool reSearch)
		{
			DateTime dt = DateTime.Now;
			string _sq = sq.QueryText;
			IndexSearcher searcher = null;
			if (rd == null)
			{
				BuildIndex();
			}

			lck.AcquireReaderLock(ReaderTimeOut);
			try
			{
				Analyzer analyzer = GetAnalyzer();

				_sq = _andRegex.Replace(_sq, " AND ");
				_sq = _orRegex.Replace(_sq, " OR ");

				QueryParser parser = GetQueryParser(analyzer);
				parser.SetDefaultOperator(QueryParser.AND_OPERATOR);
				Query q = parser.Parse(_sq);
				searcher = new IndexSearcher(rd);

				Query filterQuery = GetFilterQuery(sq);
				LFilter theFilter = null;
				if (filterQuery != null)
					theFilter = new LFilter(filterQuery);

				Hits hits = searcher.Search(q, theFilter, GetSort(sq));

				SearchResultSet<Post> searchResults = new SearchResultSet<Post>();

				searchResults.TotalRecords = hits.Length();
				TimeSpan ts = (DateTime.Now - dt);
				searchResults.SearchDuration = ts.Milliseconds;

				SimpleHTMLFormatter html = new SimpleHTMLFormatter(":openhighlight", ":closehighlight"); //use placeholders instead of html to allow later htmlencode
				Highlighter highlighter = new Highlighter(html, new QueryScorer(q));

				int start = sq.PageIndex * sq.PageSize;
				int end = (sq.PageIndex + 1) * sq.PageSize;

				if (start > hits.Length())
					start = hits.Length();

				if (end > hits.Length())
					end = hits.Length();

				for (int i = start; i < end; i++)
				{
					Document doc = hits.Doc(i);
					searchResults.Add(CreateFromDocument(doc, analyzer, highlighter));
				}

				return searchResults;
			}
			catch (Lucene.Net.QueryParsers.ParseException)
			{
				if (reSearch)
				{
					sq.QueryText = _escape.Replace(sq.QueryText, "\\$1");
					Search(sq, false);
				}
				else
					throw;
			}
			finally
			{
				if (searcher != null)
					searcher.Close();


				lck.ReleaseReaderLock();
			}

			return null;
		}


		protected static string BestMatch(string text, string fieldName, Analyzer analyzer, Highlighter hl)
		{
			try
			{
                //decode text to prevent fragmenting escape chars or char refs
                text = HttpUtility.HtmlDecode(text);
                TokenStream tokenStream = analyzer.TokenStream(fieldName, new StringReader(text));
                //encode for html validity, replace temporary highlight tags
                return HttpUtility.HtmlEncode(hl.GetBestFragments(tokenStream, text, 2, "...")).Replace(":openhighlight", "<span style=\"BACKGROUND-COLOR: Yellow\">").Replace(":closehighlight", "</span>");
			}
			catch
			{
                return RemoveHtml(text.Replace(":openhighlight", "").Replace(":closehighlight", ""), 250);
			}

		}


		void BuildIndex()
		{
			IndexWriter writer = null;
			lck.AcquireWriterLock(WriterTimeOut);
			try
			{
				if (rd == null)
				{
					rd = new RAMDirectory();
					writer = new IndexWriter(rd, GetAnalyzer(), true, IndexWriter.MaxFieldLength.LIMITED);

					foreach (Post post in ItemsToIndex())
					{
						writer.AddDocument(CreateDocument(post));
					}

				}
			}
			finally
			{
				if (writer != null)
					writer.Close();

				lck.ReleaseWriterLock();
			}

		}

		#endregion

		#region Public Methods

		public int TotalDocuments()
		{
			IndexWriter writer = null;
			lck.AcquireReaderLock(ReaderTimeOut);
			try
			{
				if (rd == null)
					return -1;

				writer = new IndexWriter(rd, GetAnalyzer(), false);
				int count = writer.DocCount();
				return count;
			}
			finally
			{
				if (writer != null)
					writer.Close();

				lck.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// For objects matching your query
		/// </summary>
		/// <param name="sq"></param>
		/// <returns></returns>
		public SearchResultSet<Post> Search(SearchQuery sq)
		{
			return Search(sq, true);
		}

		public List<Post> Similar(int postid, int itemsToReturn)
		{
			List<Post> TList = new List<Post>();

			int docId = -1;

			IndexSearcher searcher = null;
			IndexReader reader = null;

			if (rd == null)
			{
				BuildIndex();
			}

			lck.AcquireReaderLock(ReaderTimeOut);
			try
			{
				Analyzer analyzer = GetAnalyzer();
				QueryParser parser = GetQueryParser(analyzer);
				parser.SetDefaultOperator(QueryParser.AND_OPERATOR);

				Query q = parser.Parse("postid:" + postid);

				searcher = new IndexSearcher(rd);

				Hits hits = searcher.Search(q);
				if (hits != null && hits.Length() > 0)
					docId = hits.Id(0);

				if (docId > -1)
				{
					reader = IndexReader.Open(rd);

					TermFreqVector tfv = reader.GetTermFreqVector(docId, "exact");
					BooleanQuery booleanQuery = new BooleanQuery();
					for (int j = 0; j < tfv.Size(); j++)
					{
						TermQuery tq = new TermQuery(new Term("exact", tfv.GetTerms()[j]));
						booleanQuery.Add(tq, BooleanClause.Occur.SHOULD);
					}

					Hits similarhits = searcher.Search(booleanQuery, Sort.RELEVANCE);

					for (int i = 0; i < similarhits.Length(); i++)
					{
						Document doc = similarhits.Doc(i);
						if (similarhits.Id(i) != docId)
						{
							TList.Add(CreateFromDocument(doc, analyzer, null));
						}

						if (TList.Count >= itemsToReturn)
							break;

					}
				}

			}
			catch (Exception)
			{
			}
			finally
			{
				if (searcher != null)
					searcher.Close();

				if (reader != null)
					reader.Close();

				lck.ReleaseReaderLock();
			}



			return TList;
		}

		/// <summary>
		/// Clears all results from the current index. During the next search the index will be rebuilt.
		/// </summary>
		public void Clear()
		{
			lck.AcquireWriterLock(WriterTimeOut);
			try
			{
				if (rd != null)
				{
					rd.Close();
					rd = null;
				}
			}
			finally
			{
				lck.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// Clears the current index and then resuilds it.
		/// </summary>
		public void Reset()
		{
			lck.AcquireWriterLock(WriterTimeOut);
			try
			{
				if (rd != null)
				{
					rd.Close();
					rd = null;
				}

				BuildIndex();
			}
			finally
			{
				lck.ReleaseWriterLock();
			}
		}

		#endregion

		#region Helpers

		protected static string RemoveHtml(string html, int charLimit)
		{
			if (html == null || html.Trim().Length == 0)
				return string.Empty;

			html = htmlRegex.Replace(html, string.Empty);
			html = spacer.Replace(html.Trim(), " ");

			if (charLimit <= 0 || charLimit >= html.Length)
				return html;
			else
				return MaxLength(html, charLimit);
		}

		protected static string MaxLength(string text, int charLimit)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;

			if (charLimit >= text.Length)
				return text;

			Match match = isWhitespace.Match(text, charLimit);
			if (!match.Success)
				return text;
			else
				return text.Substring(0, match.Index);
		}

		protected static Regex htmlRegex = new Regex("<[^>]+>|\\&nbsp\\;|\\&lt\\;|\\&rt\\;", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		protected static Regex spacer = new Regex(@"\s{2,}");
		private static Regex isWhitespace = new Regex("[^\\w&;#]", RegexOptions.Singleline | RegexOptions.Compiled);


		#endregion


	}

	public class SearchQuery
	{
		private int _pageSize;
		private int _pageIndex;
		private string _queryText;
		private SearchSort _sort = SearchSort.Relevance;

		/// <summary>
		/// The number of records to return per page.
		/// </summary>
		public int PageSize
		{
			get { return _pageSize; }
			set { _pageSize = value; }
		}


		/// <summary>
		/// Current Page
		/// </summary>
		public int PageIndex
		{
			get { return _pageIndex; }
			set { _pageIndex = value; }
		}


		/// <summary>
		/// What are we searching for
		/// </summary>
		public string QueryText
		{
			get { return _queryText; }
			set { _queryText = value; }
		}


		/// <summary>
		/// What/how are we sorting
		/// </summary>
		public SearchSort Sort
		{
			get { return _sort; }
			set { _sort = value; }
		}

	}

	public enum SearchSort
	{
		Ascending,
		Descending,
		Index,
		Relevance

	}

	internal class LFilter : Filter
	{
		internal LFilter(Query q)
		{
			_query = q;
		}

		private Query _query = null;

		public override BitArray Bits(IndexReader reader)
		{
			BitArray bits = new BitArray((reader.MaxDoc() % 64 == 0 ? reader.MaxDoc() / 64 : reader.MaxDoc() / 64 + 1) * 64);
			new IndexSearcher(reader).Search(_query, new LHitCollector(bits));
			return bits;

		}
	}

	internal class LHitCollector : HitCollector
	{
		internal LHitCollector(BitArray bits)
		{
			_bits = bits;
		}

		private BitArray _bits = null;

		public override void Collect(int doc, float score)
		{
			_bits.Set(doc, true);
		}
	}

	public class SearchResultSet<T> : List<T> where T : class, new()
	{
		private int _pageSize;
		private int _pageIndex;
		private int _totalRecords;
		private int _SearchDuration;

		/// <summary>
		/// Current page, this value is returned via the SearchQuery
		/// </summary>
		public int PageSize
		{
			get { return _pageSize; }
			set { _pageSize = value; }
		}


		/// <summary>
		/// Current page index, this value is returned via the SearchQuery
		/// </summary>
		public int PageIndex
		{
			get { return _pageIndex; }
			set { _pageIndex = value; }
		}


		/// <summary>
		/// Total number of records matching the SearchQuery
		/// </summary>
		public int TotalRecords
		{
			get { return _totalRecords; }
			set { _totalRecords = value; }
		}


		/// <summary>
		/// Number of milliseconds the search took to complete.
		/// </summary>
		public int SearchDuration
		{
			get { return _SearchDuration; }
			set { _SearchDuration = value; }
		}




	}
}