using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace FormExtractor.Services
{
    public class FileService
    {
        public static void Upload(HttpPostedFileBase file)
        {
            var relativePath = ConfigurationManager.AppSettings["UploadedFilePath"];
            var path = HttpContext.Current.Server.MapPath(relativePath);

            Directory.CreateDirectory(path);

            file.SaveAs(string.Format("{0}/{1}" , path, file.FileName));
        }
    }
}