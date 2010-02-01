using System;
using System.Collections.Generic;
using System.Text;

namespace Graffiti.Core
{
    public class AuthorCount
    {
        private int _id;
        private int _count;
        private string _name;
        private int _categoryId;

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        
        public int CategoryId
        {
            get { return _categoryId; }
            set { _categoryId = value; }
        }
    }
}
