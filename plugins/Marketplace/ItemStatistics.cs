using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graffiti.Marketplace
{
    /// <summary>
    /// Represents the statistics info for a marketplace item (post)
    /// Currently only DownloadCount is used.
    /// Ratings to be implemented in future version.
    /// </summary>
    public class ItemStatistics
    {
        public int DownloadCount { get; set; }
        public int RatingSum { get; set; }
        public int RatingCount { get; set; }

    }
}
