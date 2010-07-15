using System.Collections.Generic;

namespace GraffitiClient.API
{
    public class PagedList<T> : List<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
    }
}