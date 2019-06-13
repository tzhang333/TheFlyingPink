using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace FormExtractor.Services
{
    public class AzureService
    {
        public static async Task<HttpResponseMessage> PostFormRecognizer(string fileName)
        {
            var url = ConfigurationManager.AppSettings["FormRecognizerUrl"];
            var relativePath = ConfigurationManager.AppSettings["UploadedFilePath"];
            var pathToFile = string.Format("{0}/{1}", HttpContext.Current.Server.MapPath(relativePath), fileName);
            var mimeType = MimeMapping.GetMimeMapping(fileName);

            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), url))
                {
                    request.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["FormRecognizerKey"]);

                    var multipartContent = new MultipartFormDataContent();
                    var file1 = new ByteArrayContent(File.ReadAllBytes(pathToFile));
                    file1.Headers.Add("Content-Type", mimeType);
                    multipartContent.Add(file1, "form", Path.GetFileName(pathToFile));
                    request.Content = multipartContent;

                    var response = await httpClient.SendAsync(request);

                    return response;
                }
            }
        }
    }
}