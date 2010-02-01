using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using DataBuddy;

namespace Graffiti.Core.API
{
    public enum TrueFalseIgnore
    {
        True,
        False,
        Ignore = 0
    }

    /// <summary>
    /// Helper to managed converting querystring parameters to a Post Query
    /// </summary>
    public class PostFilter
    {
        private string _author = string.Empty;
        private int _categoryId = 0;
        private PostStatus _status = PostStatus.NotSet;
        private TrueFalseIgnore _isDeleted = TrueFalseIgnore.False;
        private TrueFalseIgnore _isHome = TrueFalseIgnore.Ignore;
        private int _pageIndex = 1;
        private int _pageSize = 10;
        private bool _includeChildCategories = false;
        private int _revision;
        private int _parentId;

        // private constructor
        // force creations to come from querystring parsing
        private PostFilter()
        {
        }


        // parse querystring into filter
        public static PostFilter FromQueryString(NameValueCollection queryString)
        {
            PostFilter postFilter = new PostFilter();

            string id = queryString[QueryStringKey.Id];
            if(!string.IsNullOrEmpty(id))
            {
                postFilter.Id = Int32.Parse(id);
                return postFilter;
            }

            // parse and set author filter
            string author = queryString[QueryStringKey.Author];
            if (!string.IsNullOrEmpty(author))
                postFilter.Author = author;

            // parse and set category filter
            int categoryId;
            string cid = queryString[QueryStringKey.CategoryId];
            if ((!string.IsNullOrEmpty(cid)) && (int.TryParse(cid, out categoryId)))
                postFilter.CategoryId = categoryId;

            if(postFilter.CategoryId > 0)
            {
                postFilter.IncludeChildCategories =
                    Util.AreEqualIgnoreCase(queryString[QueryStringKey.IncludeChildCategories], "true");
            }

            // parse and set status filter
            string s = queryString[QueryStringKey.Status];
            if (!string.IsNullOrEmpty(s))
            {
                PostStatus status = PostStatus.NotSet;
                try { status = (PostStatus)Enum.Parse(typeof(PostStatus), s, true); }
                catch { }
                postFilter.Status = status;
            }

            // parse and set deleted filter
            bool isDeleted = false;
            string d = queryString[QueryStringKey.IsDeleted];
            if ((!string.IsNullOrEmpty(d)) && (bool.TryParse(d, out isDeleted)))
                postFilter.IsDeleted = isDeleted ? TrueFalseIgnore.True : TrueFalseIgnore.False;

            // parse and set page index filter
            int pageIndex;
            string pi = queryString[QueryStringKey.PageIndex];
            if ((!string.IsNullOrEmpty(pi)) && (int.TryParse(pi, out pageIndex)))
                postFilter.PageIndex = pageIndex;

            // parse and set page size filter
            int pageSize;
            string ps = queryString[QueryStringKey.PageSize];
            if ((!string.IsNullOrEmpty(ps)) && (int.TryParse(ps, out pageSize)))
                postFilter.PageSize = pageSize;

            int parnetId;
            string parId = queryString[QueryStringKey.ParentId];
            if ((!string.IsNullOrEmpty(parId)) && (int.TryParse(parId, out parnetId)))
                postFilter.ParentId = parnetId;

            return postFilter;
        }


        // generate query object from filter
        public Query ToQuery()
        {
            Query query = Post.CreateQuery();

            if (Id <= 0)
            {
                if (!string.IsNullOrEmpty(Author))
                    query.AndWhere(Post.Columns.UserName, Author);

                if (CategoryId > 0)
                {
                    Category category = new CategoryController().GetCachedCategory(CategoryId, false);

                    if(IncludeChildCategories && category.HasChildren)
                    {
                        List<int> ids = new List<int>(category.Children.Count + 1);
                        foreach (Category child in category.Children)
                            ids.Add(child.Id);

                        ids.Add(category.Id);

                        query.AndInWhere(Post.Columns.CategoryId, ids.ToArray());
                    }
                    else
                        query.AndWhere(Post.Columns.CategoryId, CategoryId);
                }
                if (Status == PostStatus.NotSet)
                    query.AndWhere(Post.Columns.Status, PostStatus.Publish);
                else
                    query.AndWhere(Post.Columns.Status, Status);

                if (IsDeleted != TrueFalseIgnore.Ignore)
                    query.AndWhere(Post.Columns.IsDeleted, IsDeleted == TrueFalseIgnore.True ? true : false);

                if (IsHome != TrueFalseIgnore.Ignore)
                    query.AndWhere(Post.Columns.IsHome, IsHome == TrueFalseIgnore.True ? true : false);

                if(ParentId > 0)
                    query.AndWhere(Post.Columns.ParentId,ParentId);


                query.PageIndex = PageIndex;
                query.PageSize = PageSize;

                query.OrderByDesc(Post.Columns.Published);
            }
            else
            {
                query.AndWhere(Post.Columns.Id, Id);
            }

            return query;
        }

        #region Properties...
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        public int CategoryId
        {
            get { return _categoryId; }
            set { _categoryId = value; }
        }

        public PostStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public TrueFalseIgnore IsDeleted
        {
            get { return _isDeleted; }
            set { _isDeleted = value; }
        }

        public TrueFalseIgnore IsHome
        {
            get { return _isHome; }
            set { _isHome = value; }
        }

        public int PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public int ParentId
        {
            get { return _parentId; }
            set { _parentId = value; }
        }

        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
	
        public bool IncludeChildCategories
        {
            get { return _includeChildCategories; }
            set { _includeChildCategories = value; }
        }

        public int Revision
        {
            get { return _revision; }
            set { _revision = value; }
        }

        #endregion
    }
}