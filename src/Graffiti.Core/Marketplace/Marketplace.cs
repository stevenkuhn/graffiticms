using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml.Linq;

namespace Graffiti.Core.Marketplace
{
    public static class Marketplace
    {
        private static readonly string _settingBaseUrl = "Graffiti:Marketplace:BaseUrl";
        private static readonly string _settingCacheTime = "Graffiti:Marketplace:CacheTime";
        private static readonly string _urlFormat = "{0}/{1}";
        private static readonly string _cacheKeyFormat = "{0}-{1}";

        public static CatalogInfoCollection Catalogs
        {
            get
            {
                CatalogInfoCollection catalogs = ZCache.Get<CatalogInfoCollection>(CatalogsCacheKey);
                if (catalogs == null)
                {
                    RefreshCatalogs();
                    catalogs = ZCache.Get<CatalogInfoCollection>(CatalogsCacheKey);
                }
                return catalogs;
            }
        }

        public static CreatorInfoCollection Creators
        {
            get
            {
                CreatorInfoCollection creators = ZCache.Get<CreatorInfoCollection>(CreatorsCacheKey);
                if (creators == null)
                {
                    RefreshCreators();
                    creators = ZCache.Get<CreatorInfoCollection>(CreatorsCacheKey);
                }
                return creators;
            }
        }

        public static MessageInfoCollection Messages
        {
            get
            {
                MessageInfoCollection messages = ZCache.Get<MessageInfoCollection>(MessagesCacheKey);
                if (messages == null)
                {
                    RefreshMessages();
                    messages = ZCache.Get<MessageInfoCollection>(MessagesCacheKey);
                }
                return messages;
            }
        }

        public static void RefreshCatalogs()
        {
            XDocument doc = XDocument.Load(CatalogsUrl);
            CatalogInfoCollection catalogs = new CatalogInfoCollection(doc.Element("catalogs").Elements("catalogInfo"));
            ZCache.InsertCache(CatalogsCacheKey, catalogs, CacheTime);
        }

        public static void RefreshCreators()
        {
            XDocument doc = XDocument.Load(CreatorsUrl);
            CreatorInfoCollection creators = new CreatorInfoCollection(doc.Element("creators").Elements("creatorInfo"));
            ZCache.InsertCache(CreatorsCacheKey, creators, CacheTime);
        }

        public static void RefreshMessages()
        {
            XDocument doc = XDocument.Load(MessagesUrl);
            MessageInfoCollection messages = new MessageInfoCollection(doc.Element("messages").Elements("messageInfo"));
            ZCache.InsertCache(MessagesCacheKey, messages, CacheTime);
        }

        #region Helpers...

        public static string CatalogsUrl
        {
            get { return Url("catalogs"); }
        }

        public static string CreatorsUrl
        {
            get { return Url("creators"); }
        }

        public static string MessagesUrl
        {
            get { return Url("messages"); }
        }

        public static string UrlFormat
        {
            get { return _urlFormat; }
        }

        private static string Url(string path)
        {
            string baseUrl = "http://extendgraffiti.com/data";
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[_settingBaseUrl]))
                baseUrl = ConfigurationManager.AppSettings[_settingBaseUrl];

            return string.Format(UrlFormat, baseUrl, path);
        }

        public static string CatalogsCacheKey
        {
            get { return CacheKey("catalogs"); }
        }

        private static string CreatorsCacheKey
        {
            get { return CacheKey("creators"); }
        }

        private static string MessagesCacheKey
        {
            get { return CacheKey("messages"); }
        }

        public static string CacheKeyFormat
        {
            get { return _cacheKeyFormat; }
        }

        private static string CacheKey(string type)
        {
            return string.Format(CacheKeyFormat, "marketplace", type);
        }

        public static int CacheTime
        {
            get { return int.Parse(ConfigurationManager.AppSettings[_settingCacheTime]) * 60; }
        }

        #endregion
    }
}
