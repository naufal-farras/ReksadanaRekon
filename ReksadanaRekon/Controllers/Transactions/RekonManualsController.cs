using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OfficeOpenXml;
using ReksadanaRekon.Models;
using ReksadanaRekon.Models.Data;
using ReksadanaRekon.Models.Trans;
using ReksadanaRekon.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReksadanaRekon.Controllers.Transactions
{
    public class RekonManualsController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();

        // GET: RekonManuals
        public ActionResult Index()
        {
            FundAplikasiVM fundAplikasi = new FundAplikasiVM();
            fundAplikasi.allDataAplikasi = _context.DataAplikasi.Include("Matching").Include("SA").Include("MI").Include("Fund").Where(x => x.MatchingId == 1).ToList();
            fundAplikasi.allDataFund = _context.DataFund.Include("Matching").Include("Rekening").Where(x => x.MatchingId == 1).ToList();

            return View(fundAplikasi);
        }
        public JsonResult AcceptApp(List<IdFundAplikasiVM> items, string keterangan)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());
            bool result = false;
            foreach (var data in items)
            {
                Transaksi trans = new Transaksi();
                trans.MatchingId = 5;
                trans.CreateDate = DateTime.Now;
                trans.InputerId = currentUser.Id;
                trans.Retur = false;
                trans.KeteranganInputer = keterangan;
                trans.IsDelete = false;
                _context.Transaksi.Add(trans);
                _context.SaveChanges();

                TrDataAplikasi trapp = new TrDataAplikasi();
                trapp.DataAplikasiId = data.IdApp;
                trapp.CreateDate = DateTime.Now;
                trapp.TransaksiId = trans.Id;
                _context.TrDataAplikasi.Add(trapp);

                if (_context.SaveChanges() > 0)
                {
                    var app = _context.DataAplikasi.Where(x => x.Id == data.IdApp).FirstOrDefault();
                    app.MatchingId = 5;
                    app.KeteranganUser = keterangan;
                    app.UpdateDate = DateTime.Now;
                    _context.Entry(app).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AcceptFund(List<IdFundAplikasiVM> items, string keterangan)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());
            bool result = false;
            foreach (var data in items)
            {
                Transaksi trans = new Transaksi();
                trans.MatchingId = 10;
                trans.CreateDate = DateTime.Now;
                trans.InputerId = currentUser.Id;
                trans.KeteranganInputer = keterangan;
                trans.Retur = false;
                trans.IsDelete = false;
                _context.Transaksi.Add(trans);
                _context.SaveChanges();

                TrDataFund trfund = new TrDataFund();
                trfund.DataFundId = data.IdFund;
                trfund.CreateDate = DateTime.Now;
                trfund.TransaksiId = trans.Id;
                _context.TrDataFund.Add(trfund);

                if (_context.SaveChanges() > 0)
                {
                    var fund = _context.DataFund.Where(x => x.Id == data.IdFund).FirstOrDefault();
                    fund.MatchingId = 10;
                    fund.KeteranganUser = keterangan;
                    fund.UpdateDate = DateTime.Now;
                    _context.Entry(fund).State = EntityState.Modified;
                    _context.SaveChanges();
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
                Transaksi trans = new Transaksi();
                trans.MatchingId = 6;
                trans.CreateDate = DateTime.Now;
                trans.InputerId = currentUser.Id;
                trans.KeteranganInputer = keterangan;
                trans.Retur = false;
                trans.IsDelete = false;
                _context.Transaksi.Add(trans);
                _context.SaveChanges();

                TrDataAplikasi trapp = new TrDataAplikasi();
                trapp.DataAplikasiId = data.IdApp;
                trapp.CreateDate = DateTime.Now;
                trapp.TransaksiId = trans.Id;
                _context.TrDataAplikasi.Add(trapp);

                if (_context.SaveChanges() > 0)
                {
                    var app = _context.DataAplikasi.Where(x => x.Id == data.IdApp).FirstOrDefault();
                    app.MatchingId = 6;
                    app.KeteranganUser = keterangan;
                    app.UpdateDate = DateTime.Now;
                    _context.Entry(app).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RejectFund(List<IdFundAplikasiVM> items, string keterangan)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            bool result = false;
            foreach (var data in items)
            {
                Transaksi trans = new Transaksi();
                trans.MatchingId = 11;
                trans.CreateDate = DateTime.Now;
                trans.InputerId = currentUser.Id;
                trans.KeteranganInputer = keterangan;
                trans.Retur = false;
                trans.IsDelete = false;
                _context.Transaksi.Add(trans);
                _context.SaveChanges();

                TrDataFund trfund = new TrDataFund();
                trfund.DataFundId = data.IdFund;
                trfund.CreateDate = DateTime.Now;
                trfund.TransaksiId = trans.Id;
                _context.TrDataFund.Add(trfund);

                if (_context.SaveChanges() > 0)
                {
                    var fund = _context.DataFund.Where(x => x.Id == data.IdFund).FirstOrDefault();
                    fund.MatchingId = 11;
                    fund.KeteranganUser = keterangan;
                    fund.UpdateDate = DateTime.Now;
                    _context.Entry(fund).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RekonsManual(List<IdFundAplikasiVM> IdApps, List<IdFundAplikasiVM> IdFunds, string keterangan)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            bool result = false;
            Transaksi trans = new Transaksi();
            trans.MatchingId = 4;
            trans.CreateDate = DateTime.Now;
            trans.InputerId = currentUser.Id;
            trans.KeteranganInputer = keterangan;
            trans.Retur = false;
            trans.IsDelete = false;
            _context.Transaksi.Add(trans);

            if (_context.SaveChanges() > 0)
            {
                foreach (var app in IdApps)
                {
                    TrDataAplikasi trapp = new TrDataAplikasi();
                    trapp.DataAplikasiId = app.IdApp;
                    trapp.CreateDate = DateTime.Now;
                    trapp.TransaksiId = trans.Id;
                    _context.TrDataAplikasi.Add(trapp);

                    if (_context.SaveChanges() > 0)
                    {
                        var getApp = _context.DataAplikasi.SingleOrDefault(x => x.Id == app.IdApp);
                        getApp.MatchingId = 4;
                        getApp.KeteranganUser = keterangan;
                        getApp.UpdateDate = DateTime.Now;
                        _context.Entry(getApp).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                }

                foreach (var fund in IdFunds)
                {
                    TrDataFund trfund = new TrDataFund();
                    trfund.DataFundId = fund.IdFund;
                    trfund.CreateDate = DateTime.Now;
                    trfund.TransaksiId = trans.Id;
                    _context.TrDataFund.Add(trfund);

                    if (_context.SaveChanges() > 0)
                    {
                        var getFun = _context.DataFund.SingleOrDefault(x => x.Id == fund.IdFund);
                        getFun.MatchingId = 4;
                        getFun.KeteranganUser = keterangan;
                        getFun.UpdateDate = DateTime.Now;
                        _context.Entry(getFun).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                }
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public void DownloadApp()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Unmatch Aplikasi");
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
            if (System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft)   // Right to Left for Arabic lang
            {
                ExcelWorksheetView wv = ws.View;
                wv.ZoomScale = 100;
                wv.RightToLeft = true;
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.Cells.AutoFitColumns();
            }
            else
            {
                ExcelWorksheetView wv = ws.View;
                wv.ZoomScale = 100;
                wv.RightToLeft = false;
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.Cells.AutoFitColumns();
            }
            ws.Cells.AutoFitColumns();
            DataTable dt = new DataTable(); // Read records from database here
            DataColumn[] cols = { new DataColumn("No"), new DataColumn("Tanggal Transaksi"), new DataColumn("SA Name"), new DataColumn("Fund Name"), new DataColumn("MI Name"), new DataColumn("SA Reference"), new DataColumn("A/C Name"), new DataColumn("Amount"), new DataColumn("Status") };
            dt.Columns.AddRange(cols);

            var dataApp = _context.DataAplikasi.Include("SA").Include("Fund").Include("MI").Include("Matching").Where(x => x.MatchingId == 1).OrderBy(x => new { x.SAId, x.FundId, x.MIId }).ToList();
            int i = 1;
            foreach (var data in dataApp)
            {
                DataRow row = dt.NewRow();
                row[0] = i;
                //row[1] = data.CreateDate.ToShortDateString();
                row[1] = data.TransactionDate.ToShortDateString();
                row[2] = data.SA.Nama;
                row[3] = data.Fund.Nama;
                row[4] = data.MI.Nama;
                row[5] = data.SAReference;
                row[6] = data.InvestorFundUnitName;
                row[7] = data.AmountNominal;
                row[8] = data.Matching.Nama;
                dt.Rows.Add(row);
                i++;
            }
            ws.Cells[1, 1].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);  // Print headers true
            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            HttpContext.Response.Clear();
            HttpContext.Response.AddHeader("", "");
            HttpContext.Response.Charset = System.Text.UTF8Encoding.UTF8.WebName;
            HttpContext.Response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
            HttpContext.Response.AddHeader("content-disposition", "attachment;  filename=Data Unmatch Aplikasi " + DateTime.Now.ToShortDateString().ToString() + " .xlsx");
            HttpContext.Response.ContentType = "application/text";
            HttpContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            HttpContext.Response.BinaryWrite(pck.GetAsByteArray());
            HttpContext.Response.End();
        }
        [HttpPost]
        public void DownloadFund()
        {
            ExcelPackage pck = new ExcelPackage();
            var rekening = _context.DataFund.Include("Rekening")
               .Where(x => x.MatchingId == 1)
               .GroupBy(x => x.RekeningId)
               .Select(x => new { idrek = x.FirstOrDefault().RekeningId, norek = x.FirstOrDefault().Rekening.NoRek, nama = x.FirstOrDefault().Rekening.NamaRek })
               .ToList();

            foreach (var rek in rekening)
            {
                string nrek = rek.norek.ToString();
                string newnrek = nrek.Substring(nrek.Length - 3, 3);
                string namarek = rek.nama.ToUpper().Replace("REKSA DANA", "").Replace("SYARIAH", "");
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(newnrek + "-" + namarek);
                ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
                if (System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft)   // Right to Left for Arabic lang
                {
                    ExcelWorksheetView wv = ws.View;
                    wv.ZoomScale = 100;
                    wv.RightToLeft = true;
                    ws.PrinterSettings.Orientation = eOrientation.Landscape;
                    ws.Cells.AutoFitColumns();
                }
                else
                {
                    ExcelWorksheetView wv = ws.View;
                    wv.ZoomScale = 100;
                    wv.RightToLeft = false;
                    ws.PrinterSettings.Orientation = eOrientation.Landscape;
                    ws.Cells.AutoFitColumns();
                }
                ws.Cells.AutoFitColumns();
                DataTable dt = new DataTable(); // Read records from database here
                DataColumn[] cols = { new DataColumn("No"), new DataColumn("Tanggal Transaksi"), new DataColumn("No Rekening"), new DataColumn("Nama Rekening"), new DataColumn("Mata Uang"), new DataColumn("Keterangan"), new DataColumn("Jumlah"), new DataColumn("Saldo") };
                dt.Columns.AddRange(cols);
                var dataFund = _context.DataFund.Include("Rekening").Where(x => x.MatchingId == 1 && x.RekeningId == rek.idrek).ToList();
                int i = 1;
                foreach (var data in dataFund)
                {
                    DataRow row = dt.NewRow();
                    row[0] = i;
                    //row[1] = data.CreateDate.ToShortDateString().ToString();
                    row[1] = data.Tanggal.ToShortDateString().ToString();
                    row[2] = data.Rekening.NoRek;
                    row[3] = data.Rekening.NamaRek;
                    row[4] = data.CCY;
                    row[5] = data.Keterangan;
                    row[6] = data.Jumlah;
                    row[7] = data.Saldo;
                    dt.Rows.Add(row);
                    i++;
                }
                ws.Cells[1, 1].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);  // Print headers true
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
            }

            HttpContext.Response.Clear();
            HttpContext.Response.AddHeader("", "");
            HttpContext.Response.Charset = System.Text.UTF8Encoding.UTF8.WebName;
            HttpContext.Response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
            HttpContext.Response.AddHeader("content-disposition", "attachment;  filename=Data Unmatch Rekening " + DateTime.Now.ToShortDateString().ToString() + " .xlsx");
            HttpContext.Response.ContentType = "application/text";
            HttpContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            HttpContext.Response.BinaryWrite(pck.GetAsByteArray());
            HttpContext.Response.End();
        }
    }
}