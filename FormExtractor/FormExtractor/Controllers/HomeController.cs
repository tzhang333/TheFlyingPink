using FormExtractor.Models;
using FormExtractor.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public ActionResult FileUpload(IEnumerable<HttpPostedFileBase> files)
        {
            if (files != null)
            {
                foreach (HttpPostedFileBase file in files)
                {
                    FileService.Upload(file);
                }
            }

            return RedirectToAction("Extract", new { type = "Invoice" });
        }
    }
}