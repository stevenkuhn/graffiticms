//------------------------------------------------------------------------------
// <copyright company="Telligent Systems">
//     Copyright (c) Telligent Systems Corporation.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using CookComputing.XmlRpc;

namespace Graffiti.Core
{

	public interface IMetaWeblog
	{

		#region MetaWeblog API

		[XmlRpcMethod("metaWeblog.newPost",
			 Description="Makes a new post to a designated weblog using the "
			 + "MetaWeblog API. Returns postid as a string.")]
		string newPost(
			string blogid,
			string username,
			string password,
			MetaWeblog.Post post,
			bool publish);

		[XmlRpcMethod("metaWeblog.editPost",Description="Updates and existing post to a designated weblog "
			 + "using the MetaWeblog API. Returns true if completed.")]
		bool editPost(
			string postid,
			string username,
			string password,
			MetaWeblog.Post post,
			bool publish);

		[XmlRpcMethod("metaWeblog.getPost",
			 Description="Retrieves an existing post using the MetaWeblog "
			 + "API. Returns the MetaWeblog struct.")]
		MetaWeblog.Post getPost(
			string postid,
			string username,
			string password);

        [XmlRpcMethod("mt.getCategoryList",
     Description = "Retrieves a list of valid categories for a post "
     + "using the MetaWeblog API. Returns the MetaWeblog categories "
     + "struct collection.")]
        MetaWeblog.CategoryInfo[] getCategories2(
            string blogid,
            string username,
            string password);


		[XmlRpcMethod("metaWeblog.getCategories",
			 Description="Retrieves a list of valid categories for a post "
			 + "using the MetaWeblog API. Returns the MetaWeblog categories "
			 + "struct collection.")]
		MetaWeblog.CategoryInfo[] getCategories(
			string blogid,
			string username,
			string password);

		[XmlRpcMethod("metaWeblog.getRecentPosts",
			 Description="Retrieves a list of the most recent existing post "
			 + "using the MetaWeblog API. Returns the MetaWeblog struct collection.")]
		MetaWeblog.Post[] getRecentPosts(
			string blogid,
			string username,
			string password,
			int numberOfPosts);


        [XmlRpcMethod("wp.getTags",
Description = "Retrieves a list of tags.")]
        MetaWeblog.Tag[] getTags(
            string blogid,
            string username,
            string password);

		[XmlRpcMethod("metaWeblog.newMediaObject",
			 Description = "Makes a new post to a designated blog using the "
			 + "MetaWeblog API. Returns file url as a string.")]
		MetaWeblog.MediaObjectInfo newMediaObject(
			string blogid,
			string username,
			string password,
			MetaWeblog.MediaObject mediaObject);

		#endregion

		#region BloggerAPI

		[XmlRpcMethod("blogger.deletePost",
			 Description="Deletes a post.")]
		[return: XmlRpcReturnValue(Description="Always returns true.")]
		bool deletePost(
			string appKey,
			string postid,
			string username,
			string password,
			[XmlRpcParameter(
				 Description="Where applicable, this specifies whether the weblog "
				 + "should be republished after the post has been deleted.")]
			bool publish);

		[XmlRpcMethod("blogger.getUsersBlogs",
			 Description="Returns information on all the blogs a given user "
			 + "is a member.")]
		MetaWeblog.BlogInfo[] getUsersBlogs(
			string appKey,
			string username,
			string password);

		[XmlRpcMethod("blogger.getUserInfo",
			 Description="Returns information about the given user.")]
		MetaWeblog.UserInfo getUserInfo(
			string appKey,
			string username,
			string password);

		#endregion

	}

}
