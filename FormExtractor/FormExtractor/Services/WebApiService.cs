using FormExtractor.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace FormExtractor.Services
{
    public class WebApiService
    {
        RestClient _client;

        public WebApiService()
        {
            var url = ConfigurationManager.AppSettings["Sage300WebApiUrl"];
            _client = new RestClient(url);
        }

        private static Vendor MapVendor(SageVendor sageVendor)
        {
            return new Vendor()
            {
                Id = sageVendor.VendorNumber,
                Name = sageVendor.VendorName
            };
        }

        private static SageVendor MapSageVendor(Vendor vendor)
        {
            return new SageVendor()
            {
                VendorNumber = vendor.Id,
                VendorName = vendor.Name
            };
        }

        public async Task<List<Vendor>> GetVendors(string company)
        {
            var list = new List<Vendor>();
            var request = new RestRequest(string.Format("{0}/AP/APVendors", company), Method.GET);
            var response = await _client.ExecuteGetTaskAsync<List<SageVendor>>(request);

            if(response.StatusCode == HttpStatusCode.OK)
            {
                foreach(var data in response.Data)
                {
                    list.Add(MapVendor(data));
                }
            }

            return list;
        }
    }
}