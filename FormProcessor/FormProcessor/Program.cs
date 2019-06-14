using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCPAC.Advantage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FormProcessor
{
    class Program
    {
        public static ACCPAC.Advantage.Session AccpacSession;
        public static ACCPAC.Advantage.DBLink dblink, syslink;
        static void Main(string[] args)
        {
            OpenSession("SAMINC", "ADMIN", "ADMIN");
            //createAPBatch();
            // var data = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(@"ARDINVO5.json"));
            //string number = (string)data["Number"];
            //JsonTextReader reader = new JsonTextReader(new );
            var jo = JObject.Parse(File.ReadAllText(@"ARDINVO5.json"));
            var invNo = jo["pages"][0]["keyValuePairs"][1]["value"][0]["text"];
            List<String> amounts = new List<String>();
            var am = jo["pages"][0]["tables"][1]["columns"][5]["entries"].ToArray();
            foreach (var a in am)
            {
                var abc = a[0]["text"];
                if (abc != null)
                {
                    amounts.Add(abc.ToString());
                }
            }
            createAPBatch(invNo.ToString(), "1200", amounts);
        }

        public static int OpenSession(string sOrgID, string sUserID, string sPassword)
        {
            AccpacSession = new ACCPAC.Advantage.Session();
            try
            {
                AccpacSession.Init("", "XZ", "XZ1000", "66A");
                //DateTime dt = new DateTime(2009, 02, 12);
                DateTime dt = DateTime.Today;
                //AccpacSession.Open (sUserID, sPassword, sOrgID, System.DateTime.Now, 0);
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

        public static int createAPBatch(string invNo, string vendorNo, List<string> amounts)
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


            AP0021.Fields.FieldByName("IDINVC").SetValue(invNo, false);
            AP0021.Insert();

            AP0021.Fields.FieldByName("PROCESSCMD").SetValue(7, false);
            AP0021.Process();
            AP0020.Read(false);

            return 0;
        }

        public static View OpenView(string sViewID)
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

        public static void Error(string sText)
        {
            Console.WriteLine("Error:" + sText);
        }
    }
}
