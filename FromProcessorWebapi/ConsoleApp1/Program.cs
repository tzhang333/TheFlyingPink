using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            CallWebAPIAsync()
                .Wait();
            Console.WriteLine("Hello World!");

            var name = Console.ReadLine();
        }

        private static async Task CallWebAPIAsync()
        {
            var authValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"ADMIN:ADMIN")));
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = authValue;
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), "http://localhost/Sage300WebApi/V1.0/-/SAMLTD/AP/APVendors('1200')"))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    var response = await httpClient.SendAsync(request);
                    var rawMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(rawMessage);
                }
            }
        }
    }
}
