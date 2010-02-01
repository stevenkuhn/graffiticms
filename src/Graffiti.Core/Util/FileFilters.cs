using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Graffiti.Core
{
    public static class FileFilters
    {
        private static Regex excludedFileRegex = null;
        private static Regex editableFileRegex = null;
        private static Regex versionableFileRegex = null;
        private static Regex isDownloadableRegex = null;
        private static Regex isLinkableRegex = null;
        private static Regex isDeleteable = null;

        private static Regex CreateRegex(string key)
        {
            string pattern = "(" + string.Join("|", ConfigurationManager.AppSettings[key].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)) + ")$";
            return new Regex(pattern, RegexOptions.IgnoreCase);
        }

        public static bool IsValidFile(string name)
        {
            if (excludedFileRegex == null)
                excludedFileRegex = CreateRegex("Graffiti:FileBrowser:Excluded");

            return !excludedFileRegex.IsMatch(name);
        }

        public static bool IsEditable(string name)
        {
            if (editableFileRegex == null)
                editableFileRegex = CreateRegex("Graffiti:FileBrowser:Editable");

            return editableFileRegex.IsMatch(name);
        }

        public static bool IsVersionable(string name)
        {
            if (versionableFileRegex == null)
                versionableFileRegex = CreateRegex("Graffiti:FileBrowser:Versionable");

            return versionableFileRegex.IsMatch(name);
        }

        public static bool IsLinkable(string name)
        {
            if (isLinkableRegex == null)
                isLinkableRegex = CreateRegex("Graffiti:FileBrowser:NoLink");

            return !isLinkableRegex.IsMatch(name);
        }

        public static bool IsDownloadable(string name)
        {
            if (isDownloadableRegex == null)
                isDownloadableRegex = CreateRegex("Graffiti:FileBrowser:NoDownload");

            return !isDownloadableRegex.IsMatch(name);
        }

        public static bool IsDeletable(string name)
        {
            if (isDeleteable == null)
                isDeleteable = CreateRegex("Graffiti:FileBrowser:NoDelete");

            return !isDeleteable.IsMatch(name);
        }
    }
}
