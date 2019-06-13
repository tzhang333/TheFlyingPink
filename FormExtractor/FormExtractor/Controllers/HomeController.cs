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

        public ActionResult Extract(string type)
        {
            

            return View();
        }

        [HttpPost]
        public ActionResult FileUpload(IEnumerable<HttpPostedFileBase> files)
        {
            if (files != null)
            {
                foreach (HttpPostedFileBase file in files)
                {
                    var memStream = new MemoryStream();
                    file.InputStream.CopyTo(memStream);

                    byte[] fileData = memStream.ToArray();
                }
            }

            return RedirectToAction("Extract", new { type = "Invoice" });
        }
    }
}