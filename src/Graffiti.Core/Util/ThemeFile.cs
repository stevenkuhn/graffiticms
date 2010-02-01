using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Graffiti.Core
{
    [Serializable]
    public class ThemeFile
    {


        private Theme _theme = null;
        private string _fileName = string.Empty;
        private string _fullFilePath = string.Empty;
        private string _fileContents = string.Empty;


        public ThemeFile(Theme theme, string fullFilePath)
        {
            _theme = theme;
            _fullFilePath = fullFilePath;

            // Now, get just the file name
            string[] fileParts = fullFilePath.Split(Path.DirectorySeparatorChar);
            _fileName = fileParts[fileParts.Length - 1];

            // Load the contents up
            LoadContents();
        }

        public Theme Theme
        {
            get { return _theme; }
            set { _theme = value; }
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public string FullFilePath
        {
            get { return _fullFilePath; }
            set { _fullFilePath = value; }
        }

        public string FileContents
        {
            get { return _fileContents; }
            set { _fullFilePath = value; }
        }

        private void LoadContents()
        {
            StringBuilder contents = new StringBuilder();
            string[] fileContents = File.ReadAllLines(_fullFilePath);


            foreach (string line in fileContents)
            {
                contents.Append(line + "\n");
            }

            _fileContents = contents.ToString();
        }

    }
}
