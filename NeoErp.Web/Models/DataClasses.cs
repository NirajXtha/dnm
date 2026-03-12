using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.Hosting;

namespace NeoErp.Models
{
    public class DataClasses
    {
        public List<FileNames> GetFiles()
        {
            List<FileNames> lstFiles = new List<FileNames>();
            DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/Log"));
            DirectoryInfo subFolder = new DirectoryInfo(HostingEnvironment.MapPath("~/Log/Api"));

            int i = 0;
            foreach(var item in dirInfo.GetFiles())
            {
                lstFiles.Add(new FileNames() { FileId = i + 1, FileName = item.Name, FilePath = dirInfo.FullName + @"\" + item.Name ,CreatedDate = item.LastWriteTime,FolderName=dirInfo.Name.ToUpper()});
                i = i + 1;
            }

            foreach(var item in subFolder.GetFiles())
            {
                lstFiles.Add(new FileNames() { FileId = i + 1, FileName = item.Name, FilePath = subFolder.FullName + @"\" + item.Name,CreatedDate = item.LastWriteTime, FolderName=dirInfo.Name.ToUpper()+"/"+subFolder.Name.ToUpper() });
                i = i + 1;
            }

            return lstFiles;
        }

        public List<FileNames> GetLogFiles()
        {
            List<FileNames> fileList = new List<FileNames>();
            DirectoryInfo directoryInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/Log4NetLog"));

            int i = 0;
            foreach(var file in directoryInfo.GetFiles())
            {
                var dateString = file.Name.Split('_')[1].Split('.')[0].ToString();
                // CultureInfo provider = new CultureInfo("en-US");
                DateTime dt;
                DateTime.TryParseExact(dateString.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
                fileList.Add(new FileNames() { FileId = i + 1, FileName = file.Name, FilePath = directoryInfo.FullName + @"\" + file.Name,CreatedDate=dt});
                i++;
            }

            return fileList;
        }

    }

    public class FileNames
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }    
        public DateTime CreatedDate { get; set; }
        public string FolderName { get; set; }
    }
}