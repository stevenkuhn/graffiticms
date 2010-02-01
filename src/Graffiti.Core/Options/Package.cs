using System;
using System.Collections.Generic;

namespace Graffiti.Core
{
    /// <summary>
    /// Graffiti Package (managed by PackageSettings).
    /// </summary>
    [Serializable]
    public class Package
    {
        private string _name;
        private string _description;
        private List<String> _files;
        private List<String> _directories;
        private string _version;
        private DateTime _dateInstalled;

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

        public DateTime DateInstalled
        {
            get { return _dateInstalled; }
            set { _dateInstalled = value; }
        }

        public List<String> Files
        {
            get { return _files; }
            set { _files = value; }
        }

        public List<String> Directories
        {
            get { return _directories; }
            set { _directories = value; }
        }
    }

    [Serializable]
    public class PackageCollection : List<Package>
    {
    }
}