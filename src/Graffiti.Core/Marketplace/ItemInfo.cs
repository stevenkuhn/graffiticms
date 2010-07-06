using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Graffiti.Core.Marketplace
{
    public class ItemInfo
    {
        private CatalogInfo _catalog;
        private int _id = 0;
        private int _categoryId = 0;
        private string _creatorId = string.Empty;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private string _version = string.Empty;
        private string _downloadUrl = string.Empty;
        private string _screenshotUrl = string.Empty;
        private string _iconUrl = string.Empty;
        private int _worksWithMajorVersion = 0;
        private int _worksWithMinorVersion = 0;
        private bool _requiresManualIntervention = false;
        private DateTime _dateAdded = DateTime.MinValue;
        private StatisticsInfo _statistics;
        private PurchaseInfo _purchase;
        private List<string> _tags = new List<string>();

        public ItemInfo(CatalogInfo catalog, XElement node)
        {
            _catalog = catalog;

            string value;

            if (node.TryGetAttributeValue("id", out value))
                _id = int.Parse(value);
            if (node.TryGetAttributeValue("categoryId", out value))
                _categoryId = int.Parse(value);
            if (node.TryGetAttributeValue("creatorId", out value))
                _creatorId = value;

            XElement n = node.Element("name");
            if (n != null && n.TryGetValue(out value))
                _name = value;

            n = node.Element("description");
            if (n != null && n.TryGetValue(out value))
                _description = value;

            n = node.Element("version");
            if (n != null && n.TryGetValue(out value))
                _version = value;

            n = node.Element("downloadUrl");
            if (n != null && n.TryGetValue(out value))
                _downloadUrl = value;

            n = node.Element("screenshotUrl");
            if (n != null && n.TryGetValue(out value))
                _screenshotUrl = value;

            n = node.Element("iconUrl");
            if (n != null && n.TryGetValue(out value))
                _iconUrl = value;

            n = node.Element("worksWithMajorVersion");
            if (n != null && n.TryGetValue(out value))
                _worksWithMajorVersion = int.Parse(value);

            n = node.Element("worksWithMinorVersion");
            if (n != null && n.TryGetValue(out value))
                _worksWithMinorVersion = int.Parse(value);

            n = node.Element("requiresManualIntervention");
            if (n != null && n.TryGetValue(out value))
                _requiresManualIntervention = bool.Parse(value);

            n = node.Element("dateAdded");
            if (n != null && n.TryGetValue(out value))
                _dateAdded = DateTime.Parse(value);

            n = node.Element("statisticsInfo");
            if (n != null)
                _statistics = new StatisticsInfo(n);

            n = node.Element("purchaseInfo");
            if (n != null)
                _purchase = new PurchaseInfo(n);

            n = node.Element("tags");
            if (n != null)
            {
                foreach (XElement e in n.Elements("tag"))
                {
                    string tag;
                    if (e.TryGetValue(out tag))
                        _tags.Add(tag);
                }
            }
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
            get 
            { 
                if (Catalog.Categories.ContainsKey(CategoryId))
                    return Catalog.Categories[CategoryId];
                return null;
            }
        }

        public string CreatorId
        {
            get { return _creatorId; }
            set { _creatorId = value; }
        }

        public CreatorInfo Creator
        {
            get 
            { 
                if (Marketplace.Creators.ContainsKey(CreatorId))
                    return Marketplace.Creators[CreatorId]; 
                return null;
            }
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

        public string Tags
        {
            get { return string.Join(", ", _tags.ToArray()); }
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
