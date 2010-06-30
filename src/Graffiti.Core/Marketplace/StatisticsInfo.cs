using System;
using System.Xml.Linq;

namespace Graffiti.Core.Marketplace
{
    public class StatisticsInfo
    {
        private int _downloadCount = 0;
        private int _viewCount = 0;

        public StatisticsInfo(XElement node)
        {
            string value;

            XElement n = node.Element("downloadCount");
            if (n != null && n.TryGetValue(out value))
                _downloadCount = int.Parse(value);

            n = node.Element("viewCount");
            if (n != null && n.TryGetValue(out value))
                _viewCount = int.Parse(value);
        }

        public int DownloadCount
        {
            get { return _downloadCount; }
            set { _downloadCount = value; }
        }

        public int ViewCount
        {
            get { return _viewCount; }
            set { _viewCount = value; }
        }
    }
}
