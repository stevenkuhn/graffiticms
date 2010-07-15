using System.Collections.Generic;

namespace GraffitiClient.API
{
    public class Category
    {
        private SortOrderType _sortOrder = SortOrderType.Descending;
        private List<Category> _children = new List<Category>();

        /// <summary>
        /// The id of the category.
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Name of the category
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Used to build the link for a category. It is usually the cleaned up version of FormattedName. ReadOnly. 
        /// </summary>
        public string LinkName { get; set; }

        /// <summary>
        /// A description of the category. 
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Number of posts in this category. ReadOnly. 
        /// </summary>
        public int PostCount { get; internal set; }

        /// <summary>
        /// Url for the category.
        /// </summary>
        public string Url { get; internal set; }

        /// <summary>
        /// The Id for the category's parent. 
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// SortOrder of the categry. 
        /// </summary>
        public SortOrderType SortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; }
        }

        /// <summary>
        /// Child Categories. 
        /// </summary>
        public List<Category> Children
        {
            get { return _children; }
            internal set { _children = value; }
        }

        public bool HasChildren { get { return ParentId == 0 && Children.Count > 0; } }

        /// <summary>
        /// The meta description for this category.
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// The meta keywords for this category.
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// An optional feedburner Url
        /// </summary>
        public string FeedBurnerUrl { get; set; }
    }

    public enum SortOrderType
    {
        Ascending = 1,
        Descending = 0,
        Views = 2,
        Custom = 3,
        Alphabetical = 4
    }
}