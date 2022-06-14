using Microsoft.AspNet.Identity;
using ReksadanaRekon.Models;
using ReksadanaRekon.Models.Trans;
using ReksadanaRekon.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ReksadanaRekon.Controllers.History
{
    public class HistoryAppsController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        // GET: HistoryApps
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetIdApp(int id)
        {
            FundAplikasiDuaVM fundAplikasi = new FundAplikasiDuaVM();
            fundAplikasi.allDataAplikasi = _context.TrDataAplikasi
                    .Include("DataAplikasi")
                    .Include("DataAplikasi.Matching")
                    .Include("DataAplikasi.SA")
                    .Include("DataAplikasi.MI")
                    .Include("DataAplikasi.Fund")
                    .Where(x => x.TransaksiId == id).ToList();

            fundAplikasi.allDataFund = _context.TrDataFund
                .Include("DataFund")
                .Include("DataFund.Matching")
                .Include("DataFund.Rekening")
                .Where(x => x.TransaksiId == id).ToList();
            return Json(fundAplikasi, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetIdAppDet(int id)
        {
            var result = _context.Transaksi.Include("Matching").Include("Inputer").Include("Approver").SingleOrDefault(x => x.Id == id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetList(DateTime? startDate, DateTime? endDate)
        {
            List<TrDataAplikasi> result = new List<TrDataAplikasi>();
            if (startDate == null || endDate == null)
            {
                result = _context.TrDataAplikasi
                    .Include("DataAplikasi")
                    .Include("DataAplikasi.Matching")
                    .Include("DataAplikasi.SA")
                    .Include("DataAplikasi.MI")
                    .Include("DataAplikasi.Fund")
                    .Where(x => x.CreateDate.Day == DateTime.Now.Day &&
                                x.CreateDate.Month == DateTime.Now.Month &&
                                x.CreateDate.Year == DateTime.Now.Year)
                    .OrderBy(x => x.Id).ToList();
            }
            else
            {
                DateTime start = startDate.Value;
                DateTime end = endDate.Value.AddDays(1);
                result = _context.TrDataAplikasi
                    .Include("DataAplikasi")
                    .Include("DataAplikasi.Matching")
                    .Include("DataAplikasi.SA")
                    .Include("DataAplikasi.MI")
                    .Include("DataAplikasi.Fund")
                    .Where(x => x.CreateDate >= start &&
                                x.CreateDate <= end)
                    .OrderBy(x => x.Id).ToList();
            }



            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var results = new ContentResult
            {
                Content = serializer.Serialize(result),
                ContentType = "application/json"
            };
            return results;
        }
    }
}