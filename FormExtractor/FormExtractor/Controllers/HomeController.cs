using FormExtractor.Models;
using FormExtractor.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FormExtractor.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

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

            var currentUserId = User.Identity.GetUserId();
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(currentUserId);
            ViewBag.FormType = type;
            var vm = new ExtractViewModel(currentUser);
            vm.Vendors.Add(new Vendor() { Id = "1200", Name = "Chloride Systems" });
            vm.Vendors.Add(new Vendor() { Id = "1350", Name = "Excide Industrial Batteries" });
            vm.Vendors.Add(new Vendor() { Id = "1400", Name = "Coastal Heating of Ottawa" });

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> FileUpload(IEnumerable<HttpPostedFileBase> files)
        {
            if (files != null)
            {
                var tasks = new List<Task>();
                foreach (HttpPostedFileBase file in files)
                {
                    FileService.Upload(file);

                    tasks.Add(AzureService.PostFormRecognizer(file.FileName));
                }

                await Task.WhenAll(tasks);
            }

            return RedirectToAction("Extract", new { type = "Invoice" });
        }
    }
}