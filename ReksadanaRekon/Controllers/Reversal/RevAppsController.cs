using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ReksadanaRekon.Models;
using ReksadanaRekon.Models.Trans;
using ReksadanaRekon.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ReksadanaRekon.Controllers.Reversal
{
    public class RevAppsController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        // GET: RevApps
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
            var result = _context.Transaksi.Include("Matching").Include("Inputer").SingleOrDefault(x => x.Id == id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetList(DateTime? startDate, DateTime? endDate)
        {
            var match = new List<int> { 7, 8, 9, 10, 11 };
            List<TrDataAplikasi> result = new List<TrDataAplikasi>();
            if (startDate == null || endDate == null)
            {
                result = _context.TrDataAplikasi
                    .Include("DataAplikasi")
                    .Include("DataAplikasi.Matching")
                    .Include("DataAplikasi.SA")
                    .Include("DataAplikasi.MI")
                    .Include("DataAplikasi.Fund")
                    .Where(x => match.Contains(x.Transaksi.MatchingId) &&
                                x.CreateDate.Day == DateTime.Now.Day &&
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
                    .Where(x => match.Contains(x.Transaksi.MatchingId) &&
                                x.CreateDate >= start &&
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
        public JsonResult Reversal(int id, string keterangan)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());
            bool result = false;
            var match = new List<int> { 7, 8, 9, 10, 11 };

            var trans = _context.Transaksi.SingleOrDefault(x => x.Id == id && match.Contains(x.MatchingId));
            if (trans != null)
            {
                int matchid = trans.MatchingId + 5;
                trans.MatchingId = matchid;
                trans.KeteranganInputer = keterangan;
                trans.CreateDate = DateTime.Now;
                trans.InputerId = currentUser.Id;
                _context.Entry(trans).State = EntityState.Modified;
                _context.SaveChanges();

                var trapp = _context.TrDataAplikasi.Where(x => x.TransaksiId == id).ToList();
                foreach (var app in trapp)
                {
                    var apps = _context.DataAplikasi.SingleOrDefault(x => x.Id == app.DataAplikasiId);
                    apps.MatchingId = matchid;
                    _context.Entry(apps).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                var trfund = _context.TrDataFund.Where(x => x.TransaksiId == id).ToList();
                foreach (var fund in trfund)
                {
                    var funds = _context.DataFund.SingleOrDefault(x => x.Id == fund.DataFundId);
                    funds.MatchingId = matchid;
                    _context.Entry(funds).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}