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
using ACCPAC.Advantage;

namespace FormExtractor.Controllers
{
    public class HomeController : Controller
    {
        private WebApiService _service;
        private ACCPAC.Advantage.Session AccpacSession;
        private ACCPAC.Advantage.DBLink dblink;

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
        public async Task<ActionResult> FileUpload(IEnumerable<HttpPostedFileBase> files, string vendorId)
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

                    /// Insert into Sage 300
                    OpenSession("SAMINC", "ADMIN", "ADMIN");
                    var apInvsrc = ParseForminJSON(JSON);
                    createAPBatch(apInvsrc.invoiceNo, vendorId, apInvsrc.amounts);

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
        private APInvSrc ParseForminJSON(String JSON)
        {
            APInvSrc apInv = new APInvSrc();
            var jo = JObject.Parse(JSON);
            apInv.invoiceNo = jo["pages"][0]["keyValuePairs"][1]["value"][0]["text"].ToString();
            List<String> amounts = new List<String>();
            var am = jo["pages"][0]["keyValuePairs"][11]["value"];
            //var am = jo["pages"][0]["tables"][1]["columns"][5]["entries"].ToArray();
            foreach (var a in am)
            {
                var abc = a["text"];
                if (abc != null)
                {
                    amounts.Add(abc.ToString());
                }
            }
            apInv.amounts = amounts;

            return apInv;
        }
        private int OpenSession(string sOrgID, string sUserID, string sPassword)
        {
            AccpacSession = new ACCPAC.Advantage.Session();
            try
            {
                AccpacSession.Init("", "XZ", "XZ1000", "66A");
                DateTime dt = DateTime.Today;
                AccpacSession.Open(sUserID, sPassword, sOrgID, dt, 0);
            }
            catch (Exception ex)
            {
                Error("Exception:" + ex.ToString());
                return 1;
            }

            dblink = AccpacSession.OpenDBLink(DBLinkType.Company, DBLinkFlags.ReadWrite);
            return 0;
        }
        private int createAPBatch(string invNo, string vendorNo, List<string> amounts)
        {
            try
            {
                View AP0020 = OpenView("AP0020"); //Invoice Batches
                View AP0021 = OpenView("AP0021"); //Invoice
                View AP0022 = OpenView("AP0022"); //Invoice Details
                View AP0023 = OpenView("AP0023"); //Payment Schedule
                View AP0402 = OpenView("AP0402"); //Invoice Optional Fields
                View AP0401 = OpenView("AP0401"); //Invoice Detail Optional field

                View[] v1 = new View[1];
                v1[0] = AP0021;
                AP0020.Compose(v1);

                View[] v2 = new View[4];
                v2[0] = AP0020;
                v2[1] = AP0022;
                v2[2] = AP0023;
                v2[3] = AP0402;
                AP0021.Compose(v2);

                View[] v3 = new View[3];
                v3[0] = AP0021;
                v3[1] = AP0020;
                v3[2] = AP0401;
                AP0022.Compose(v3);

                View[] v4 = new View[1];
                v4[0] = AP0021;
                AP0023.Compose(v4);

                View[] v5 = new View[1];
                v5[0] = AP0021;
                AP0402.Compose(v5);

                View[] v6 = new View[1];
                v6[0] = AP0022;
                AP0401.Compose(v6);

                AP0020.RecordCreate(ViewRecordCreate.Insert);
                AP0020.Fields.FieldByName("PROCESSCMD").SetValue(1, false);
                AP0020.Process();

                AP0020.Read(false);
                AP0021.RecordCreate(ViewRecordCreate.NoInsert);
                AP0022.Cancel();
                AP0021.Fields.FieldByName("IDVEND").SetValue(vendorNo, false);
                AP0021.Fields.FieldByName("PROCESSCMD").SetValue(7, false);
                AP0021.Process();

                AP0021.Fields.FieldByName("PROCESSCMD").SetValue(7, false);
                AP0021.Process();

                AP0021.Fields.FieldByName("PROCESSCMD").SetValue(4, false);
                AP0021.Process();

                AP0022.Fields.FieldByName("CNTLINE").SetValue(-1, false);
                AP0022.Read(false);
                AP0022.Delete();
                //AP0022.Fields.FieldByName("AMTDIST").SetValue(777.55, false);
                //AP0022.Update();

                //AP0022.Fields.FieldByName("CNTLINE").SetValue(-1, false);
                //AP0022.Read(false);
                foreach (var amount in amounts)
                {
                    AP0022.RecordCreate(ViewRecordCreate.DelayKey);
                    AP0022.Fields.FieldByName("PROCESSCMD").SetValue(0, false);
                    AP0022.Process();
                    AP0022.Fields.FieldByName("IDDIST").SetValue("AMEX", false);
                    AP0022.Fields.FieldByName("AMTDIST").SetValue(amount, false);
                    AP0022.Insert();
                }

                Decimal distAmount = (Decimal) AP0021.Fields.FieldByName("AMTUNDISTR").Value;
                AP0021.Fields.FieldByName("AMTGROSTOT").SetValue(Decimal.Negate(distAmount), false);
                AP0021.Fields.FieldByName("IDINVC").SetValue(invNo, false);
                AP0021.Insert();

                AP0021.Fields.FieldByName("PROCESSCMD").SetValue(7, false);
                AP0021.Process();
                AP0020.Read(false);
            }
            catch (Exception e)
            {

            }

            return 0;
        }

        private View OpenView(string sViewID)
        {
            ACCPAC.Advantage.View v;
            try
            {
                v = dblink.OpenView(sViewID);
            }
            catch (Exception ex)
            {
                Error("Exception:" + ex.ToString());
                return null;
            }
            if (v == null) return null;
            return v;
        }

        private void Error(string sText)
        {
            Console.WriteLine("Error:" + sText);
        }
    }

    class APInvSrc
    {
        public String invoiceNo { get; set; }
        public List<String> amounts { get; set; }
    }
}