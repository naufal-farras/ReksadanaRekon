using ReksadanaRekon.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReksadanaRekon.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public JsonResult pieResult()
        {
            var result = _context.DataAplikasi.Include("Matching")
                .Where(x => x.CreateDate.Year == DateTime.Now.Year && x.CreateDate.Month == DateTime.Now.Month && x.CreateDate.Day == DateTime.Now.Day)
                .GroupBy(x => new { x.MatchingId })
                .OrderBy(x => x.Key)
                .Select(x => new { Nama = x.FirstOrDefault().Matching.Nama, NamaDua = x.FirstOrDefault().Matching.Keterangan, Total = x.Count() })
                .ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult pieSA()
        {
            var result = _context.DataAplikasi.Include("SA")
                .Where(x => x.CreateDate.Year == DateTime.Now.Year && x.CreateDate.Month == DateTime.Now.Month && x.CreateDate.Day == DateTime.Now.Day)
                .GroupBy(x => x.SAId)
                .OrderBy(x => x.Key)
                .Select(x => new { Nama = x.FirstOrDefault().SA.Nama, Total = x.Count() })
                .ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult pieFund()
        {
            var result = _context.DataAplikasi.Include("Fund")
                .Where(x => x.CreateDate.Year == DateTime.Now.Year && x.CreateDate.Month == DateTime.Now.Month && x.CreateDate.Day == DateTime.Now.Day)
                .GroupBy(x => x.FundId)
                .OrderBy(x => x.Key)
                .Select(x => new { Nama = x.FirstOrDefault().Fund.Nama, Total = x.Count() })
                .ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult pieMI()
        {
            var result = _context.DataAplikasi.Include("MI")
                //.Where(x => x.CreateDate.Year == DateTime.Now.Year && x.CreateDate.Month == DateTime.Now.Month && x.CreateDate.Day == DateTime.Now.Day)
                .GroupBy(x => x.MIId)
                .OrderBy(x => x.Key)
                .Select(x => new { Nama = x.FirstOrDefault().MI.Nama, Total = x.Count() })
                .ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult lineChartData(string kelompok)
        //{
        //    List<ChartVM> result = new List<ChartVM>();
        //    if (kelompok == "OPR")
        //    {
        //        result = (from t in _context.Tahapan.Where(x => x.Id != 1 && x.Id != 2)
        //                  select new ChartVM
        //                  {
        //                      Label = t.NamaTahapan,
        //                      DatasetVM = (from b in _context.Bulan.Where(x => x.Id <= DateTime.Now.Month)
        //                                   join ar in _context.AlokasiRealisasi.Where(x => x.Periode == DateTime.Now.Year && x.TahapanId == t.Id).GroupBy(x => x.BulanId).Select(x => new { BulanId = x.FirstOrDefault().BulanId, Total = x.Sum(y => y.Total) }) on b.Id equals ar.BulanId
        //                                   select new DatasetVM
        //                                   {
        //                                       Labels = b.NamaBulan,
        //                                       Total = ar.Total
        //                                   }
        //                                   ).ToList()
        //                  }
        //           ).ToList();

        //    }
        //    else
        //    {
        //        result = (from t in _context.Tahapan.Where(x => x.Id != 1 && x.Id != 2)
        //                  select new ChartVM
        //                  {
        //                      Label = t.NamaTahapan,
        //                      DatasetVM = (from b in _context.Bulan.Where(x => x.Id <= DateTime.Now.Month)
        //                                   join ar in _context.AlokasiRealisasi.Where(x => x.Kelompok == kelompok && x.Periode == DateTime.Now.Year && x.TahapanId == t.Id) on b.Id equals ar.BulanId /*into bar*/
        //                                                                                                                                                                                              //from barr in bar.DefaultIfEmpty()
        //                                   select new DatasetVM
        //                                   {
        //                                       Labels = b.NamaBulan,
        //                                       //Total = barr == null ? 0 : barr.Total
        //                                       Total = ar.Total
        //                                   }
        //                                   ).ToList()
        //                  }
        //           ).ToList();
        //    }


        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult lineHistoryLabel()
        {
            var yesterday = DateTime.Today.AddDays(-14);
            var result = _context.Transaksi.Where(x => DbFunctions.TruncateTime(x.CreateDate) >= yesterday && DbFunctions.TruncateTime(x.CreateDate) <= DateTime.Today)
                .GroupBy(x => DbFunctions.TruncateTime(x.CreateDate))
                .OrderBy(x => x.FirstOrDefault().Id).ToList()
                .Select(x => new { Label = x.FirstOrDefault().CreateDate.ToString("dddd d/MM", new System.Globalization.CultureInfo("id-ID")) });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult lineHistoryData()
        {
            var yesterday = DateTime.Today.AddDays(-14);
            var result = _context.TrDataAplikasi.Where(x => DbFunctions.TruncateTime(x.Transaksi.CreateDate) >= yesterday && DbFunctions.TruncateTime(x.Transaksi.CreateDate) <= DateTime.Today)
                .GroupBy(x => DbFunctions.TruncateTime(x.Transaksi.CreateDate))
                .OrderBy(x => x.FirstOrDefault().Transaksi.Id)
                .Select(x => new { Count = x.Count(), Amount = x.Sum(y => y.DataAplikasi.AmountNominal) });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult pieChartData3()
        {
            var result = _context.DataAplikasi.Include("SA").Include("Fund")
                .Where(x => x.CreateDate.Year == DateTime.Now.Year && x.CreateDate.Month == DateTime.Now.Month && x.CreateDate.Day == DateTime.Now.Day)
                .GroupBy(x => new { x.SAId, x.FundId })
                .OrderBy(x => x.Key)
                .Select(x => new { Nama = x.FirstOrDefault().SA.Nama, NamaDua = x.FirstOrDefault().Fund.Nama, Total = x.Count() })
                .ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}