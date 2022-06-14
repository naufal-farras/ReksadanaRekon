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

namespace ReksadanaRekon.Controllers.History
{
    public class HistoryRejectFundsController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        // GET: HistoryRejectFunds
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetIdApp(int id)
        {
            FundAplikasiDuaVM fundAplikasi = new FundAplikasiDuaVM();
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
            List<TrDataFund> result = new List<TrDataFund>();
            var match = new List<int> { 6, 11, 16 };
            if (startDate == null || endDate == null)
            {
                result = _context.TrDataFund
                    .Include("DataFund")
                    .Include("DataFund.Matching")
                    .Include("DataFund.Rekening")
                    .Where(x => x.CreateDate.Day == DateTime.Now.Day &&
                                x.CreateDate.Month == DateTime.Now.Month &&
                                x.CreateDate.Year == DateTime.Now.Year &&
                                match.Contains(x.Transaksi.MatchingId))
                    .OrderBy(x => x.Id).ToList();
            }
            else
            {
                DateTime start = startDate.Value;
                DateTime end = endDate.Value.AddDays(1);
                result = _context.TrDataFund
                    .Include("DataFund")
                    .Include("DataFund.Matching")
                    .Include("DataFund.Rekening")
                    .Where(x => x.CreateDate >= start &&
                                x.CreateDate <= end &&
                                match.Contains(x.Transaksi.MatchingId))
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
            var match = new List<int> { 6, 11 };

            var trans = _context.Transaksi.SingleOrDefault(x => x.Id == id && match.Contains(x.MatchingId));
            if (trans != null)
            {
                int matchid = 1;
                var trfund = _context.TrDataFund.Where(x => x.TransaksiId == id).ToList();
                foreach (var fund in trfund)
                {
                    var funds = _context.DataFund.SingleOrDefault(x => x.Id == fund.DataFundId);
                    funds.MatchingId = matchid;
                    funds.KeteranganUser = keterangan;
                    _context.Entry(funds).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                _context.TrDataFund.RemoveRange(trfund);
                _context.Transaksi.Remove(trans);

                _context.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Migrasi()
        {
            var match = new List<int> { 5, 6 };
            var result = _context.TrDataFund
                    .Where(x => match.Contains(x.Transaksi.MatchingId))
                    .OrderBy(x => x.Id).ToList();

            foreach (var data in result)
            {
                var trans = _context.Transaksi.SingleOrDefault(x => x.Id == data.TransaksiId && match.Contains(x.MatchingId));
                if (trans != null)
                {
                    int matchid = trans.MatchingId + 5;
                    trans.MatchingId = matchid;
                    _context.Entry(trans).State = EntityState.Modified;
                    _context.SaveChanges();

                    var funds = _context.DataFund.SingleOrDefault(x => x.Id == data.DataFundId);
                    funds.MatchingId = matchid;
                    _context.Entry(funds).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            return View();
        }
    }
}