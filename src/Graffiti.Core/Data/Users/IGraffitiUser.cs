using System;

namespace Graffiti.Core
{
    /// <summary>
    /// Data about a particular user. ($user, $post.User)
    /// </summary>
    public interface IGraffitiUser : IUser
    {

        /// <summary>
        /// The user's email address
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// The user's public email address
        /// </summary>
        string PublicEmail { get; set; }

        /// <summary>
        /// The user's time zone
        /// </summary>
        double TimeZoneOffSet { get; set; }

        /// <summary>
        /// The user's bio
        /// </summary>
        string Bio { get; set; }

        /// <summary>
        /// The user's avatar
        /// </summary>
        string Avatar { get; set; }

        /// <summary>
        /// The unique ID (guid) of the user
        /// </summary>
        Guid UniqueId { get; set;}

        /// <summary>
        /// An array of roles the user is in
        /// </summary>
        string[] Roles { get; }
    }
}