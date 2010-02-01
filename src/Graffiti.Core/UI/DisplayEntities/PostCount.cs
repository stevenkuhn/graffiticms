using System;
using System.Collections.Generic;
using System.Text;

namespace Graffiti.Core
{
    public class PostCount
    {
        private PostStatus _postStatus;
        private int _count;
        private int _categoryId;

        public PostStatus PostStatus
        {
            get { return _postStatus; }
            set { _postStatus = value; }
        }

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }
        
        public int CategoryId
        {
            get { return _categoryId; }
            set { _categoryId = value; }
        }
    }
}