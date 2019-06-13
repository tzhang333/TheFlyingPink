using System;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Web;
using System.IO;

namespace CSHttpClientSample
{
    static class Program
    {
        static void Main()
        {
            string pathToFile = "C:\\2019Hackthon\\ARDINVO1.pdf"; // the path to the file that you request for
            string keys = "Amount"; // the key you request for

            MakeRequest(pathToFile, keys);
            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }

        static async void MakeRequest(string pathToFile, string keys)
        {
            
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://westus2.api.cognitive.microsoft.com/formrecognizer/v1.0-preview/custom/models/499eb71c-7344-4f46-9b47-880d0df9436f/analyze?keys=" + keys))
                {
                    request.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", "034ef1f3835d4b4c9ac4e8d9f982092e");

                    var multipartContent = new MultipartFormDataContent();
                    var file1 = new ByteArrayContent(File.ReadAllBytes(pathToFile));
                    file1.Headers.Add("Content-Type", "application/pdf");
                    multipartContent.Add(file1, "form", Path.GetFileName(pathToFile));
                    request.Content = multipartContent;

                    var response = await httpClient.SendAsync(request);
                    var contents = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(contents);
                }
            }
        }
    }
}