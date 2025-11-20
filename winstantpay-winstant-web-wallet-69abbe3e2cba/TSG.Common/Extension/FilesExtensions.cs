using System;
using System.Collections.Generic;
using System.IO;

namespace WinstantPay.Common.Extension
{
    public static class FilesExtensions
    {
        public static IEnumerable<FileSystemInfo> AllFilesAndFolders(this DirectoryInfo dir)
        {
            foreach (var f in dir.GetFiles())
                yield return f;
            foreach (var d in dir.GetDirectories())
            {
                yield return d;
                foreach (var o in AllFilesAndFolders(d))
                    yield return o;
            }
        }

        #region Save file in file system

        public static bool SaveSystemFile(System.Web.HttpPostedFileBase file, string path, out string fileName, out string filePath)
        {
            try
            {
                fileName = String.Format("{0}{1}{2}", Path.GetFileNameWithoutExtension(file.FileName), Guid.NewGuid(), Path.GetExtension(file.FileName));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = Path.Combine(path, fileName);
                file.SaveAs(filePath);
                return true;
            }
            catch (Exception e)
            {
                fileName = filePath = String.Empty;
                return false;
            }
        }

        public static bool SaveSystemFile(System.Web.HttpPostedFileBase file, string infilename, string path, out string fileName, out string filePath)
        {
            try
            {
                fileName = String.Format("{0}{1}{2}", Path.GetFileNameWithoutExtension(infilename), Guid.NewGuid(), Path.GetExtension(file.FileName));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = Path.Combine(path, fileName);
                file.SaveAs(filePath);
                return true;
            }
            catch (Exception e)
            {
                fileName = filePath = String.Empty;
                return false;
            }
        }

        public static bool SaveSystemFile(string file, string filename, string path, out string fileName, out string filePath)
        {
            try
            {
                fileName = String.Format("{0}{1}{2}", Path.GetFileNameWithoutExtension(filename), Guid.NewGuid(), Path.GetExtension(filename));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = Path.Combine(path, fileName);
                Byte[] bytes = Convert.FromBase64String(file);
                File.WriteAllBytes(filePath, bytes);
                return true;
            }
            catch (Exception e)
            {
                fileName = filePath = String.Empty;
                return false;
            }
        }

        #endregion

    }


}