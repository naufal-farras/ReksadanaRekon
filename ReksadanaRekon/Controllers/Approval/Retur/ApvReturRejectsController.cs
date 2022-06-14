using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ReksadanaRekon.Models;
using ReksadanaRekon.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Web;
using System.Web.Mvc;

namespace ReksadanaRekon.Controllers.Approval.Retur
{
    public class ApvReturRejectsController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        // GET: ApvReturRejects
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList()
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
            //var country = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt16(start) : 0;
            int recordsTotal = 0;

            var match = new List<int> { 6 };

            //_context.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
            var v = (from a in _context.TrDataRetur
                     .Include("DataRetur")
                    .Include("DataRetur.Matching")
                    .Include("DataRetur.SA")
                    .Include("DataRetur.MI")
                    .Include("DataRetur.Fund")
                    .Where(x => x.Transaksi.Retur == true &&
                                match.Contains(x.Transaksi.MatchingId))
                     select new
                     {
                         TransaksiId = a.TransaksiId,
                         TanggalUpload = a.DataRetur.CreateDate,
                         TanggalMatch = a.DataRetur.UpdateDate,
                         TanggalTransaksi = a.DataRetur.TransDate,
                         SA = a.DataRetur.SA.Nama,
                         Fund = a.DataRetur.Fund.Nama,
                         MI = a.DataRetur.MI.Nama,
                         InvestorFundUnitName = a.DataRetur.NamaNasabah,
                         AmountNominal = a.DataRetur.Nominal,
                         MatchingWarna = a.DataRetur.Matching.Warna,
                         MatchingNama = a.DataRetur.Matching.Nama
                     }).AsQueryable();

            //SEARCHING...
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                v = v.Where(a => //a.TanggalUpload.Contains(search) ||
                                 //a.TanggalMatch.Contains(search) ||
                                 //a.TanggalTransaksi.Contains(search) || 
                                 //(SqlFunctions.DateName("day", a.TanggalUpload) + "/" + SqlFunctions.DateName("month", a.TanggalUpload) + "/" + SqlFunctions.DateName("year", a.TanggalUpload)).Contains(search) ||
                                 //(SqlFunctions.DateName("day", a.TanggalMatch) + "/" + SqlFunctions.DateName("month", a.TanggalMatch) + "/" + SqlFunctions.DateName("year", a.TanggalMatch)).Contains(search) ||
                                 //(SqlFunctions.DateName("day", a.TanggalTransaksi) + "/" + SqlFunctions.DateName("month", a.TanggalTransaksi) + "/" + SqlFunctions.DateName("year", a.TanggalTransaksi)).Contains(search) ||
                                 a.SA.ToLower().Contains(search) ||
                                 a.Fund.ToLower().Contains(search) ||
                                 a.InvestorFundUnitName.ToLower().Contains(search) ||
                                 a.AmountNominal.ToString().Contains(search.Replace(",", "").Replace(".", "")));
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
            TransFundReturVM fundAplikasi = new TransFundReturVM();
            fundAplikasi.allDataAplikasi = _context.TrDataRetur
                    .Include("DataRetur")
                    .Include("DataRetur.Matching")
                    .Include("DataRetur.SA")
                    .Include("DataRetur.MI")
                    .Include("DataRetur.Fund")
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

        public JsonResult ApproveApp(List<IdFundAplikasiVM> items, string keterangan)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());
            bool result = false;
            var match = new List<int> { 6 };
            foreach (var data in items)
            {
                var trans = _context.Transaksi.SingleOrDefault(x => x.Id == data.IdApp && match.Contains(x.MatchingId));
                if (trans != null)
                {
                    int matchid = trans.MatchingId + 5;
                    trans.MatchingId = matchid;
                    trans.KeteranganApprover = keterangan;
                    trans.UpdateDate = DateTime.Now;
                    trans.Retur = true;
                    trans.ApproverId = currentUser.Id;
                    _context.Entry(trans).State = EntityState.Modified;
                    _context.SaveChanges();

                    var trapp = _context.TrDataRetur.Where(x => x.TransaksiId == data.IdApp).ToList();
                    foreach (var app in trapp)
                    {
                        var apps = _context.DataRetur.SingleOrDefault(x => x.Id == app.DataReturId);
                        apps.MatchingId = matchid;
                        _context.Entry(apps).State = EntityState.Modified;
                        _context.SaveChanges();
                    }

                    var trfund = _context.TrDataFund.Where(x => x.TransaksiId == data.IdApp).ToList();
                    foreach (var fund in trfund)
                    {
                        var funds = _context.DataFund.SingleOrDefault(x => x.Id == fund.DataFundId);
                        funds.MatchingId = matchid;
                        _context.Entry(funds).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                }
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RejectApp(List<IdFundAplikasiVM> items, string keterangan)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());
            bool result = false;
            foreach (var data in items)
            {
                var trans = _context.Transaksi.SingleOrDefault(x => x.Id == data.IdApp);
                if (trans != null)
                {
                    int matchid = 1;
                    var trapp = _context.TrDataRetur.Where(x => x.TransaksiId == data.IdApp).ToList();
                    foreach (var app in trapp)
                    {
                        var apps = _context.DataRetur.SingleOrDefault(x => x.Id == app.DataReturId);
                        apps.MatchingId = matchid;
                        apps.KeteranganUser = keterangan;
                        _context.Entry(apps).State = EntityState.Modified;
                        _context.SaveChanges();
                    }

                    var trfund = _context.TrDataFund.Where(x => x.TransaksiId == data.IdApp).ToList();
                    foreach (var fund in trfund)
                    {
                        var funds = _context.DataFund.SingleOrDefault(x => x.Id == fund.DataFundId);
                        funds.MatchingId = matchid;
                        funds.KeteranganUser = keterangan;
                        _context.Entry(funds).State = EntityState.Modified;
                        _context.SaveChanges();
                    }

                    _context.TrDataRetur.RemoveRange(trapp);
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