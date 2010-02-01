using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using System.Web;
using System.IO;

namespace Graffiti.Core
{
    /// <summary>
    /// Manages packages in Graffiti. This object is stored in the ObjectStore.
    /// </summary>
    [Serializable]
    public class PackageSettings
    {
        private PackageCollection _packages;

        public PackageCollection Packages
        {
            get { return _packages; }
            set { _packages = value; }
        }

        public void Save()
        {
            ObjectManager.Save(this, "PackageSettings");
        }

        public static PackageSettings Get()
        {
            return ObjectManager.Get<PackageSettings>("PackageSettings");
        }

        public static bool RemovePackage(string packageName)
        {
            PackageSettings pSettings = PackageSettings.Get();

            Package temp = pSettings.Packages.Find(
                                        delegate(Package p)
                                        {
                                            return p.Name == packageName;
                                        });

            if (temp != null)
            {
                RemoveFilesAndFolders(temp);
                pSettings.Packages.Remove(temp);
                pSettings.Save();
                return true;
            }

            return false;        
        }

        public static void ToDisk(string xml, bool overwrite)
        {
            ToDisk(xml, overwrite, null);
        }

        public static string ToDisk(string xml, bool overwrite, string overrideThemeName)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNode node = doc.SelectSingleNode("/package");

            if (!String.IsNullOrEmpty(overrideThemeName))
            {
                node.Attributes["name"].Value = overrideThemeName;
            }

            string name = node.Attributes["name"].Value;
            string unpackTo = node.Attributes["location"].Value;
            string version = null;

            if (node.Attributes["version"] != null)
                version = node.Attributes["version"].Value;

            if (!unpackTo.StartsWith("/"))
                unpackTo = "/" + unpackTo;

            PackageSettings pkgSettings = PackageSettings.Get();

            if (pkgSettings.Packages == null)
                pkgSettings.Packages = new PackageCollection();

            // check for duplicates
            Package temp = pkgSettings.Packages.Find(
                                            delegate(Package p)
                                            {
                                                return p.Name == name;
                                            });

            if (temp != null)
                throw new Exception("A package with this name already exist.");

            Package pkg = new Package();
            pkg.Name = name;

            if (version != null)
                pkg.Version = version;

            pkg.DateInstalled = DateTime.Today;

            ProcessFolderNode(node, unpackTo, overwrite, pkg, true);

            pkgSettings.Packages.Add(pkg);

            pkgSettings.Save();

            return name;
        }

        /// <summary>
        /// Checks if the current file is ok to install
        /// </summary>
        /// <param name="file"></param>
        /// <returns>The package that is using the file if it cannot be installed, null if it can be installed</returns>
        private static Package IsFileOkToInstall(string file)
        {
            PackageCollection packages = PackageSettings.Get().Packages;

            if(packages != null)
            {
                foreach(Package p in packages)
                {
                    if(p.Files != null)
                    {
                        string temp = p.Files.Find(
                                        delegate(string s)
                                        {
                                            return s == file;
                                        });

                        if (!String.IsNullOrEmpty(temp))
                            return p;
                    }
                }
            }

            return null;
        }

        private static void RemoveFilesAndFolders(Package p)
        {
            if(p.Files != null)
            {
                foreach(string file in p.Files)
                {
                    string fileName = HttpContext.Current.Server.MapPath(VirtualPathUtility.ToAbsolute("~" + file));

                    if(File.Exists(fileName))
                        File.Delete(fileName);

                    if (File.Exists(fileName + ".old"))
                        File.Move(fileName + ".old", fileName);
                    else
                    {
                        VersionStoreCollection vsc = VersionStore.GetVersionHistory(fileName, false);

                        if (vsc != null && vsc.Count > 0)
                        {
                            WriteFile(fileName, vsc[0].Data);

                            VersionStore.Destroy(VersionStore.Columns.UniqueId, vsc[0].UniqueId);
                        }
                    }
                }
            }

            if (p.Directories != null)
            {
                foreach (string dir in p.Directories)
                {
                    if(Directory.Exists(dir) && Directory.GetFiles(dir).Length == 0)
                        Directory.Delete(dir);
                }
            }
        }

        private static void WriteFile(string fileName, string data)
        {
            try
            {
                string base64 = data;
                byte[] ba = Convert.FromBase64String(base64);
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(ba, 0, ba.Length);
                    fs.Close();
                }
            }
            catch (Exception)
            {
                try
                {
                    // try to write the file as text
                    using (TextWriter tw = new StreamWriter(fileName))
                    {
                        tw.Write(data);
                        tw.Close();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Could not write file " + fileName + ".");
                }
            }
        }

        private static void ProcessFolderNode(XmlNode node, string basePath, bool overwrite, Package pkg, bool root)
        {
            string unpackTo = HttpContext.Current.Server.MapPath(VirtualPathUtility.ToAbsolute("~" + basePath));

            if (!root && node.Attributes["name"] != null)
            {
                string folderName = node.Attributes["name"].Value;

                basePath = Path.Combine(basePath, folderName);
                unpackTo = Path.Combine(unpackTo, folderName);
            }

            DirectoryInfo di = new DirectoryInfo(unpackTo);

            if (!di.Exists)
            {
                di.Create();

                if (pkg.Directories == null)
                    pkg.Directories = new List<String>();

                pkg.Directories.Add(unpackTo);
            }

            foreach (XmlNode fileNode in node.SelectNodes("files/file"))
            {
                string fileToCreateName = Path.Combine(unpackTo, fileNode.Attributes["name"].Value);

                string pkgFileName = basePath.Replace("\\", "/") + "/" + fileNode.Attributes["name"].Value;
                pkgFileName = pkgFileName.Replace("//", "/");

                Package p = IsFileOkToInstall(pkgFileName);
                if (p != null)
                {
                    RemoveFilesAndFolders(pkg);
                    throw new Exception("Cannot install this package because the file " + fileNode.Attributes["name"].Value + " is in use by the package <strong>" + p.Name + "</strong>.");
                }

                if (!overwrite && File.Exists(fileToCreateName))
                {
                    RemoveFilesAndFolders(pkg);
                    throw new Exception("The file " + fileNode.Attributes["name"].Value + " already exists.");
                }

                if (overwrite && File.Exists(fileToCreateName))
                {
                    // if the file is a .dll, rename it. otherwise version it.
                    if (fileToCreateName.ToLower().EndsWith(".dll"))
                        File.Move(fileToCreateName, fileToCreateName + ".old");
                    else
                        VersionStore.VersionFile(new FileInfo(fileToCreateName));

                    File.Delete(fileToCreateName);
                }

                if (pkg.Files == null)
                    pkg.Files = new List<String>();

                pkg.Files.Add(pkgFileName);

                try
                {
                    WriteFile(fileToCreateName, fileNode.InnerText);
                }
                catch (Exception exc)
                {
                    throw new Exception("Could not create file " + fileNode.Attributes["name"].Value + ". " + exc.Message);
                }
            }

            foreach (XmlNode folderNode in node.SelectNodes("folders/folder"))
            {
                ProcessFolderNode(folderNode, basePath, overwrite, pkg, false);
            }
        }
    }
}