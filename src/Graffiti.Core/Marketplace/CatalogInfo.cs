using System;
using System.Xml.Linq;

namespace Graffiti.Core.Marketplace
{
    public class CatalogInfo
    {
        private int _id = 0;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private CatalogType _type = CatalogType.All;

        public CatalogInfo(XElement node)
        {
            string value;

            if (node.TryGetAttributeValue("id", out value))
                _id = int.Parse(value);

            XElement n = node.Element("name");
            if (n != null && n.TryGetValue(out value))
                _name = value;

            n = node.Element("description");
            if (n != null && n.TryGetValue(out value))
                _description = value;

            n = node.Element("type");
            if (n != null && n.TryGetValue(out value))
                _type = (CatalogType)System.Enum.Parse(typeof(CatalogType), value);
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public CatalogType Type
        {
            get { return _type; }
            set { _type = value; }
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

        public CategoryInfoCollection Categories
        {
            get
            {
                CategoryInfoCollection categories = ZCache.Get<CategoryInfoCollection>(CategoryCacheKey);
                if (categories == null)
                {
                    RefreshCategories();
                    categories = ZCache.Get<CategoryInfoCollection>(CategoryCacheKey);
                }
                return categories;
            }
        }

        public ItemInfoCollection Items
        {
            get
            {
                ItemInfoCollection items = ZCache.Get<ItemInfoCollection>(ItemCacheKey);
                if (items == null)
                {
                    RefreshItems();
                    items = ZCache.Get<ItemInfoCollection>(ItemCacheKey);
                }
                return items;
            }
        }

        public CreatorInfoCollection Creators
        {
            get
            {
                CreatorInfoCollection creators = ZCache.Get<CreatorInfoCollection>(CreatorCacheKey);
                if (creators == null)
                {
                    RefreshCreators();
                    creators = ZCache.Get<CreatorInfoCollection>(CreatorCacheKey);
                }
                return creators;
            }
        }

        public MessageInfoCollection Messages
        {
            get
            {
                MessageInfoCollection messages = ZCache.Get<MessageInfoCollection>(MessageCacheKey);
                if (messages == null)
                {
                    RefreshMessages();
                    messages = ZCache.Get<MessageInfoCollection>(MessageCacheKey);
                }
                return messages;
            }
        }

        public void RefreshCategories()
        {
            XDocument doc = XDocument.Load(CategoryUrl);
            CategoryInfoCollection categories = new CategoryInfoCollection(this, doc.Element("categories").Elements("categoryInfo"));
            ZCache.InsertCache(CategoryCacheKey, categories, Marketplace.CacheTime);
        }

        public void RefreshItems()
        {
            XDocument doc = XDocument.Load(ItemUrl);
            ItemInfoCollection items = new ItemInfoCollection(this, doc.Element("items").Elements("itemInfo"));
            ZCache.InsertCache(ItemCacheKey, items, Marketplace.CacheTime);
        }

        public void RefreshCreators()
        {
            CreatorInfoCollection creators = new CreatorInfoCollection();
            foreach (ItemInfo item in Items.Values)
            {
                if (!creators.ContainsKey(item.CreatorId))
                    creators.Add(item.CreatorId, item.Creator);
            }
            ZCache.InsertCache(CreatorCacheKey, creators, Marketplace.CacheTime);
        }

        public void RefreshMessages()
        {
            XDocument doc = XDocument.Load(MessageUrl);
            MessageInfoCollection messages = new MessageInfoCollection(doc.Element("messages").Elements("messageInfo"));
            ZCache.InsertCache(MessageCacheKey, messages, Marketplace.CacheTime);
        }

        private string CategoryUrl
        {
            get { return Url("categories"); }
        }

        private string ItemUrl
        {
            get { return Url("items"); }
        }

        private string MessageUrl
        {
            get { return Url("messages"); }
        }

        private string Url(string path)
        {
            return string.Format(Marketplace.UrlFormat, CatalogUrl, path);
        }

        private string CatalogUrl
        {
            get { return string.Format(Marketplace.UrlFormat, Marketplace.CatalogsUrl, Name); }
        }

        private string CategoryCacheKey
        {
            get { return CacheKey("categories"); }
        }

        private string ItemCacheKey
        {
            get { return CacheKey("items"); }
        }

        private string CreatorCacheKey
        {
            get { return CacheKey("creators"); }
        }

        private string MessageCacheKey
        {
            get { return CacheKey("messages"); }
        }

        private string CacheKey(string type)
        {
            return string.Format(Marketplace.CacheKeyFormat, CatalogCacheKey, type);
        }

        private string CatalogCacheKey
        {
            get { return string.Format(Marketplace.CacheKeyFormat, Marketplace.CatalogsCacheKey, Name); }
        }
    }
}
