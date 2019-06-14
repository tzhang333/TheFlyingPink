using FormExtractor.Models;
using FormExtractor.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FormExtractor.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Extract(string type)
        {
            if(string.IsNullOrEmpty(type))
            {
                return RedirectToAction("Index");
            }

            ViewBag.FormType = type;
            var vm = new ExtractViewModel();
            vm.Vendors.Add(new Vendor() { Id = "1200", Name = "Chloride Systems" });
            vm.Vendors.Add(new Vendor() { Id = "1350", Name = "Excide Industrial Batteries" });
            vm.Vendors.Add(new Vendor() { Id = "1400", Name = "Coastal Heating of Ottawa" });

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> FileUpload(IEnumerable<HttpPostedFileBase> files)
        {
            var invoices = new List<AzureInvoice>();
            var successCount = 0;
            if (files != null)
            {
                var tasks = new List<Task<HttpResponseMessage>>();
                foreach (HttpPostedFileBase file in files)
                {
                    // upload files to server
                    FileService.Upload(file);

                    // upload files from server to Azure
                    tasks.Add(AzureService.PostFormRecognizer(file.FileName));
                }

                await Task.WhenAll(tasks);
                foreach(var task in tasks)
                {
                    var response = task.Result;
                    var success = (response.StatusCode == System.Net.HttpStatusCode.OK) ? true : false;
                    var JSON = await response.Content.ReadAsStringAsync();

                    var jo = JObject.Parse(JSON);
                    var invNo = jo["pages"][0]["keyValuePairs"][1]["value"][0]["text"];

                    var invoice = new AzureInvoice()
                    {
                        Id = invNo.ToString(),
                        Total = 0,
                        Success = success,
                        JSON = JSON,
                    };
                    invoices.Add(invoice);

                    if (success)
                    {
                        successCount++;
                    }
                }
            }

            return Json(new
            {
                invoices,
                redirectUrl = Url.Action("Extract", new { type = "Invoice" }),
                successCount,
            });
        }
    }
}