﻿using Microsoft.AspNet.Identity;
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

namespace ReksadanaRekon.Controllers.History
{
    public class HistoryReturFundsController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        // GET: HistoryReturFunds
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetList(FundSearchVM Data)
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
            IQueryable<HistoryFundVM> v = null;

            if (Data == null)
            {
                v = (from a in _context.TrDataFund
                     .Include("DataFund").Include("DataFund.Matching").Include("DataFund.Rekening")
                    .Where(x => x.Transaksi.Retur == true &&
                        match.Contains(x.Transaksi.MatchingId) &&
                        x.DataFund.UpdateDate.Value.Year == DateTime.Now.Year &&
                        x.DataFund.UpdateDate.Value.Month == DateTime.Now.Month &&
                        x.DataFund.UpdateDate.Value.Day == DateTime.Now.Day)
                     select new HistoryFundVM { TransaksiId = a.TransaksiId, TanggalUpload = a.DataFund.CreateDate, TanggalMatch = a.DataFund.UpdateDate, TanggalTransaksi = a.DataFund.Tanggal, NoRek = a.DataFund.Rekening.NoRek, NamaRek = a.DataFund.Rekening.NamaRek, Keterangan = a.DataFund.Keterangan, Jumlah = a.DataFund.Jumlah, MatchingId = a.DataFund.MatchingId, MatchingWarna = a.DataFund.Matching.Warna, MatchingNama = a.DataFund.Matching.Nama }).AsQueryable();
            }
            else
            {

                if (Data.OptionMatchDate && Data.OptionMatch)
                {
                    DateTime startdate = Data.StartMatchDate;
                    DateTime enddate = Data.EndMatchDate.AddDays(1);

                    if (Data.Match)
                    {
                        v = (from a in _context.TrDataFund
                            .Include("DataFund").Include("DataFund.Matching").Include("DataFund.Rekening")
                            .Where(x => x.Transaksi.Retur == true &&
                                        match.Contains(x.Transaksi.MatchingId) &&
                                        x.DataFund.UpdateDate >= startdate && x.DataFund.UpdateDate <= enddate)
                             select new HistoryFundVM { TransaksiId = a.TransaksiId, TanggalUpload = a.DataFund.CreateDate, TanggalMatch = a.DataFund.UpdateDate, TanggalTransaksi = a.DataFund.Tanggal, NoRek = a.DataFund.Rekening.NoRek, NamaRek = a.DataFund.Rekening.NamaRek, Keterangan = a.DataFund.Keterangan, Jumlah = a.DataFund.Jumlah, MatchingId = a.DataFund.MatchingId, MatchingWarna = a.DataFund.Matching.Warna, MatchingNama = a.DataFund.Matching.Nama }).AsQueryable();
                    }
                    else
                    {
                        v = (from a in _context.TrDataFund
                            .Include("DataFund").Include("DataFund.Matching").Include("DataFund.Rekening")
                            .Where(x => x.Transaksi.Retur == true &&
                                        reject.Contains(x.Transaksi.MatchingId) &&
                                        x.DataFund.UpdateDate >= startdate && x.DataFund.UpdateDate <= enddate)
                             select new HistoryFundVM { TransaksiId = a.TransaksiId, TanggalUpload = a.DataFund.CreateDate, TanggalMatch = a.DataFund.UpdateDate, TanggalTransaksi = a.DataFund.Tanggal, NoRek = a.DataFund.Rekening.NoRek, NamaRek = a.DataFund.Rekening.NamaRek, Keterangan = a.DataFund.Keterangan, Jumlah = a.DataFund.Jumlah, MatchingId = a.DataFund.MatchingId, MatchingWarna = a.DataFund.Matching.Warna, MatchingNama = a.DataFund.Matching.Nama }).AsQueryable();
                    }
                }

                if (Data.OptionNoRek)
                {
                    v = v.Where(x => x.NoRek.ToLower().Contains(Data.NoRek.ToLower()));
                }

                if (Data.OptionNamaRek)
                {
                    v = v.Where(x => x.NamaRek.ToLower().Contains(Data.NamaRek.ToLower()));
                }

                if (Data.OptionKeterangan)
                {
                    v = v.Where(x => x.Keterangan.ToLower().Contains(Data.Keterangan.ToLower()));
                }

                if (Data.OptionJumlah)
                {
                    v = v.Where(x => x.Jumlah == Data.Jumlah);
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
            TransFundReturVM fundAplikasi = new TransFundReturVM();
            fundAplikasi.allDataFund = _context.TrDataFund
                .Include("DataFund")
                .Include("DataFund.Matching")
                .Include("DataFund.Rekening")
                .Where(x => x.TransaksiId == id).ToList();

            fundAplikasi.allDataAplikasi = _context.TrDataRetur
                    .Include("DataRetur")
                    .Include("DataRetur.Matching")
                    .Include("DataRetur.SA")
                    .Include("DataRetur.MI")
                    .Include("DataRetur.Fund")
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
            //var match = new List<int> { 5, 10, 15 };
            var match = new List<int> { 2, 3, 4, 5, 7, 8, 9, 10, 12, 13, 14, 15 };

            var trans = _context.Transaksi.SingleOrDefault(x => x.Id == id && match.Contains(x.MatchingId));
            if (trans != null)
            {
                //int matchid = 1;
                //var trfund = _context.TrDataFund.Where(x => x.TransaksiId == id).ToList();
                //foreach (var fund in trfund)
                //{
                //    var funds = _context.DataFund.SingleOrDefault(x => x.Id == fund.DataFundId);
                //    funds.MatchingId = matchid;
                //    funds.KeteranganUser = keterangan;
                //    _context.Entry(funds).State = EntityState.Modified;
                //    _context.SaveChanges();
                //}

                //_context.TrDataFund.RemoveRange(trfund);
                //_context.Transaksi.Remove(trans);

                //_context.SaveChanges();

                if (trans.MatchingId >= 7)
                {
                    int matchid = trans.MatchingId + 5;
                    trans.MatchingId = matchid;
                    trans.KeteranganInputer = keterangan;
                    trans.CreateDate = DateTime.Now;
                    trans.InputerId = currentUser.Id;
                    _context.Entry(trans).State = EntityState.Modified;
                    _context.SaveChanges();

                    var trapp = _context.TrDataRetur.Where(x => x.TransaksiId == id).ToList();
                    foreach (var app in trapp)
                    {
                        var apps = _context.DataRetur.SingleOrDefault(x => x.Id == app.DataReturId);
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
                    var trapp = _context.TrDataRetur.Where(x => x.TransaksiId == id).ToList();
                    foreach (var app in trapp)
                    {
                        var apps = _context.DataRetur.SingleOrDefault(x => x.Id == app.DataReturId);
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