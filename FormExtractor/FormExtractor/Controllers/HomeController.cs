using FormExtractor.Models;
using FormExtractor.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FormExtractor.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebGrease.Css.Extensions;

namespace FormExtractor.Controllers
{
    public class HomeController : Controller
    {
        private WebApiService _service;

        public HomeController()
        {
            _service = new WebApiService();
        }

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

            var currentUserId = User.Identity.GetUserId();
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(currentUserId);
            ViewBag.FormType = type;
            var vm = new ExtractViewModel(currentUser);

            if (currentUser.UserName == "ADMIN")
            {
                var allVendors = _service.GetVendors("SAMINC");

                var dbContext = new ApplicationDbContext();
                var users = dbContext.Users.Where(x => x.UserName != "ADMIN");
                List<Vendor> vendorList = new List<Vendor>();
                foreach (var vendor in users)
                {
                    var vendorId = manager.FindById(vendor.Id).ApplicationUserInfo.VendorNumber;
                    var vendorName = allVendors.Where(x => x.Id == vendorId).Select(x => x.Name).FirstOrDefault();
                    vendorList.Add(new Vendor{Id = vendorId, Name = vendorName });
                }
                vm.Vendors = vendorList;
            }

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