using System;
using System.Xml;

namespace Graffiti.Core.Marketplace
{
    public class StatisticsInfo
    {
        private int _downloadCount = 0;
        private int _ratingSum = 0;
        private int _ratingCount = 0;
        private int _viewCount = 0;

        public StatisticsInfo(XmlNode node)
        {
            XmlNode n = node.SelectSingleNode("downloadCount");
            if (n != null)
                _downloadCount = int.Parse(n.InnerText);

            n = node.SelectSingleNode("ratingSum");
            if (n != null)
                _ratingSum = int.Parse(n.InnerText);

            n = node.SelectSingleNode("ratingCount");
            if (n != null)
                _ratingCount = int.Parse(n.InnerText);

            n = node.SelectSingleNode("viewCount");
            if (n != null)
                _viewCount = int.Parse(n.InnerText);
        }

        public int DownloadCount
        {
            get { return _downloadCount; }
            set { _downloadCount = value; }
        }

        public int AverageRating
        {
            get
            {
                if (_ratingCount > 0)
                    return (_ratingSum / _ratingCount);
                else
                    return 0;
            }
        }

        public int RatingSum
        {
            get { return _ratingSum; }
            set { _ratingSum = value; }
        }

        public int RatingCount
        {
            get { return _ratingCount; }
            set { _ratingCount = value; }
        }

        public int ViewCount
        {
            get { return _viewCount; }
            set { _viewCount = value; }
        }
    }
}
