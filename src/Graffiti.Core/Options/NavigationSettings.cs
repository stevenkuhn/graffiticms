using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Graffiti.Core
{
    /// <summary>
    /// Manages the custom navigation values. This object is stored in the ObjectStore
    /// </summary>
    [Serializable]
    public class NavigationSettings
    {
        private List<DynamicNavigationItem> _items = null;
        private object lockedObject = new object();

        public List<DynamicNavigationItem> SafeItems()
        {
            if (Items == null)
            {
                lock (lockedObject)
                {
                    if (Items == null)
                    {
                        Items = new List<DynamicNavigationItem>();

                        CategoryCollection cc = new CategoryController().GetTopLevelCachedCategories();
                        foreach (Category c in cc)
                        {
                            DynamicNavigationItem item = new DynamicNavigationItem();
                            item.CategoryId = c.Id;
                            item.NavigationType = DynamicNavigationType.Category;
                            item.Id = c.UniqueId;
                            Items.Add(item);
                        }
                    }
                }
            }
            return Items;
        }

        public List<DynamicNavigationItem> Items
        {
            get{return _items;}
            set { _items = value; }
        }
	


        public void Save()
        {
           ObjectManager.Save(this, "NavigationSettings");
        }

        public static NavigationSettings Get()
        {
            return ObjectManager.Get<NavigationSettings>("NavigationSettings");
        }

        /// <summary>
        /// Adds the new item to the list
        /// </summary>
        /// <param name="item"></param>
        public static void Add(DynamicNavigationItem item)
        {
            NavigationSettings settings = Get();
            item.Order = Int16.MaxValue;
            settings.SafeItems().Add(item);
            settings.Save();
        }

        /// <summary>
        /// Remove an item by Id
        /// </summary>
        /// <param name="id"></param>
        public static void Remove(Guid id)
        {
            NavigationSettings settings = Get();
            int index = -1;
            for (int i = 0; i < settings.SafeItems().Count; i++)
            {
                if (settings.SafeItems()[i].Id == id)
                {
                    index = i;
                    break;
                }
            }

            if(index > -1)
            {
                settings.SafeItems().RemoveAt(index);
                settings.Save();
            }
        }

        /// <summary>
        /// Reorder. Expects a serialized list from scriptaculous
        /// </summary>
        /// <param name="list"></param>
        public static void ReOrder(string list)
        {
            list = Regex.Replace(list, @"Category\-|Link\-|Post\-", string.Empty);
            string[] items = list.Split(new string[] {"&"}, StringSplitOptions.RemoveEmptyEntries);
            int i = 0;
            NavigationSettings settings = Get();

            foreach (string id in items)
            {
                Guid g = new Guid(id);
                foreach (DynamicNavigationItem item in settings.SafeItems())
                {
                    if(item.Id == g)
                    {
                        item.Order = i;
                        i++;
                        break;
                    }
                }
            }

            settings.SafeItems().Sort(delegate(DynamicNavigationItem i1, DynamicNavigationItem i2)
                {
                    return Comparer<int>.Default.Compare(i1.Order, i2.Order);
                }
             );

            settings.Save();
        }
    }

    public enum DynamicNavigationType
    {
        Category = 1,
        Post = 3,
        Link = 5
    }

    [Serializable]
    public class DynamicNavigationItem
    {
        private Guid _id;

        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }
	

        private DynamicNavigationType _navigationType;

        public DynamicNavigationType NavigationType
        {
            get { return _navigationType; }
            set { _navigationType = value; }
        }
	

        private int _categoryId;

        public int CategoryId
        {
            get { return _categoryId; }
            set { _categoryId = value; }
        }

        private int _postId;

        public int PostId
        {
            get { return _postId; }
            set { _postId = value; }
        }

        private string _text;

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        private string _href;

        public string Href
        {
            get { return _href; }
            set { _href = value; }
        }

        private int _order;

        public int Order
        {
            get { return _order; }
            set { _order = value; }
        }
	
        public string Name
        {
            get
            {
                switch(NavigationType)
                {
                    case DynamicNavigationType.Category:
                        return new CategoryController().GetCachedCategory(CategoryId, false).Name;
                    case DynamicNavigationType.Post:
                        return new Post(PostId).Title;
                    default:
                        return Text;
                }
            }
        }

    }
}
