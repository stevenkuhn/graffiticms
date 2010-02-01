using System;
using System.Collections.Generic;
using System.Xml;

namespace Graffiti.Core.Marketplace
{
    public class ItemInfo
    {
        private CatalogInfo _catalog;
        private int _id = 0;
        private int _categoryId = 0;
        private int _creatorId = 0;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private string _version = string.Empty;
        private int _size = 0;
        private string _downloadUrl = string.Empty;
        private string _screenshotUrl = string.Empty;
        private string _iconUrl = string.Empty;
        private int _worksWithMajorVersion = 0;
        private int _worksWithMinorVersion = 0;
        private bool _requiresManualIntervention = false;
        private bool _isApproved = false;
        private DateTime _dateAdded = DateTime.MinValue;
        private StatisticsInfo _statistics;
        private PurchaseInfo _purchase;

        public ItemInfo(CatalogInfo catalog, XmlNode node)
        {
            _catalog = catalog;

            XmlAttribute a = node.Attributes["id"];
            if (a != null)
                _id = int.Parse(a.Value);

            a = node.Attributes["categoryId"];
            if (a != null)
                _categoryId = int.Parse(a.Value);

            a = node.Attributes["creatorId"];
            if (a != null)
                _creatorId = int.Parse(a.Value);

            XmlNode n = node.SelectSingleNode("name");
            if (n != null)
                _name = n.InnerText;

            n = node.SelectSingleNode("description");
            if (n != null)
                _description = n.InnerText;

            n = node.SelectSingleNode("version");
            if (n != null)
                _version = n.InnerText;

            n = node.SelectSingleNode("size");
            if (n != null)
                _size = int.Parse(n.InnerText);

            n = node.SelectSingleNode("downloadUrl");
            if (n != null)
                _downloadUrl = n.InnerText;

            n = node.SelectSingleNode("screenshotUrl");
            if (n != null)
                _screenshotUrl = n.InnerText;

            n = node.SelectSingleNode("iconUrl");
            if (n != null)
                _iconUrl = n.InnerText;

            n = node.SelectSingleNode("worksWithMajorVersion");
            if (n != null)
                _worksWithMajorVersion = int.Parse(n.InnerText);

            n = node.SelectSingleNode("worksWithMinorVersion");
            if (n != null)
                _worksWithMinorVersion = int.Parse(n.InnerText);

            n = node.SelectSingleNode("requiresManualIntervention");
            if (n != null)
                _requiresManualIntervention = bool.Parse(n.InnerText);

            n = node.SelectSingleNode("isApproved");
            if (n != null)
                _isApproved = bool.Parse(n.InnerText);

            n = node.SelectSingleNode("dateAdded");
            if (n != null)
                _dateAdded = DateTime.Parse(n.InnerText);

            n = node.SelectSingleNode("statisticsInfo");
            if (n != null)
                _statistics = new StatisticsInfo(n);

            n = node.SelectSingleNode("purchaseInfo");
            if (n != null)
                _purchase = new PurchaseInfo(n);
        }

        public CatalogInfo Catalog
        {
            get { return _catalog; }
            set { _catalog = value; }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int CategoryId
        {
            get { return _categoryId; }
            set { _categoryId = value; }
        }

        public CategoryInfo Category
        {
            get { return Catalog.Categories[CategoryId]; }
        }

        public int CreatorId
        {
            get { return _creatorId; }
            set { _creatorId = value; }
        }

        public CreatorInfo Creator
        {
            get { return Marketplace.Creators[CreatorId]; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public string FormattedSize
        {
            get
            {
                if (_size < 0)
                    return string.Empty;
                else if (_size == 1)
                    return string.Format("{0} byte", _size);
                else if (_size < 1000)
                    return string.Format("{0} bytes", _size);
                else if (_size < 1000000)
                    return string.Format("{0:G3} KB", ((double)_size) / 1000.0);
                else if (_size < 1000000000)
                    return string.Format("{0:G3} MB", ((double)_size) / 1000000.0);
                else
                    return "Huge";
            }
        }

        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public string DownloadUrl
        {
            get { return _downloadUrl; }
            set { _downloadUrl = value; }
        }

        public string FileName
        {
            get { return DownloadUrl.Substring(DownloadUrl.LastIndexOf('/') + 1); }
        }

        public string ScreenshotUrl
        {
            get { return _screenshotUrl; }
            set { _screenshotUrl = value; }
        }

        public string IconUrl
        {
            get { return _iconUrl; }
            set { _iconUrl = value; }
        }

        public int WorksWithMajorVersion
        {
            get { return _worksWithMajorVersion; }
            set { _worksWithMajorVersion = value; }
        }

        public int WorksWithMinorVersion
        {
            get { return _worksWithMinorVersion; }
            set { _worksWithMinorVersion = value; }
        }

        public bool RequiresManualIntervention
        {
            get { return _requiresManualIntervention; }
            set { _requiresManualIntervention = value; }
        }

        public bool IsApproved
        {
            get { return _isApproved; }
            set { _isApproved = value; }
        }

        public DateTime DateAdded
        {
            get { return _dateAdded; }
            set { _dateAdded = value; }
        }

        public StatisticsInfo Statistics
        {
            get { return _statistics; }
            set { _statistics = value; }
        }

        public PurchaseInfo Purchase
        {
            get { return _purchase; }
            set { _purchase = value; }
        }
    }
}
