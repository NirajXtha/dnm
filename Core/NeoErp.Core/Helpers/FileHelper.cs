using NeoErp.Core.Integration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Helpers
{
    public class FileHelper
    {
        #region File Manipulation

        public static readonly string ReportFilePath = "~/App_Data/ReportFiles";

        public static string GetUploadFilePath(string FileName, string UploadPath, string UniquePrefix = "")
        {
            //string ext = Path.GetExtension(FileName);
            Random rd = new Random();
            string fileName = UniquePrefix + rd.Next(11111111, 999999999).ToString() + FileName;
            string directory = HttpContext.Current.Server.MapPath(UploadPath);
            string path = Path.Combine(directory, fileName);
            return path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <param name="UploadPath">virtual path for Folder</param>
        /// <returns></returns>
        public bool UploadFile(string fileName, HttpPostedFileBase file, string UploadPath)
        {
            //ensure Directory exist
            bool Status = CreateDirectory(UploadPath);

            file.SaveAs(fileName);
            return true;
        }

        public bool UploadFile(HttpRequestBase Request, string UploadPath, string FilePrefix = "")
        {
            try
            {
                bool Status =CreateDirectory(UploadPath);

                if (Request.Files != null)
                {
                    foreach (string requestFile in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[requestFile];
                        if (file.ContentLength > 0)
                        {
                            string path = GetUploadFilePath(file.FileName, UploadPath, FilePrefix);
                            file.SaveAs(path);
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public string[] GetAttachedFileList(HttpRequestBase Request)
        {
            try
            {
                List<string> FileList = new List<string>();
                if (Request.Files != null)
                {
                    foreach (string requestFile in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[requestFile];
                        if (file.ContentLength > 0)
                        {
                            //string path = Utilities.GetUploadFilePath(file.FileName, UploadPath, FilePrefix);
                            //file.SaveAs(path);
                            FileList.Add(file.FileName);
                        }
                    }
                    return FileList.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public bool CreateDirectory(string Path)
        {
            try
            {
                string directory = HttpContext.Current.Server.MapPath(Path);
                if (!Directory.Exists(directory))
                {
                    DirectoryInfo di = Directory.CreateDirectory(directory);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string ValidateFileName(string filename)
        {
            filename = (CryptorEngine.Encrypt(filename, true)).Replace("=", "").Replace("/", "").Replace("\\\\", "").Replace("+", "").Replace(" ", "_").Replace(".", "").Replace("^", "");
            return filename;
        }

        public static string GetFileExt(string fileName)
        {
            string ext = fileName.Substring(fileName.LastIndexOf('.') + 1);
            return ext;
        }

        public static string GetFileName(string fileFullPath)
        {
            string file = fileFullPath.Substring(fileFullPath.LastIndexOf('\\') + 1);
            if(file== fileFullPath) 
                file = fileFullPath.Substring(fileFullPath.LastIndexOf('/') + 1);
            return file;
        }

        public bool RemoveFile(string FileName)
        {
            try
            {
                if (System.IO.File.Exists(FileName))
                {
                    System.IO.File.Delete(FileName);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CopyFile(string Source, string Target,bool overwrite)
        {           
            try
            {
                File.Copy(Source, Target,overwrite);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SaveReportFile(string base64, string fileName, out string errorMessage)
        {
            if (string.IsNullOrEmpty(base64) || string.IsNullOrEmpty(fileName))
                throw new ArgumentException("either file or filename is not provided");

            var directoryPath = HttpContext.Current.Server.MapPath(ReportFilePath);

            if(!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            try
            {
                File.WriteAllBytes(string.Format("{0}/{1}", directoryPath, fileName), Convert.FromBase64String(base64));
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }

        #endregion
    }
}