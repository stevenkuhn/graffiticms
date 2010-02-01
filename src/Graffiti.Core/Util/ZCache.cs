using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

namespace Graffiti.Core
{

    /// <summary>
    /// Summary description for ZCache
    /// </summary>
    public static class ZCache
    {
        private static Cache GetCache()
        {
            HttpContext cntx = HttpContext.Current;
            if (cntx != null)
                return cntx.Cache;
            else
                return HttpRuntime.Cache;
        }

        public static void InsertCache(string key, object obj, int seconds, CacheDependency dep)
        {
            GetCache().Insert(key, obj, dep, DateTime.Now.AddSeconds(seconds), TimeSpan.Zero, CacheItemPriority.Normal, null);
        }

        public static void InsertCache(string key, object obj, int seconds)
        {
            GetCache().Insert(key, obj, null, DateTime.Now.AddSeconds(seconds), TimeSpan.Zero, CacheItemPriority.Normal, null);
        }

        public static void MaxCache(string key, object obj)
        {
            MaxCache(key, obj, null);
        }

        public static void MaxCache(string key, object obj, CacheDependency dep)
        {
            GetCache().Insert(key, obj, dep, DateTime.Now.AddDays(1), TimeSpan.Zero, CacheItemPriority.NotRemovable, null);
        }

        public static T Get<T>(string key) where T : class
        {
            return GetCache()[key] as T;
        }

        public static void RemoveCache(string key)
        {
            GetCache().Remove(key);
        }

        public static void Clear()
        {
            Cache cache = GetCache();
            IDictionaryEnumerator CacheEnum = cache.GetEnumerator();
            List<string> keys = new List<string>();
            while (CacheEnum.MoveNext())
            {
                keys.Add(CacheEnum.Key.ToString());
            }

            foreach (string key in keys)
            {
                cache.Remove(key);
            }

        }

        public static void RemoveByPattern(string pattern)
        {
            Cache cache = GetCache();
            IDictionaryEnumerator CacheEnum = cache.GetEnumerator();
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            List<string> keys = new List<string>();
            while (CacheEnum.MoveNext())
            {
                if (regex.IsMatch(CacheEnum.Key.ToString()))
                    keys.Add(CacheEnum.Key.ToString());

            }

            if (keys.Count > 0)
            {
                foreach (string key in keys)
                {
                    cache.Remove(key);
                }
            }

        }
    }
}