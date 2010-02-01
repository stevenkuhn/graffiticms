using System;
using System.Collections.Generic;
using System.Text;
using Telligent.Glow;
using System.IO;
using System.Web;

namespace Graffiti.Core
{
    public class UploadFileManager : IMultipleUploadFileManager
    {
        #region IMultipleUploadFileManager Members

        public void AddFile(UploadedFile file, string fileUploadContext)
        {
            string folder = GetUploadPath(fileUploadContext);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            FileStream fs = File.Create(Path.Combine(folder, file.FileName), file.ContentLength);
            byte[] buffer = new byte[(int)file.InputStream.Length];
            file.InputStream.Read(buffer, 0, (int)file.InputStream.Length);
            fs.Write(buffer, 0, buffer.Length);
            fs.Close();
        }

        public UploadedFile[] GetFiles(string fileUploadContext)
        {
            string folder = GetUploadPath(fileUploadContext);
            if (!Directory.Exists(folder))
                return new UploadedFile[0];

            DirectoryInfo root = new DirectoryInfo(folder);
            List<UploadedFile> files = new List<UploadedFile>();
            foreach (FileInfo fi in root.GetFiles("*.*", SearchOption.TopDirectoryOnly))
            {
                files.Add(new UploadedFile(fi.Name, (int)fi.Length, "", fileUploadContext, this.OpenStream));
            }

            return files.ToArray();
        }

        public void RemoveFiles(string fileUploadContext)
        {
            string folder = GetUploadPath(fileUploadContext);
            if (Directory.Exists(folder))
                Directory.Delete(folder, true);
        }

        #endregion

        public string GetUploadPath(string uploadContext)
        {
            HttpContext context = HttpContext.Current;
            return context.Request.MapPath(context.Response.ApplyAppPathModifier("~/files/uploads/" + uploadContext));
        }

        public Stream OpenStream(UploadedFile file)
        {
            return (new FileInfo(Path.Combine(GetUploadPath(file.UploadContext), file.FileName))).OpenRead();
        }

        #region static methods

        public static void RemoveUploadContextsOlderThan(string rootPath, int hours)
        {
            DateTime expiresBefore = DateTime.Now.AddHours(-hours);

            try
            {
                DirectoryInfo root = new DirectoryInfo(Path.Combine(rootPath, "files/uploads"));
                DirectoryInfo[] subFolders = root.GetDirectories();
               
                for (int i = subFolders.Length - 1; i >= 0; i--)
                {
                    if (((subFolders[i].Attributes & FileAttributes.Hidden) != FileAttributes.Hidden) && subFolders[i].CreationTime < expiresBefore)
                    {
                        try
                        {
                            subFolders[i].Delete(true);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("UploadFileManager", "An error occured while deleting an expired upload context.\n\nReason: {0}", ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("UploadFileManager", "An error occured while deleting an expired upload context.\n\nReason: {0}", ex.Message);
            }
        }

        #endregion
    }
}
