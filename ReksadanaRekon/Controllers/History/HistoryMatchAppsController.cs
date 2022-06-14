using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ReksadanaRekon.Models;
using ReksadanaRekon.Models.Trans;
using ReksadanaRekon.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ReksadanaRekon.Controllers.History
{
    public class HistoryMatchAppsController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        // GET: HistoryMatchApps
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetList(AplikasiSearchVM Data)
        {
            //jQuery DataTables Param
            var draw = Request.Form.GetValues("draw").FirstOrDefault();

            //Find paging info
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();

            //Find order columns info
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            //find search columns info
            var search = Request.Form.GetValues("search[value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            int recordsTotal = 0;
            
            var match = new List<int> { 2, 3, 4, 5, 7, 8, 9, 10, 12, 13, 14, 15 };
            var reject = new List<int> { 6, 11, 16 };

            //_context.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
            IQueryable<HistoryAppVM> v = null;

            if (Data == null)
            {
                v = (from a in _context.TrDataAplikasi
                 .Include("DataAplikasi").Include("DataAplikasi.Matching").Include("DataAplikasi.SA").Include("DataAplikasi.MI").Include("DataAplikasi.Fund")
                .Where(x => x.Transaksi.Retur == false && 
                        match.Contains(x.Transaksi.MatchingId) && 
                        x.DataAplikasi.UpdateDate.Value.Year == DateTime.Now.Year &&
                        x.DataAplikasi.UpdateDate.Value.Month == DateTime.Now.Month &&
                        x.DataAplikasi.UpdateDate.Value.Day == DateTime.Now.Day)
                     select new HistoryAppVM { TransaksiId = a.TransaksiId, TanggalUpload = a.DataAplikasi.CreateDate, TanggalMatch = a.DataAplikasi.UpdateDate, TanggalTransaksi = a.DataAplikasi.TransactionDate, SA = a.DataAplikasi.SA.Nama, Fund = a.DataAplikasi.Fund.Nama, MI = a.DataAplikasi.MI.Nama, SAReference = a.DataAplikasi.SAReference, InvestorFundUnitName = a.DataAplikasi.InvestorFundUnitName, AmountNominal = a.DataAplikasi.AmountNominal, MatchingId = a.DataAplikasi.MatchingId, MatchingWarna = a.DataAplikasi.Matching.Warna, MatchingNama = a.DataAplikasi.Matching.Nama }).AsQueryable();
            }
            else
            {

                if (Data.OptionMatchDate && Data.OptionMatch)
                {
                    DateTime startdate = Data.StartMatchDate;
                    DateTime enddate = Data.EndMatchDate.AddDays(1);

                    if (Data.Match)
                    {
                        v = (from a in _context.TrDataAplikasi
                             .Include("DataAplikasi").Include("DataAplikasi.Matching").Include("DataAplikasi.SA").Include("DataAplikasi.MI").Include("DataAplikasi.Fund")
                            .Where(x => x.Transaksi.Retur == false &&
                                        match.Contains(x.Transaksi.MatchingId) &&
                                        x.DataAplikasi.UpdateDate >= startdate && x.DataAplikasi.UpdateDate <= enddate)
                                select new HistoryAppVM { TransaksiId = a.TransaksiId, TanggalUpload = a.DataAplikasi.CreateDate, TanggalMatch = a.DataAplikasi.UpdateDate, TanggalTransaksi = a.DataAplikasi.TransactionDate, SA = a.DataAplikasi.SA.Nama, Fund = a.DataAplikasi.Fund.Nama, MI = a.DataAplikasi.MI.Nama, SAReference = a.DataAplikasi.SAReference, InvestorFundUnitName = a.DataAplikasi.InvestorFundUnitName, AmountNominal = a.DataAplikasi.AmountNominal, MatchingId = a.DataAplikasi.MatchingId, MatchingWarna = a.DataAplikasi.Matching.Warna, MatchingNama = a.DataAplikasi.Matching.Nama }).AsQueryable();
                    }else
                    {
                        v = (from a in _context.TrDataAplikasi
                             .Include("DataAplikasi").Include("DataAplikasi.Matching").Include("DataAplikasi.SA").Include("DataAplikasi.MI").Include("DataAplikasi.Fund")
                            .Where(x => x.Transaksi.Retur == false &&
                                        reject.Contains(x.Transaksi.MatchingId) &&
                                        x.DataAplikasi.UpdateDate >= startdate && x.DataAplikasi.UpdateDate <= enddate)
                                select new HistoryAppVM { TransaksiId = a.TransaksiId, TanggalUpload = a.DataAplikasi.CreateDate, TanggalMatch = a.DataAplikasi.UpdateDate, TanggalTransaksi = a.DataAplikasi.TransactionDate, SA = a.DataAplikasi.SA.Nama, Fund = a.DataAplikasi.Fund.Nama, MI = a.DataAplikasi.MI.Nama, SAReference = a.DataAplikasi.SAReference, InvestorFundUnitName = a.DataAplikasi.InvestorFundUnitName, AmountNominal = a.DataAplikasi.AmountNominal, MatchingId = a.DataAplikasi.MatchingId, MatchingWarna = a.DataAplikasi.Matching.Warna, MatchingNama = a.DataAplikasi.Matching.Nama }).AsQueryable();
                    }
                }

                if (Data.OptionSA)
                {
                    v = v.Where(x => x.SA.ToLower().Contains(Data.SA.ToLower()));
                }

                if (Data.OptionFund)
                {
                    v = v.Where(x => x.Fund.ToLower().Contains(Data.Fund.ToLower()));
                }

                if (Data.OptionMI)
                {
                    v = v.Where(x => x.MI.ToLower().Contains(Data.MI.ToLower()));
                }

                if (Data.OptionSARef)
                {
                    v = v.Where(x => x.SAReference.ToLower().Contains(Data.SARef.ToLower()));
                }

                if (Data.OptionInvestor)
                {
                    v = v.Where(x => x.InvestorFundUnitName.ToLower().Contains(Data.Investor.ToLower()));
                }

                if (Data.OptionNominal)
                {
                    v = v.Where(x => x.AmountNominal == Data.Nominal);
                }
            }

            //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }
            else
            {
                v = v.OrderBy(x => x.TransaksiId);
            }

            recordsTotal = v.Count();
            var data = v.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
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

        public JsonResult Reversal(int id, string keterangan)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());
            bool result = false;
            var match = new List<int> { 2, 3, 4, 5, 7, 8, 9, 10 };

            var trans = _context.Transaksi.SingleOrDefault(x => x.Id == id && match.Contains(x.MatchingId));
            if (trans != null)
            {
                if (trans.MatchingId >= 7)
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
                }
                else
                {
                    int matchid = 1;
                    var trapp = _context.TrDataAplikasi.Where(x => x.TransaksiId == id).ToList();
                    foreach (var app in trapp)
                    {
                        var apps = _context.DataAplikasi.SingleOrDefault(x => x.Id == app.DataAplikasiId);
                        apps.MatchingId = matchid;
                        apps.KeteranganUser = keterangan;
                        _context.Entry(apps).State = EntityState.Modified;
                        _context.SaveChanges();
                    }

                    var trfund = _context.TrDataFund.Where(x => x.TransaksiId == id).ToList();
                    foreach (var fund in trfund)
                    {
                        var funds = _context.DataFund.SingleOrDefault(x => x.Id == fund.DataFundId);
                        funds.MatchingId = matchid;
                        funds.KeteranganUser = keterangan;
                        _context.Entry(funds).State = EntityState.Modified;
                        _context.SaveChanges();
                    }

                    _context.TrDataAplikasi.RemoveRange(trapp);
                    _context.TrDataFund.RemoveRange(trfund);
                    _context.Transaksi.Remove(trans);

                    _context.SaveChanges();
                }
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}