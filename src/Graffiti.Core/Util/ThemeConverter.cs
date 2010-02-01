using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Graffiti.Core
{
    public static class ThemeConverter
    {
        public static void ToDisk(string xml, string basePath, bool overwrite)
        {
            ToDisk(xml, basePath, overwrite, null);
        }

        public static string ToDisk(string xml, string basePath, bool overwrite, string overrideThemeName)
        {   
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNode node = doc.SelectSingleNode("/theme/folder");

            if (!String.IsNullOrEmpty(overrideThemeName))
            {
                node.Attributes["name"].Value = overrideThemeName;
            }

            string name = node.Attributes["name"].Value;

            ProcessFolderNode(node, basePath, overwrite);

            return name;
        }

        private static void ProcessFolderNode(XmlNode node, string basePath, bool overwrite)
        {
            string folderName = node.Attributes["name"].Value;
            string newPath = Path.Combine(basePath, folderName);
            DirectoryInfo di = new DirectoryInfo(newPath);
            if (!di.Exists)
            {
                di.Create();
            }
            else if (!overwrite)
                throw new Exception("Directory " + newPath + " already exists");

            foreach(XmlNode fileNode in node.SelectNodes("files/file"))
            {
                string fileToCreateName = Path.Combine(newPath, fileNode.Attributes["name"].Value);
                if (!overwrite && File.Exists(fileToCreateName))
                    throw new Exception("The file " + fileToCreateName + " already exists");

                bool isText = fileNode.Attributes["type"] != null ? fileNode.Attributes["type"].Value == "text" : false;
                if(isText)
                {
                    using (StreamWriter sw = new StreamWriter(fileToCreateName))
                    {
                        sw.Write(fileNode.InnerText);
                        sw.Close();
                    }
                }
                else
                {
                    string base64 = fileNode.InnerText;
                    byte[] ba = Convert.FromBase64String(base64);
                    using(FileStream fs = new FileStream(fileToCreateName,FileMode.Create,FileAccess.Write))
                    {
                        fs.Write(ba,0,ba.Length);
                        fs.Close();
                    }
                }

                VersionStore.VersionFile(new FileInfo((fileToCreateName)));
            }

            foreach(XmlNode folderNode in node.SelectNodes("folders/folder"))
            {
                ProcessFolderNode(folderNode,newPath,overwrite);
            }
        }

        public static string ToXML(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(sw);
            writer.Formatting = Formatting.Indented;
            writer.WriteRaw("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            writer.WriteStartElement("theme");
            
            DirectoryToXML(di,writer);
            
            writer.WriteEndElement();

            return sb.ToString();
        }

        private static void DirectoryToXML(DirectoryInfo di, XmlTextWriter writer)
        {
            writer.WriteStartElement("folder");
            writer.WriteAttributeString("name", di.Name);

            FileInfo[] files = di.GetFiles();
            if (files != null && files.Length > 0)
            {
                writer.WriteStartElement("files");

                foreach (FileInfo fi in files)
                {
                    writer.WriteStartElement("file");
                    writer.WriteAttributeString("name", fi.Name);

                    //switch (fi.Extension.ToLower())
                    //{
                    //    case ".txt":
                    //    case ".js":
                    //    case ".css":
                    //    case ".view":
                    //    case ".xml":
                    //    case ".htm":
                    //    case ".html":
                    //    case ".config":

                    //        writer.WriteAttributeString("type", "text");

                    //        using (StreamReader sr = new StreamReader(fi.FullName))
                    //        {
                    //            writer.WriteCData(sr.ReadToEnd());
                    //            sr.Close();
                    //        }


                    //        break;

                    //    case ".jpg":
                    //    case ".jpeg":
                    //    case ".gif":
                    //    case ".png":
                    //    case ".zip":
                    //    case ".bmp":

                            writer.WriteAttributeString("type", "base64");

                            using (FileStream fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read))
                            {
                                byte[] ba = new byte[fs.Length];
                                fs.Read(ba, 0, ba.Length);
                                fs.Close();

                                string base64 = Convert.ToBase64String(ba);
                                writer.WriteCData(base64);
                            }


                    //        break;
                    //}

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            DirectoryInfo[] children = di.GetDirectories();

            if (children != null && children.Length > 0)
            {
                writer.WriteStartElement("folders");
                foreach (DirectoryInfo diChild in children)
                {
                    if (!diChild.Name.StartsWith(".") && !diChild.Name.StartsWith("_svn"))
                    {
                        DirectoryToXML(diChild, writer);
                    }
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }


    }
}