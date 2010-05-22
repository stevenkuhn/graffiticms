using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using DataBuddy;

namespace Graffiti.Core
{
    //This file contains extensions to the SubSonic generated classes Category, CategoryCollection, and CategoryController

    public partial class Category : IMenuItem
    {
        public SortOrderType SortOrder
        {
            get { return (SortOrderType) SortOrderTypeId; }
            set { SortOrderTypeId = (int) value; }
        }

        //Local value containing the link name of the category. We compare this
        //value when saving changes to see if there has been a change
        private string __initialCategoryName = null;

        protected override void Loaded()
        {
            base.Loaded();

            //Store the value in a local variable to check pre-save for changes
            __initialCategoryName = LinkName;
        }

        protected override void BeforeValidate()
        {
            base.BeforeValidate();

            if (IsNew)
            {
                if(UniqueId == Guid.Empty)
                    UniqueId = Guid.NewGuid();
            }

            if (string.IsNullOrEmpty(FormattedName))
                FormattedName = Util.CleanForUrl(Name);

            //We append the parent link name if it exists
            if (ParentId <= 0)
                LinkName = Util.CleanForUrl(FormattedName);
            else
                LinkName = new CategoryController().GetCachedCategory(ParentId, false).LinkName + "/" + Util.CleanForUrl(FormattedName);

            if(!Util.IsValidFileName(LinkName))
            {
                throw new Exception("Sorry, you cannot use the reserved word *" + LinkName + "* for a file name.");
            }

            if(string.IsNullOrEmpty(FeedUrlOverride))
                FeedUrlOverride = null;

            //We need to protected against category names colliding with uncategorized posts.
            //Uncategorized posts also do the same check
            if (Name != CategoryController.UncategorizedName)
            {
                Query q = Post.CreateQuery();
                q.AndWhere(Post.Columns.Name, LinkName);
                q.AndWhere(Post.Columns.CategoryId, CategoryController.UnCategorizedId);
                if (q.GetRecordCount() > 0)
                    throw new Exception("Categories cannot have the same name as an uncategorized post");
            }

            //Check for changes. At this time, we do not support renaming 
            //categories if it has a post. 
            //if(__initialCategoryName != null && __initialCategoryName != LinkName)
            //{
            //    Query q = Post.CreateQuery();
            //    q.AddWhere(Post.Columns.CategoryId, Id);
            //    if (q.GetRecordCount() > 0)
            //        throw new Exception("Sorry, at this time you cannot rename a category with posts in it.");
            //}

            if (string.IsNullOrEmpty(Body) || Body == "<p>&nbsp;</p>")
                Body = null;
        }

        protected override void AfterCommit()
        {
            base.AfterCommit();

            //Clear the cache
            CategoryController.Reset();

            //right pages so we can avoid UrlRewriting
            WritePages();
        }

        /// <summary>
        /// Returns true if the Id of this category matches CategoryController.UnCategorizedId
        /// </summary>
        public bool IsUncategorized
        {
            get { return Id == CategoryController.UnCategorizedId; }
        }

        /// <summary>
        /// Writes empty pages to disk. 
        /// </summary>
        public void WritePages()
        {
            PageTemplateToolboxContext templateContext = new PageTemplateToolboxContext();
            templateContext.Put("CategoryId", Id);
            templateContext.Put("CategoryName", LinkName);
            templateContext.Put("MetaDescription",
                                !string.IsNullOrEmpty(MetaDescription)
                                    ? MetaDescription
                                    : HttpUtility.HtmlEncode(Util.RemoveHtml(Body, 255) ?? string.Empty));

            templateContext.Put("MetaKeywords",
                                !string.IsNullOrEmpty(MetaKeywords)
                                    ? MetaKeywords
                                    : Name);
                                   


            if (!IsUncategorized)
            {
                PageWriter.Write("category.view", "~/" + LinkName + "/" + Util.DEFAULT_PAGE, templateContext);
				PageWriter.Write("categoryrss.view", "~/" + LinkName + "/feed/" + Util.DEFAULT_PAGE, templateContext);

            }

            if(__initialCategoryName != null && __initialCategoryName != LinkName)
            {
                PostCollection pc = new PostCollection();
                Query postQuery = Post.CreateQuery();
                postQuery.AndWhere(Post.Columns.CategoryId, Id);
                pc.LoadAndCloseReader(postQuery.ExecuteReader());
                foreach(Post p in pc)
                {
                    p.Save();
                }
            }
        }

        #region Properties

        private CategoryCollection _children = new CategoryCollection();
        private object lockedObject = new object();

        private bool _loadedChildren = false;

        /// <summary>
        /// Returns the sub categories if they exist. 
        /// </summary>
        public CategoryCollection Children
        {
            get
            {
                if(!_loadedChildren)
                {
                    lock (lockedObject)
                    {
                        if (!_loadedChildren)
                        {
                            foreach (Category c in new CategoryController().GetCachedCategories())
                            {
                                if (c.ParentId == Id)
                                    _children.Add(c);
                            }

                            _children.Sort(delegate(Category c1, Category c2)
                                               {
                                                   return
                                                       Comparer<string>.Default.Compare(c1.Name,
                                                                                        c2.Name);
                                               });
                        }
                        _loadedChildren = true;
                    }
                }
                 return _children;
            }

        }

        /// <summary>
        /// Returns the parent Category object
        /// </summary>
        public Category Parent
        {
            get 
            {
                return new CategoryController().GetCachedCategory(this.ParentId, true);
            }
        }

        /// <summary>
        /// Returns true if ParentId > 0
        /// </summary>
        public bool HasParent
        {
            get { return ParentId > 0; }
        }


        /// <summary>
        /// Returns true if ParentId > -1 and Children.Count > 0
        /// </summary>
        public bool HasChildren
        {
            get { return ParentId <= 0 && Children.Count > 0; }
        }

        /// <summary>
        /// Returns the absolute url of this category
        /// </summary>
        public string Url
        {
            get
            {
                return VirtualPathUtility.ToAbsolute(VirtualUrl);
            }
        }

        /// <summary>
        /// Returns the virtual url of this category (ie, ~/)
        /// </summary>
        public string VirtualUrl
        {
            get
            {
                return "~/" + LinkName + "/";
            }
        }

        #endregion

        public static bool IncludeChildPosts
        {
            get { return SiteSettings.Get().IncludeChildPosts; }
        }

        #region IMenuItem Members

        public List<MenuItem> GetMenuItems()
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            menuItems.Add(new MenuItem("Id", "$category.Id", "The category id", "Category"));
            menuItems.Add(new MenuItem("Name", "$category.Name", "The proper name of the category", "Category"));
            menuItems.Add(new MenuItem("LinkName", "$category.LinkName", "The value used in the category url", "Category"));
            menuItems.Add(new MenuItem("FeedUrlOverride", "$category.FeedUrlOverride", "Optional value used direct requests to the category feed feedburner", "Category"));
            menuItems.Add(new MenuItem("Body", "$category.Body", "A description of the category", "Category"));
            menuItems.Add(new MenuItem("PostCount", "$category.PostCount", "Number of published posts in this category", "Category"));
            menuItems.Add(new MenuItem("UniqueId", "$category.UniqueId", "Global unique id (guid)", "Category"));
            menuItems.Add(new MenuItem("ParentId", "$category.ParentId", "Parent category (defaults to 0)", "Category"));
            menuItems.Add(new MenuItem("IsUncategorized", "$category.IsUncategorized", "Returns true if this category is the special uncategorized category", "Category"));
            menuItems.Add(new MenuItem("Children", "$category.Children", "A list of child categories", "Category"));
            menuItems.Add(new MenuItem("HasChildren", "$category.HasChildren", "Returns true if this category has child categories", "Category"));
            menuItems.Add(new MenuItem("Url", "$category.Url", "The absolute url of this category", "Category"));

            return menuItems;
        }

        #endregion
    }

    public partial class CategoryController
    {

        public static void UpdatePostCounts()
        {
            if (!Util.IsAccess)
            {
                DataService.ExecuteNonQuery(
                    new QueryCommand(
                        "Update graffiti_Categories Set Post_Count = (Select " + DataService.Provider.SqlCountFunction() + " FROM graffiti_Posts where graffiti_Posts.CategoryId = graffiti_Categories.Id and graffiti_Posts.Status = 1 and graffiti_Posts.IsPublished = 1 and graffiti_Posts.IsDeleted = 0)"));
                DataService.ExecuteNonQuery(
                    new QueryCommand(
                        "Update graffiti_Categories Set Post_Count = (Select coalesce(sum(x.Post_Count), 0) FROM " + DataService.Provider.GenerateDerivedView("graffiti_Categories") + " AS x where x.ParentId = graffiti_Categories.Id) + Post_Count Where graffiti_Categories.ParentId <= 0"));
            }
            else
            {
                QueryCommand cmd1 = new QueryCommand("select count(*) AS PostCount, p.CategoryId from graffiti_Posts p where p.Status = 1 and p.IsPublished = @IsPublished and p.IsDeleted = 0 group by p.CategoryId");
				cmd1.Parameters.Add("IsPublished", true);

				using(IDataReader dr = DataService.ExecuteReader(cmd1))
				{
					while ( dr.Read() )
					{
						QueryCommand uCmd = new QueryCommand(string.Format("update graffiti_Categories set Post_Count = {0} where Id = {1}", dr["PostCount"], dr["CategoryId"]));
						DataService.ExecuteNonQuery(uCmd);
					}
				}

				QueryCommand cmd = new QueryCommand("Select sum(x.Post_Count) as ParentIdPostCount, x.ParentId FROM graffiti_Categories AS x where x.ParentId > 0 group by x.ParentId");
				using (IDataReader dr = DataService.ExecuteReader(cmd))
				{
					while (dr.Read())
					{
						QueryCommand uCmd = new QueryCommand(string.Format("update graffiti_Categories set Post_Count = Post_Count + {0} where Id = {1} and ParentId <= 0", dr["ParentIdPostCount"], dr["ParentId"]));
						DataService.ExecuteNonQuery(uCmd);
					}
				}
			}
        }

        public static readonly string CacheKey = "graffiti-categories";
        public static readonly string UncategorizedName = "Uncategorized";

        /// <summary>
        /// Returns all the parent categories and the UnCategorized Categry
        /// </summary>
        /// <returns></returns>
        public CategoryCollection GetAllTopLevelCachedCategories()
        {
            return GetTopLevelCachedCategories(false);
        }

        /// <summary>
        /// Returns the all the parent categories, but excludes the uncateogirzed category
        /// </summary>
        /// <returns></returns>
        public CategoryCollection GetTopLevelCachedCategories()
        {
            return GetTopLevelCachedCategories(true);
        }


        private CategoryCollection GetTopLevelCachedCategories(bool filterUncategorized)
        {
            CategoryCollection cc = new CategoryCollection();
            foreach(Category c in GetAllCachedCategories())
            {
                if(c.ParentId <= 0 )
                {
                    if (filterUncategorized && c.Name == UncategorizedName)
                        continue;

                    cc.Add(c);
                }
            }

            cc.Sort(delegate(Category c1, Category c2) { return Comparer<string>.Default.Compare(c1.Name, c2.Name); });

            return cc;
        }

        /// <summary>
        /// Returns all categories along with the uncategorized category
        /// </summary>
        /// <returns></returns>
        public CategoryCollection GetAllCachedCategories()
        {
            CategoryCollection cc = ZCache.Get<CategoryCollection>(CacheKey);
            if (cc == null)
            {
                cc = CategoryCollection.FetchAll();
                
                bool foundUncategorized = false;
                foreach (Category c in cc)
                {
                    if (c.Name == UncategorizedName)
                    {
                        foundUncategorized = true;
                        break;
                    }
                }

                if (!foundUncategorized)
                {
                    Category uncategorizedCategory = new Category();
                    uncategorizedCategory.Name = UncategorizedName;
                    uncategorizedCategory.LinkName = "uncategorized";
                    uncategorizedCategory.Save();
                    cc.Add(uncategorizedCategory);

                }

                ZCache.InsertCache(CacheKey, cc, 90);
            }
            return cc;

        }

        /// <summary>
        /// Returns all categories except the uncategorized category
        /// </summary>
        /// <returns></returns>
        public CategoryCollection GetCachedCategories()
        {
            CategoryCollection cc = GetAllCachedCategories();
            CategoryCollection cc2 = new CategoryCollection();
            foreach (Category c in cc)
                if (c.Name != UncategorizedName)
                    cc2.Add(c);


            cc2.Sort(delegate(Category c1, Category c2) { return Comparer<string>.Default.Compare(c1.Name, c2.Name); });


            return cc2;
        }

        /// <summary>
        /// Returns a single category.
        /// </summary>
        /// <param name="id">Id of the category to return</param>
        /// <param name="allowNull">determins if an exception is thrown if the category does not exist</param>
        /// <returns></returns>
        public Category GetCachedCategory(int id, bool allowNull)
        {
            CategoryCollection cc = GetAllCachedCategories();
            foreach (Category c in cc)
                if (c.Id == id)
                    return c;

            if (allowNull)
                return null;
            else
                throw new Exception("The category " + id + " could not be found");
        }

        /// <summary>
        /// Returns a single category.
        /// </summary>
        /// <param name="name">The name of the category to find. This is not case sensitive</param>
        /// <param name="allowNull">determins if an exception is thrown if the category does not exist</param>
        /// <returns></returns>
        public Category GetCachedCategory(string name, bool allowNull)
        {
            CategoryCollection cc = GetAllCachedCategories();
            foreach (Category c in cc)
                if (Util.AreEqualIgnoreCase(c.Name, name))
                    return c;

            if (allowNull)
                return null;
            else
                throw new Exception("The category " + name + " could not be found");
        }

        /// <summary>
        /// Returns a single category.
        /// </summary>
        /// <param name="name">The linkname of the category to find. This is not case sensitive</param>
        /// <param name="allowNull">determins if an exception is thrown if the category does not exist</param>
        /// <returns></returns>
        public Category GetCachedCategoryByLinkName(string linkName, bool allowNull)
        {
            CategoryCollection cc = GetAllCachedCategories();
            foreach (Category c in cc)
                if (Util.AreEqualIgnoreCase(c.LinkName, linkName))
                    return c;

            if (allowNull)
                return null;
            else
                throw new Exception("The category with link name " + linkName + " could not be found");
        }

		  /// <summary>
		  /// Returns a single category.
		  /// </summary>
		  /// <param name="name">The linkname of the category to find. This is not case sensitive</param>
		  /// <param name="allowNull">determins if an exception is thrown if the category does not exist</param>
		  /// <returns></returns>
		  public Category GetCachedCategoryByLinkName(string linkName, int ParentId, bool allowNull)
		  {
			  CategoryCollection cc = GetAllCachedCategories();
			  foreach (Category c in cc)
				  if (c.ParentId == ParentId && Util.AreEqualIgnoreCase(c.LinkName, linkName))
					  return c;

			  if (allowNull)
				  return null;
			  else
				  throw new Exception("The category with link name " + linkName + " could not be found");
		  }

        /// <summary>
        /// Returns the special uncategorized category (which does exist in the db)
        /// </summary>
        /// <returns></returns>
        public Category GetUnCategorizedCategory()
        {
            return GetCachedCategory(UncategorizedName, false);
        }

        /// <summary>
        /// Clears the category cache
        /// </summary>
        public static void Reset()
        {
            ZCache.RemoveCache(CacheKey);
        }

        private static int _uncatId = -1;

        /// <summary>
        /// Returns the Id of the Uncategorized Category. This value is cached staticly. 
        /// </summary>
        public static int UnCategorizedId
        {
            get
            {
                if (_uncatId == -1)
                {
                    _uncatId = new CategoryController().GetUnCategorizedCategory().Id;
                }

                return _uncatId;
            }
        }
    }

    public enum SortOrderType
    {
        Ascending = 1,
        Descending = 0,
        Views = 2,
        Custom = 3,
        Alphabetical = 4,

    }
}