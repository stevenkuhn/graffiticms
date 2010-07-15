using System;

namespace GraffitiClient.API
{
    /// <summary>
    /// Represents a single Graffiti commments. Comments can be edited and deleted via the API but may not be created. 
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// The Id of the comment. ReadOnly.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The post this comment belongs to. ReadOnly.
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// The IPAddress of the commenter. ReadOnly.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// The comment details. 
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// (optional) The url/website the commentor entered when leaving the comment.
        /// </summary>
        public string WebSite { get; set; }

        /// <summary>
        /// Name of the user making the comment. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If the comment was left by a registerd user, this will be the username. 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Direct url of the comment.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Graffiti uses an interal API to score all comments for spam. 
        /// </summary>
        public int SpamScore { get; set; }

        /// <summary>
        /// DateTime when this comment was left. 
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Is this comment currently published. 
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// Has this comment been flagged as a Trackback (NOTE: This is for future functionaltiy). 
        /// </summary>
        public bool IsTrackback { get; set; }

        /// <summary>
        /// Has this comment been flagged to be deleted.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// The email address of the user leaving the comment. 
        /// </summary>
        public string Email { get; set; }
    }
}