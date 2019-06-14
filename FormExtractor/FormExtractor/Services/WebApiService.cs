using FormExtractor.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace FormExtractor.Services
{
    public class WebApiService
    {
        RestClient _client;
        string _baseUrl;

        public WebApiService()
        {
            _baseUrl = ConfigurationManager.AppSettings["Sage300WebApiUrl"];
            var user = ConfigurationManager.AppSettings["WebApiUser"];
            var pass = ConfigurationManager.AppSettings["WebApiPass"];
            _client = new RestClient(_baseUrl);
            _client.Authenticator = new HttpBasicAuthenticator(user, pass);
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

        public List<Vendor> GetVendors(string company)
        {
            var list = new List<Vendor>();

            var url = string.Format("{0}{1}/AP/APVendors", _baseUrl, company);
            var request = new RestRequest(url, Method.GET);

            var response = _client.Get<dynamic>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var data = (Dictionary<string, object>)response.Data;
                object sdata = null;
                data.TryGetValue("value", out sdata);

                var serializer = new JsonSerializer();
                var ldata = JsonConvert.DeserializeObject<List<SageVendor>>(sdata.ToString());

                foreach (var vendor in ldata)
                {
                    list.Add(MapVendor(vendor));
                }
            }

            return list;
        }
    }
}