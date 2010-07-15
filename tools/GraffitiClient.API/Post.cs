using System;
using System.Collections.Specialized;

namespace GraffitiClient.API
{
    /// <summary>
    /// Represents a Graffiti Post
    /// </summary>
    public class Post
    {
        private int _status = 1;
        private NameValueCollection _customFields = new NameValueCollection();

        /// <summary>
        /// The Id of the Post. This value is readonly.
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// The title of the post. This value will be used to generate the post name if one is not set on the Name property.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The Name of the post link (url). If null, the title will be used. This value must be unqiue per category. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Readonly fullyqualifed url to the post.
        /// </summary>
        public string Url { get; internal set; }

        /// <summary>
        /// Comma seperated list of tags for this post. 
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// The post content. This value will generally be used as a lead in (excerpt) if the ExtendedBody property is null. 
        /// </summary>
        public string PostBody { get; set; }

        /// <summary>
        /// The post body + extended body.
        /// </summary>
        public string Body { get; internal set; }

        /// <summary>
        /// Additional post content. When this value exists, the Body is generally considered a lead in (excerpt). 
        /// </summary>
        public string ExtendedBody { get; set; }

        /// <summary>
        /// Local time this post was published. This value may be in the future. 
        /// </summary>
        public DateTime PublishDate { get; set; }

        /// <summary>
        /// The work flow status of this post. If the user is not an Admin or Manager this value generally cannot be changed. 0 = NotSet, 1 = Publish, 2 = Draft, 3 = Pending, 4 = RequiresChanges
        /// </summary>
        public int Status
        {
            get { return _status; }
            set { _status = value; }
        }

        /// <summary>
        /// (user)Name of the user who wrote the post. 
        /// </summary>
        public string Author { get; internal set; }

        /// <summary>
        /// Number of times this post has been viewed. ReadOnly.
        /// </summary>
        public int Views { get; internal set; }

        /// <summary>
        /// Unique Id of the category this post is assigned to.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Number of published comments for this post. ReadOnly.
        /// </summary>
        public int CommentCount { get; internal set; }

        /// <summary>
        /// Number of comments for this post which require approval. ReadOnly.
        /// </summary>
        public int PendingCommentCount { get; internal set; }

        /// <summary>
        /// Flag which defines if this post is marked as deleted. 
        /// </summary>
        public bool IsDeleted { get; internal set; }

        public string ContentType { get; set; }

        /// <summary>
        /// Used if this post's category's sort order type is set to custom. 
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Last user to modify this post.
        /// </summary>
        public string ModifiedBy { get; internal set; }

        /// <summary>
        /// The user who initially created this post. 
        /// </summary>
        public string CreatedBy { get; internal set; }

        /// <summary>
        /// The last datetime this post was modified. This value is in the server datetime and is readonly. 
        /// </summary>
        public DateTime ModifiedOn { get; internal set; }


        /// <summary>
        /// The datetime this post was created. This value is in the server datetime and is readonly. 
        /// </summary>
        public DateTime CreatedOn { get; internal set; }

        /// <summary>
        /// Are comments enabled on this post. It does not take into account if comments are enabled site wide. 
        /// </summary>
        public bool EnableComments { get; set; }

        /// <summary>
        /// Is this post current published. 
        /// </summary>
        public bool IsPublished { get; internal set; }

        /// <summary>
        /// Is this post displayed on the home page if home filtering is enabled. 
        /// </summary>
        public bool IsHome { get; set; }

        /// <summary>
        /// If IsHome is enabled, the post order.
        /// </summary>
        public int HomeSortOrder { get; set; }

        /// <summary>
        /// Which post is this post a child of. (NOTE: This is for future functionality).
        /// </summary>
        public int ParentId { get; internal set; }

        /// <summary>
        /// Metadescription to be used on this post's page.
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// MetaKeywords to use for this post. If null, tags will be used.
        /// </summary>
        public string MetaKeywords { get; set; }

        public NameValueCollection CustomFields
        {
            get { return _customFields; }
            internal set { _customFields = value; }
        }
    }
}