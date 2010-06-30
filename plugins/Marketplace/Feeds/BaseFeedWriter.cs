using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using Graffiti.Core;

namespace Graffiti.Marketplace
{
	public abstract class BaseFeedWriter : IHttpHandler
	{

		/// <summary>
		/// The HTTP context that we're replying to.
		/// </summary>
		public HttpContext Context { get; set; }

		/// <summary>
		/// The length of time the feed should be cached in seconds.
		/// </summary>
		protected virtual int CacheTime
		{
			get { return 30; }
		}

		/// <summary>
		/// The key used to lookup the feed uniquely in the cache.
		/// </summary>
		protected abstract string CacheKey { get; }

		/// <summary>
		/// Builds the actual feed.
		/// </summary>
		protected abstract string BuildFeed();

		/// <summary>
		/// Write the feed to the response stream
		/// </summary>
		protected virtual void WriteFeed(string feed)
		{
			Context.Response.Clear();
			Context.Response.ContentEncoding = System.Text.Encoding.UTF8;
			Context.Response.ContentType = "text/xml";
			Context.Response.Cache.SetCacheability(HttpCacheability.Public);
			Context.Response.Write(feed);
		}

        /// <summary>
        /// Write the feed to the response stream
        /// </summary>
        protected static SqlConnection CreateMarketplaceConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["Graffiti"].ConnectionString);
        }

		/// <summary>
		/// Process the request for the feed.
		/// </summary>
		/// <param name="context">The HTTP context for the request.</param>
		public virtual void ProcessRequest(HttpContext context)
		{
			Context = context;

            string feed = ZCache.Get<string>(this.CacheKey);
			if (feed == null)
			{
				feed = BuildFeed();

                ZCache.InsertCache(this.CacheKey, feed, this.CacheTime);
			}

			WriteFeed(feed);
		}

		public virtual bool IsReusable
		{
			get { return false; }
		}

	}
}
