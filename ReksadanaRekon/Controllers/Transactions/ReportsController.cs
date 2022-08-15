using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity.EntityFramework;
using OfficeOpenXml;
using ReksadanaRekon.Models;
using ReksadanaRekon.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReksadanaRekon.Controllers.Transactions
{
    public class ReportsController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        // GET: Reports
        public ActionResult Index()
        {
            //var match = new List<int> { 7, 8, 9, 10 };
            //var reject = new List<int> { 11 };

            //var result = _context.DataAplikasi.Include("SA").Include("Fund").Include("MI")
            //    //.Where(x => x.CreateDate.Year == DateTime.Now.Year && x.CreateDate.Month == DateTime.Now.Month && x.CreateDate.Day == DateTime.Now.Day)
            //    .GroupBy(x => new { x.SAId, x.FundId, x.MIId })
            //    .OrderBy(x => x.Key)
            //    .Select(x => new
            //    {
            //        SA = x.FirstOrDefault().SA.Nama,
            //        Fund = x.FirstOrDefault().Fund.Nama,
            //        MI = x.FirstOrDefault().MI.Nama,
            //        TotalData = x.Count(),
            //        TotalDataMatch = x.Count(y => match.Contains(y.MatchingId)),
            //        TotalDataReject = x.Count(y => reject.Contains(y.MatchingId)),
            //        TotalDana = x.Sum(y => y.AmountNominal),
            //        TotalDanaMatch = x.Where(y => match.Contains(y.MatchingId)).Sum(z => (Int64?)z.AmountNominal) ?? 0,
            //        TotalDanaReject = x.Where(y => reject.Contains(y.MatchingId)).Sum(z => (Int64?)z.AmountNominal) ?? 0
            //    })
            //    .ToList();

            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem
            {
                Text = "Subscription",
                Value = "1"
            });
            listItems.Add(new SelectListItem
            {
                Text = "Redemption & Switching",
                Value = "2",
                Selected = true
            });
            listItems.Add(new SelectListItem
            {
                Text = "Retur",
                Value = "3"
            });

            DateVM date = new DateVM();
            date.RekonTypeList = listItems;
            return View(date);
        }

        [HttpPost]
        public void DownloadReport()
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
            DataColumn[] cols = { new DataColumn("No"), new DataColumn("SA Name"), new DataColumn("Fund Name"), new DataColumn("MI Name"), new DataColumn("Subscription"), new DataColumn("Subscription Match"), new DataColumn("Subscription Reject"), new DataColumn("All Amount Fund"), new DataColumn("Match Amount Fund"), new DataColumn("Reject Amount Fund") };
            dt.Columns.AddRange(cols);

            var match = new List<int> { 2, 3, 4, 5, 7, 8, 9, 10 };
            var reject = new List<int> { 6, 11 };

            var result = _context.DataAplikasi.Include("SA").Include("Fund").Include("MI")
                .Where(x => x.CreateDate.Year == DateTime.Now.Year && x.CreateDate.Month == DateTime.Now.Month && x.CreateDate.Day == DateTime.Now.Day)
                .GroupBy(x => new { x.SAId, x.FundId, x.MIId })
                .OrderBy(x => x.Key)
                .Select(x => new
                {
                    SA = x.FirstOrDefault().SA.Nama,
                    Fund = x.FirstOrDefault().Fund.Nama,
                    MI = x.FirstOrDefault().MI.Nama,
                    TotalData = x.Count(),
                    TotalDataMatch = x.Count(y => match.Contains(y.MatchingId)),
                    TotalDataReject = x.Count(y => reject.Contains(y.MatchingId)),
                    TotalDana = x.Sum(y => y.AmountNominal),
                    TotalDanaMatch = x.Where(y => match.Contains(y.MatchingId)).Sum(z => (Int64?)z.AmountNominal) ?? 0,
                    TotalDanaReject = x.Where(y => reject.Contains(y.MatchingId)).Sum(z => (Int64?)z.AmountNominal) ?? 0
                })
                .ToList();

            //var dataApp = _context.DataAplikasi.Include("SA").Include("Fund").Include("MI").Include("Matching").Where(x => x.MatchingId == 1).OrderBy(x => new { x.SAId, x.FundId, x.MIId }).ToList();
            int i = 1;
            foreach (var data in result)
            {
                DataRow row = dt.NewRow();
                row[0] = i;
                row[1] = data.SA;
                row[2] = data.Fund;
                row[3] = data.MI;
                row[4] = data.TotalData;
                row[5] = data.TotalDataMatch;
                row[6] = data.TotalDataReject;
                row[7] = data.TotalDana;
                row[8] = data.TotalDanaMatch;
                row[9] = data.TotalDanaReject;
                dt.Rows.Add(row);
                i++;
            }
            ws.Cells[1, 1].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);  // Print headers true
            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            HttpContext.Response.Clear();
            HttpContext.Response.AddHeader("", "");
            HttpContext.Response.Charset = System.Text.UTF8Encoding.UTF8.WebName;
            HttpContext.Response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
            HttpContext.Response.AddHeader("content-disposition", "attachment;  filename=Report Hasil Rekonsiliasi " + DateTime.Now.ToShortDateString().ToString() + " .xlsx");
            HttpContext.Response.ContentType = "application/text";
            HttpContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            HttpContext.Response.BinaryWrite(pck.GetAsByteArray());
            HttpContext.Response.End();
        }

        [HttpPost]
        public void DownloadReportPdf1()
        {
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 25, 25, 25, 15);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            //Top Heading
            Chunk chunk = new Chunk("Summary of Daily Unit Transactions", FontFactory.GetFont("Arial", 18, Font.BOLD, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            //Horizontal Line
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            //Table
            PdfPTable table = new PdfPTable(10);
            float[] widths = new float[] { 10f, 55f, 90f, 55f, 15f, 15f, 15f, 30f, 30f, 30f };
            var backcolour = new BaseColor(255, 159, 64);
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;

            //Cell
            PdfPCell cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format("No"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("SA Name"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Fund Name"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("MI Name"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Data Subscription"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Amount Subscription"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("All"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Match"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Reject"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("All"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Match"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Reject"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            var match = new List<int> { 2, 3, 4, 5, 7, 8, 9, 10 };
            var reject = new List<int> { 6, 11 };
            var datas = _context.DataAplikasi.Include("SA").Include("Fund").Include("MI")
                //.Where(x => x.CreateDate.Year == DateTime.Now.Year && x.CreateDate.Month == DateTime.Now.Month && x.CreateDate.Day == DateTime.Now.Day)
                .GroupBy(x => new { x.SAId, x.FundId, x.MIId })
                .OrderBy(x => x.Key)
                .Select(x => new
                {
                    SA = x.FirstOrDefault().SA.Nama,
                    Fund = x.FirstOrDefault().Fund.Nama,
                    MI = x.FirstOrDefault().MI.Nama,
                    TotalData = x.Count(),
                    TotalDataMatch = x.Count(y => match.Contains(y.MatchingId)),
                    TotalDataReject = x.Count(y => reject.Contains(y.MatchingId)),
                    TotalDana = x.Sum(y => y.AmountNominal),
                    TotalDanaMatch = x.Where(y => match.Contains(y.MatchingId)).Sum(z => (Int64?)z.AmountNominal) ?? 0,
                    TotalDanaReject = x.Where(y => reject.Contains(y.MatchingId)).Sum(z => (Int64?)z.AmountNominal) ?? 0
                })
                .ToList();

            int i = 1;
            if (datas != null)
            {
                foreach (var data in datas)
                {
                    cell = new PdfPCell(new Phrase(String.Format(i.ToString()), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.SA), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.Fund), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.MI), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalData.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDataMatch.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDataReject.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDana.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDanaMatch.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDanaReject.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    i++;
                }
            }
            else
            {
                cell = new PdfPCell(new Phrase(String.Format("Data Tidak tersedia"), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                cell.Colspan = 10;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }

            //var totaldatas = _context.DataAplikasi.Where(x => x.CreateDate.Year == DateTime.Now.Year && x.CreateDate.Month == DateTime.Now.Month && x.CreateDate.Day == DateTime.Now.Day);
            var totaldatas = _context.DataAplikasi;
            var alldata = totaldatas.Count();
            var matchdata = totaldatas.Count(y => match.Contains(y.MatchingId));
            var rejectdata = totaldatas.Count(y => reject.Contains(y.MatchingId));
            var alldana = totaldatas.Sum(y => (Int64?)y.AmountNominal) ?? 0;
            var matchdana = totaldatas.Where(y => match.Contains(y.MatchingId)).Sum(z => (Int64?)z.AmountNominal) ?? 0;
            var rejectdana = totaldatas.Where(y => reject.Contains(y.MatchingId)).Sum(z => (Int64?)z.AmountNominal) ?? 0;

            cell = new PdfPCell(new Phrase(String.Format("Total"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(alldata.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(matchdata.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(rejectdata.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(alldana.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(matchdana.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(rejectdana.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_RIGHT;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            pdfDoc.Add(table);

            //Table
            table = new PdfPTable(4);
            table.WidthPercentage = 100;
            //0=Left, 1=Centre, 2=Right
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;

            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Made By"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Checked By"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Approved By"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(" "), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 4;
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Made By"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Checked By"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Approved By"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            //Add table to document
            pdfDoc.Add(table);

            Paragraph para = new Paragraph();
            para.Add("Hello Salma,\n\nThank you for being our valuable customer. We hope our letter finds you in the best of health and wealth.\n\nYours Sincerely, \nBank of America");
            pdfDoc.Add(para);

            //Horizontal Line
            line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            para = new Paragraph();
            para.Add("This PDF is generated using iTextSharp. You can read the turorial:");
            para.SpacingBefore = 20f;
            para.SpacingAfter = 20f;
            pdfDoc.Add(para);

            //Creating link
            chunk = new Chunk("How to Create a Pdf File");
            chunk.Font = FontFactory.GetFont("Arial", 25, Font.BOLD, BaseColor.RED);
            chunk.SetAnchor("http://www.yogihosting.com/create-pdf-asp-net-mvc/");
            pdfDoc.Add(chunk);

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Credit-Card-Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }

        [HttpPost]
        public void DownloadReportPdfSubs(DateVM datevm)
        {
            Document pdfDoc = new Document();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);

            #region Query

            _context.Database.CommandTimeout = 300;

            var match = new List<int> { 2, 3, 4, 5, 7, 8, 9, 10 };
            var matchSA = new List<int> { 2, 7 };
            var matchName = new List<int> { 3, 8 };
            var matchManual = new List<int> { 4, 5, 9, 10 };
            var reject = new List<int> { 6, 11 };

            var totaldatas = _context.TrDataAplikasi.Where(x => EntityFunctions.TruncateTime(x.CreateDate) == EntityFunctions.TruncateTime(datevm.startdate) && x.Transaksi.Retur == false);
            var mi = totaldatas.GroupBy(x => x.DataAplikasi.MIId).Count();
            var sa = totaldatas.GroupBy(x => x.DataAplikasi.SAId).Count();
            var fund = totaldatas.GroupBy(x => x.DataAplikasi.FundId).Count();
            var datesubcription = totaldatas.Select(x => x.DataAplikasi.TransactionDate).FirstOrDefault();

            var alldata = totaldatas.Count();
            var matchdata = totaldatas.Count(y => match.Contains(y.Transaksi.MatchingId));
            var matchdataSA = totaldatas.Count(y => matchSA.Contains(y.Transaksi.MatchingId));
            var matchdataName = totaldatas.Count(y => matchName.Contains(y.Transaksi.MatchingId));
            var matchdataManual = totaldatas.Count(y => matchManual.Contains(y.Transaksi.MatchingId));
            var rejectdata = totaldatas.Count(y => reject.Contains(y.Transaksi.MatchingId));
            var alldana = totaldatas.Sum(y => (Int64?)y.DataAplikasi.AmountNominal) ?? 0;
            var matchdana = totaldatas.Where(y => match.Contains(y.Transaksi.MatchingId)).Sum(z => (Int64?)z.DataAplikasi.AmountNominal) ?? 0;
            var rejectdana = totaldatas.Where(y => reject.Contains(y.Transaksi.MatchingId)).Sum(z => (Int64?)z.DataAplikasi.AmountNominal) ?? 0;
            var respelaksana = totaldatas.GroupBy(x => x.Transaksi.InputerId).Select(x => new { Nama = x.FirstOrDefault().Transaksi.Inputer.Nama }).ToList();
            string pelaksana = "";
            foreach (var pel in respelaksana)
            {
                pelaksana = pelaksana + pel.Nama + "\n ";
            }

            var datas = _context.DataAplikasi.Include("SA").Include("Fund").Include("MI")
                .Where(x => x.CreateDate.Year == datevm.startdate.Year && x.CreateDate.Month == datevm.startdate.Month && x.CreateDate.Day == datevm.startdate.Day)
                .GroupBy(x => new { x.SAId, x.FundId, x.MIId })
                .OrderBy(x => x.Key)
                .Select(x => new
                {
                    SA = x.FirstOrDefault().SA.Nama,
                    Fund = x.FirstOrDefault().Fund.Nama,
                    MI = x.FirstOrDefault().MI.Nama,
                    TotalData = x.Count(),
                    TotalDataMatch = x.Count(y => match.Contains(y.MatchingId)),
                    TotalDataReject = x.Count(y => reject.Contains(y.MatchingId)),
                    TotalDana = x.Sum(y => y.AmountNominal),
                    TotalDanaMatch = x.Where(y => match.Contains(y.MatchingId)).Sum(z => (Int64?)z.AmountNominal) ?? 0,
                    TotalDanaReject = x.Where(y => reject.Contains(y.MatchingId)).Sum(z => (Int64?)z.AmountNominal) ?? 0
                })
                .ToList();

            #endregion

            #region BeritaAcara
            pdfDoc.SetPageSize(PageSize.A4);
            //pdfDoc.SetMargins(40f, 40f, 40f, 30f);
            pdfDoc.SetMargins(85.05f, 73.7f, 40f, 85.05f);
            //pdfDoc.SetMargins(85.05f, 73.7f, 130.4f, 85.05f);
            pdfDoc.Open();

            #region Header
            //Table 1
            PdfPTable table = new PdfPTable(2);
            float[] widths = new float[] { 70f, 30f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;

            //Cell Row 1
            PdfPCell cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.PaddingTop = 15f;
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Content/Images/BNI_logo.png"));
            //image.ScaleAbsolute(200, 150);
            image.ScalePercent(10);
            cell.AddElement(image);
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_BOTTOM;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("BERITA ACARA REKONSILIASI TRANSAKSI\nSUBSCRIPTION REKSA DANA"), FontFactory.GetFont("Arial", 16, Font.BOLD, BaseColor.BLACK)));
            cell.PaddingTop = 20f;
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            //Paragraf 1
            Paragraph para = new Paragraph();
            para.Add("Pada hari " + datevm.startdate.ToString("dddd", new System.Globalization.CultureInfo("id-ID")) + " tanggal " + datevm.startdate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID")) + " telah dilakukan rekonsiliasi transaksi subscription Reksa Dana dengan keterangan sebagai berikut :");
            para.SpacingBefore = 20f;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            #region Data
            //Table 2
            table = new PdfPTable(4);
            widths = new float[] { 90f, 5f, 15f, 85f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 10f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("Jumlah MI yang bertransaksi"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(mi.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Perusahaan"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Jumlah Selling Agent yang bertransaksi"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(sa.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Perusahaan"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 3
            cell = new PdfPCell(new Phrase(String.Format("Jumlah Product Reksa Dana"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(fund.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Unit"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            //Paragraf 2
            para = new Paragraph();
            para.Add("Hasil dari rekonsiliasi yang telah dilakukan melalui sistem adalah sebagai berikut :");
            para.SpacingBefore = 10f;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            #region Data2
            //Table 3
            table = new PdfPTable(4);
            //widths = new float[] { 100f, 5f, 20f, 60f };
            widths = new float[] { 120f, 5f, 15f, 55f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("Jumlah data subscription tanggal " + datesubcription.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"))), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(alldata.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("data"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            #region Hasil
            //Table 4
            table = new PdfPTable(4);
            //widths = new float[] { 80f, 5f, 20f, 80f };
            widths = new float[] { 90f, 5f, 15f, 85f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 10f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("Jumlah match by SA Referance"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(matchdataSA.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("data"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Jumlah match by Name"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(matchdataName.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("data"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 3
            cell = new PdfPCell(new Phrase(String.Format("Jumlah match by Manual"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(matchdataManual.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("data"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 4
            cell = new PdfPCell(new Phrase(String.Format("Jumlah reject"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(rejectdata.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("data"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            //Paragraf 3
            para = new Paragraph();
            para.Add("Dengan summary rekonsiliasi transaksi subscription Reksa Dana terlampir.");
            para.SpacingBefore = 10f;
            pdfDoc.Add(para);

            //Paragraf 4
            para = new Paragraph();
            para.Add("Demikian Berita Acara Klarifikasi ini dibuat dengan sebenar-benarnya untuk dapat dipergunakan sebagaimana mestinya");
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            #region TTD
            //Table 5
            table = new PdfPTable(3);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 20f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("Pelaksana"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            //cell = new PdfPCell(new Phrase(String.Format("Mengetahui"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Approval"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Mengetahui"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            //cell = new PdfPCell(new Phrase(String.Format("Pelaksana"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);


            //Cell Row 2
            //cell = new PdfPCell(new Phrase(String.Format(mengetahui.Nama), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell = new PdfPCell(new Phrase(String.Format(pelaksana), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            //cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.PaddingTop = 100f;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(String.Format(approval), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.PaddingTop = 100f;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            //cell = new PdfPCell(new Phrase(String.Format(pelaksana), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.PaddingTop = 100f;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            #endregion

            #region Lampiran
            pdfDoc.SetPageSize(PageSize.A4.Rotate());
            pdfDoc.SetMargins(30f, 30f, 30f, 30f);
            pdfDoc.NewPage();

            para = new Paragraph();
            para.Add("Lampiran Berita Acara Rekonsiliasi");
            para.SpacingBefore = 10f;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            //Top Heading
            Chunk chunk = new Chunk("Summary Rekonsiliasi Transaksi Subscription Reksa Dana", FontFactory.GetFont("Arial", 16, Font.BOLD, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            //Horizontal Line
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            //Table
            table = new PdfPTable(10);
            widths = new float[] { 10f, 55f, 90f, 55f, 15f, 15f, 15f, 30f, 30f, 30f };
            var backcolour = new BaseColor(255, 159, 64);
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;

            #region Header Tabel
            //Cell
            cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format("No"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("SA Name"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Fund Name"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("MI Name"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Data Subscription"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Amount Subscription"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("All"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Match"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Reject"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("All"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Match"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Reject"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);
            #endregion            

            #region Isi Tabel
            int i = 1;
            //decimal alldana = 0, matchdana = 0, rejectdana = 0;
            if (datas != null)
            {
                foreach (var data in datas)
                {
                    cell = new PdfPCell(new Phrase(String.Format(i.ToString()), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.SA), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.Fund), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.MI), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalData.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDataMatch.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDataReject.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDana.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);
                    //alldana = alldana + data.TotalDana;

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDanaMatch.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);
                    //matchdana = matchdana + data.TotalDanaMatch;

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDanaReject.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);
                    //rejectdana = rejectdana + data.TotalDanaReject;

                    i++;
                }
            }
            else
            {
                cell = new PdfPCell(new Phrase(String.Format("Data Tidak tersedia"), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                cell.Colspan = 10;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }
            #endregion

            #region Footer Tabel
            cell = new PdfPCell(new Phrase(String.Format("Total"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(alldata.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(matchdata.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(rejectdata.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(alldana.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(matchdana.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(rejectdana.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_RIGHT;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);
            #endregion

            pdfDoc.Add(table);
            #endregion

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Berita Acara Subscription " + datevm.startdate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID")) + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }

        [HttpPost]
        public void DownloadReportPdfReds(DateVM datevm)
        {
            Document pdfDoc = new Document();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);

            #region Query

            _context.Database.CommandTimeout = 300;

            var match = new List<int> { 2, 3, 4, 5, 7, 8, 9, 10 };
            var matchSA = new List<int> { 2, 7 };
            var matchName = new List<int> { 3, 8 };
            var matchManual = new List<int> { 4, 5, 9, 10 };
            var reject = new List<int> { 6, 11 };

            var totaldatas = _context.TrRedAplikasi.Where(x => EntityFunctions.TruncateTime(x.CreateDate) == EntityFunctions.TruncateTime(datevm.startdate));
            var mi = totaldatas.GroupBy(x => x.DataRedemp.MIId).Count();
            var sa = totaldatas.GroupBy(x => x.DataRedemp.SAId).Count();
            var fund = totaldatas.GroupBy(x => x.DataRedemp.FundId).Count();
            var datesubcription = totaldatas.Select(x => x.DataRedemp.TransDate).FirstOrDefault();

            var alldata = totaldatas.Count();
            var matchdata = totaldatas.Count(y => match.Contains(y.TransRedemp.MatchingId));
            var matchdataSA = totaldatas.Count(y => matchSA.Contains(y.TransRedemp.MatchingId));
            var matchdataName = totaldatas.Count(y => matchName.Contains(y.TransRedemp.MatchingId));
            var matchdataManual = totaldatas.Count(y => matchManual.Contains(y.TransRedemp.MatchingId));
            var rejectdata = totaldatas.Count(y => reject.Contains(y.TransRedemp.MatchingId));
            var alldana = totaldatas.Sum(y => (Int64?)y.DataRedemp.Nominal) ?? 0;
            var matchdana = totaldatas.Where(y => match.Contains(y.TransRedemp.MatchingId)).Sum(z => (Int64?)z.DataRedemp.Nominal) ?? 0;
            var rejectdana = totaldatas.Where(y => reject.Contains(y.TransRedemp.MatchingId)).Sum(z => (Int64?)z.DataRedemp.Nominal) ?? 0;
            var respelaksana = totaldatas.GroupBy(x => x.TransRedemp.InputerId).Select(x => new { Nama = x.FirstOrDefault().TransRedemp.Inputer.Nama }).ToList();
            string pelaksana = "";
            foreach (var pel in respelaksana)
            {
                pelaksana = pelaksana + pel.Nama + "\n ";
            }

            var datas = _context.DataRedemp.Include("SA").Include("Fund").Include("MI")
                .Where(x => x.CreateDate.Year == datevm.startdate.Year && x.CreateDate.Month == datevm.startdate.Month && x.CreateDate.Day == datevm.startdate.Day)
                .GroupBy(x => new { x.SAId, x.FundId, x.MIId })
                .OrderBy(x => x.Key)
                .Select(x => new
                {
                    SA = x.FirstOrDefault().SA.Nama,
                    Fund = x.FirstOrDefault().Fund.Nama,
                    MI = x.FirstOrDefault().MI.Nama,
                    TotalData = x.Count(),
                    TotalDataMatch = x.Count(y => match.Contains(y.MatchingId)),
                    TotalDataReject = x.Count(y => reject.Contains(y.MatchingId)),
                    TotalDana = x.Sum(y => y.Nominal),
                    TotalDanaMatch = x.Where(y => match.Contains(y.MatchingId)).Sum(z => (Int64?)z.Nominal) ?? 0,
                    TotalDanaReject = x.Where(y => reject.Contains(y.MatchingId)).Sum(z => (Int64?)z.Nominal) ?? 0
                })
                .ToList();

            #endregion

            #region BeritaAcara
            pdfDoc.SetPageSize(PageSize.A4);
            //pdfDoc.SetMargins(40f, 40f, 40f, 30f);
            pdfDoc.SetMargins(85.05f, 73.7f, 40f, 85.05f);
            //pdfDoc.SetMargins(85.05f, 73.7f, 130.4f, 85.05f);
            pdfDoc.Open();

            #region Header
            //Table 1
            PdfPTable table = new PdfPTable(2);
            float[] widths = new float[] { 70f, 30f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;

            //Cell Row 1
            PdfPCell cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.PaddingTop = 15f;
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Content/Images/BNI_logo.png"));
            //image.ScaleAbsolute(200, 150);
            image.ScalePercent(10);
            cell.AddElement(image);
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_BOTTOM;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("BERITA ACARA REKONSILIASI TRANSAKSI\nREDEMPTION & SWITCHING REKSA DANA"), FontFactory.GetFont("Arial", 16, Font.BOLD, BaseColor.BLACK)));
            cell.PaddingTop = 20f;
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            //Paragraf 1
            Paragraph para = new Paragraph();
            para.Add("Pada hari " + datevm.startdate.ToString("dddd", new System.Globalization.CultureInfo("id-ID")) + " tanggal " + datevm.startdate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID")) + " telah dilakukan rekonsiliasi transaksi redemption & switching Reksa Dana dengan keterangan sebagai berikut :");
            para.SpacingBefore = 20f;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            #region Data
            //Table 2
            table = new PdfPTable(4);
            widths = new float[] { 90f, 5f, 15f, 85f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 10f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("Jumlah MI yang bertransaksi"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(mi.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Perusahaan"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Jumlah Selling Agent yang bertransaksi"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(sa.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Perusahaan"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 3
            cell = new PdfPCell(new Phrase(String.Format("Jumlah Product Reksa Dana"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(fund.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Unit"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            //Paragraf 2
            para = new Paragraph();
            para.Add("Hasil dari rekonsiliasi yang telah dilakukan melalui sistem adalah sebagai berikut :");
            para.SpacingBefore = 10f;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            #region Data2
            //Table 3
            table = new PdfPTable(4);
            //widths = new float[] { 100f, 5f, 20f, 60f };
            widths = new float[] { 120f, 5f, 15f, 55f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("Jumlah data subscription tanggal " + datesubcription.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"))), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(alldata.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("data"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            #region Hasil
            //Table 4
            table = new PdfPTable(4);
            //widths = new float[] { 80f, 5f, 20f, 80f };
            widths = new float[] { 90f, 5f, 15f, 85f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 10f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("Jumlah match by SA Referance"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(matchdataSA.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("data"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Jumlah match by Name"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(matchdataName.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("data"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 3
            cell = new PdfPCell(new Phrase(String.Format("Jumlah match by Manual"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(matchdataManual.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("data"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 4
            cell = new PdfPCell(new Phrase(String.Format("Jumlah reject"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(rejectdata.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("data"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            //Paragraf 3
            para = new Paragraph();
            para.Add("Dengan summary rekonsiliasi transaksi redemption & switching Reksa Dana terlampir.");
            para.SpacingBefore = 10f;
            pdfDoc.Add(para);

            //Paragraf 4
            para = new Paragraph();
            para.Add("Demikian Berita Acara Klarifikasi ini dibuat dengan sebenar-benarnya untuk dapat dipergunakan sebagaimana mestinya");
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            #region TTD
            //Table 5
            table = new PdfPTable(3);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 20f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("Pelaksana"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            //cell = new PdfPCell(new Phrase(String.Format("Mengetahui"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Approval"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Mengetahui"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            //cell = new PdfPCell(new Phrase(String.Format("Pelaksana"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);


            //Cell Row 2
            //cell = new PdfPCell(new Phrase(String.Format(mengetahui.Nama), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell = new PdfPCell(new Phrase(String.Format(pelaksana), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            //cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.PaddingTop = 100f;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(String.Format(approval), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.PaddingTop = 100f;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            //cell = new PdfPCell(new Phrase(String.Format(pelaksana), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.PaddingTop = 100f;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            #endregion

            #region Lampiran
            pdfDoc.SetPageSize(PageSize.A4.Rotate());
            pdfDoc.SetMargins(30f, 30f, 30f, 30f);
            pdfDoc.NewPage();

            para = new Paragraph();
            para.Add("Lampiran Berita Acara Rekonsiliasi");
            para.SpacingBefore = 10f;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            //Top Heading
            Chunk chunk = new Chunk("Summary Rekonsiliasi Transaksi Redemption & Switching Reksa Dana", FontFactory.GetFont("Arial", 16, Font.BOLD, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            //Horizontal Line
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            //Table
            table = new PdfPTable(10);
            widths = new float[] { 10f, 55f, 90f, 55f, 15f, 15f, 15f, 30f, 30f, 30f };
            var backcolour = new BaseColor(255, 159, 64);
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;

            #region Header Tabel
            //Cell
            cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format("No"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("SA Name"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Fund Name"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("MI Name"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Data Subscription"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Amount Subscription"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("All"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Match"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Reject"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("All"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Match"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Reject"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);
            #endregion            

            #region Isi Tabel
            int i = 1;
            //decimal alldana = 0, matchdana = 0, rejectdana = 0;
            if (datas != null)
            {
                foreach (var data in datas)
                {
                    cell = new PdfPCell(new Phrase(String.Format(i.ToString()), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    var SAs = "";
                    if (data.SA == null) SAs = "-"; else SAs = data.SA;

                    cell = new PdfPCell(new Phrase(String.Format(SAs), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    var Funds = "";
                    if (data.Fund == null) Funds = "-"; else Funds = data.Fund;

                    cell = new PdfPCell(new Phrase(String.Format(Funds), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    var MIs = "";
                    if (data.MI == null) MIs = "-"; else MIs = data.MI;

                    cell = new PdfPCell(new Phrase(String.Format(MIs), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalData.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDataMatch.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDataReject.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDana.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);
                    //alldana = alldana + data.TotalDana;

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDanaMatch.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);
                    //matchdana = matchdana + data.TotalDanaMatch;

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDanaReject.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);
                    //rejectdana = rejectdana + data.TotalDanaReject;

                    i++;
                }
            }
            else
            {
                cell = new PdfPCell(new Phrase(String.Format("Data Tidak tersedia"), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                cell.Colspan = 10;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }
            #endregion

            #region Footer Tabel
            cell = new PdfPCell(new Phrase(String.Format("Total"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(alldata.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(matchdata.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(rejectdata.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(alldana.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(matchdana.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(rejectdana.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_RIGHT;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);
            #endregion

            pdfDoc.Add(table);
            #endregion

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Berita Acara Rekonsiliasi Redemption-Switching " + datevm.startdate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID")) + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }

        [HttpPost]
        public void DownloadReportPdfReturs(DateVM datevm)
        {
            Document pdfDoc = new Document();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);

            #region Query

            _context.Database.CommandTimeout = 300;

            var match = new List<int> { 2, 3, 4, 5, 7, 8, 9, 10 };
            var matchSA = new List<int> { 2, 7 };
            var matchName = new List<int> { 3, 8 };
            var matchManual = new List<int> { 4, 5, 9, 10 };
            var reject = new List<int> { 6, 11 };

            var totaldatas = _context.TrDataRetur.Where(x => EntityFunctions.TruncateTime(x.CreateDate) == EntityFunctions.TruncateTime(datevm.startdate) && x.Transaksi.Retur == true);
            var mi = totaldatas.GroupBy(x => x.DataRetur.MIId).Count();
            var sa = totaldatas.GroupBy(x => x.DataRetur.SAId).Count();
            var fund = totaldatas.GroupBy(x => x.DataRetur.FundId).Count();
            var datesubcription = totaldatas.Select(x => x.DataRetur.TransDate).FirstOrDefault();

            var alldata = totaldatas.Count();
            var matchdata = totaldatas.Count(y => match.Contains(y.Transaksi.MatchingId));
            var matchdataSA = totaldatas.Count(y => matchSA.Contains(y.Transaksi.MatchingId));
            var matchdataName = totaldatas.Count(y => matchName.Contains(y.Transaksi.MatchingId));
            var matchdataManual = totaldatas.Count(y => matchManual.Contains(y.Transaksi.MatchingId));
            var rejectdata = totaldatas.Count(y => reject.Contains(y.Transaksi.MatchingId));
            var alldana = totaldatas.Sum(y => (Int64?)y.DataRetur.Nominal) ?? 0;
            var matchdana = totaldatas.Where(y => match.Contains(y.Transaksi.MatchingId)).Sum(z => (Int64?)z.DataRetur.Nominal) ?? 0;
            var rejectdana = totaldatas.Where(y => reject.Contains(y.Transaksi.MatchingId)).Sum(z => (Int64?)z.DataRetur.Nominal) ?? 0;
            var respelaksana = totaldatas.GroupBy(x => x.Transaksi.InputerId).Select(x => new { Nama = x.FirstOrDefault().Transaksi.Inputer.Nama }).ToList();
            string pelaksana = "";
            foreach (var pel in respelaksana)
            {
                pelaksana = pelaksana + pel.Nama + "\n ";
            }

            var datas = _context.DataRetur.Include("SA").Include("Fund").Include("MI")
                .Where(x => x.CreateDate.Year == datevm.startdate.Year && x.CreateDate.Month == datevm.startdate.Month && x.CreateDate.Day == datevm.startdate.Day)
                .GroupBy(x => new { x.SAId, x.FundId, x.MIId })
                .OrderBy(x => x.Key)
                .Select(x => new
                {
                    SA = x.FirstOrDefault().SA.Nama,
                    Fund = x.FirstOrDefault().Fund.Nama,
                    MI = x.FirstOrDefault().MI.Nama,
                    TotalData = x.Count(),
                    TotalDataMatch = x.Count(y => match.Contains(y.MatchingId)),
                    TotalDataReject = x.Count(y => reject.Contains(y.MatchingId)),
                    TotalDana = x.Sum(y => y.Nominal),
                    TotalDanaMatch = x.Where(y => match.Contains(y.MatchingId)).Sum(z => (Int64?)z.Nominal) ?? 0,
                    TotalDanaReject = x.Where(y => reject.Contains(y.MatchingId)).Sum(z => (Int64?)z.Nominal) ?? 0
                }).ToList();

            var datas2 = _context.DataRetur.Include("Matching")
                        .Where(x => x.CreateDate.Year == datevm.startdate.Year && x.CreateDate.Month == datevm.startdate.Month && x.CreateDate.Day == datevm.startdate.Day)
                        .Select(x => new
                        {
                            TglTransaksi = x.TransDate,
                            RekeningNasabah = x.RekeningNasabah,
                            NamaNasabah = x.NamaNasabah,
                            NoRekening = x.NoRekening,
                            Nominal = x.Nominal,
                            Status = x.Matching.Nama,
                            SaRefrence = x.SARefrence
                        }).ToList();


            var datas3 = _context.DataRetur
                        .Where(x => x.CreateDate.Year == datevm.startdate.Year && x.CreateDate.Month == datevm.startdate.Month && x.CreateDate.Day == datevm.startdate.Day)
                        .GroupBy(x => x.NoRekening)
                        .AsEnumerable()
                        .Select(x => new {
                            NoRekening = x.FirstOrDefault().NoRekening,
                            TotalNominal = x.Sum(y => y.Nominal)
                        });
            #endregion

            #region BeritaAcara
            pdfDoc.SetPageSize(PageSize.A4);
            //pdfDoc.SetMargins(40f, 40f, 40f, 30f);
            pdfDoc.SetMargins(85.05f, 73.7f, 40f, 85.05f);
            //pdfDoc.SetMargins(85.05f, 73.7f, 130.4f, 85.05f);
            pdfDoc.Open();

            #region Header
            //Table 1
            PdfPTable table = new PdfPTable(2);
            float[] widths = new float[] { 70f, 30f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;

            //Cell Row 1
            PdfPCell cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.PaddingTop = 15f;
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Content/Images/BNI_logo.png"));
            //image.ScaleAbsolute(200, 150);
            image.ScalePercent(10);
            cell.AddElement(image);
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_BOTTOM;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("BERITA ACARA REKONSILIASI TRANSAKSI\nRETUR REKSA DANA"), FontFactory.GetFont("Arial", 16, Font.BOLD, BaseColor.BLACK)));
            cell.PaddingTop = 20f;
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            //Paragraf 1
            Paragraph para = new Paragraph();
            para.Add("Pada hari " + datevm.startdate.ToString("dddd", new System.Globalization.CultureInfo("id-ID")) + " tanggal " + datevm.startdate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID")) + " telah dilakukan rekonsiliasi transaksi retur Reksa Dana dengan keterangan sebagai berikut :");
            para.SpacingBefore = 20f;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            #region Data
            //Table 2
            table = new PdfPTable(4);
            widths = new float[] { 90f, 5f, 15f, 85f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 10f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("Jumlah MI yang bertransaksi"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(mi.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Perusahaan"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Jumlah Selling Agent yang bertransaksi"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(sa.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Perusahaan"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 3
            cell = new PdfPCell(new Phrase(String.Format("Jumlah Product Reksa Dana"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(fund.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Unit"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            //Paragraf 2
            para = new Paragraph();
            para.Add("Hasil dari rekonsiliasi yang telah dilakukan melalui sistem adalah sebagai berikut :");
            para.SpacingBefore = 10f;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            #region Data2
            //Table 3
            table = new PdfPTable(4);
            //widths = new float[] { 100f, 5f, 20f, 60f };
            widths = new float[] { 120f, 5f, 15f, 55f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("Jumlah data subscription tanggal " + datesubcription.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"))), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(alldata.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("data"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            #region Hasil
            //Table 4
            table = new PdfPTable(4);
            //widths = new float[] { 80f, 5f, 20f, 80f };
            widths = new float[] { 90f, 5f, 15f, 85f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 10f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("Jumlah match by SA Referance"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(matchdataSA.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("data"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Jumlah match by Name"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(matchdataName.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("data"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 3
            cell = new PdfPCell(new Phrase(String.Format("Jumlah match by Manual"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(matchdataManual.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("data"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 4
            cell = new PdfPCell(new Phrase(String.Format("Jumlah reject"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(rejectdata.ToString()), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("data"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            //Paragraf 3
            para = new Paragraph();
            para.Add("Dengan summary rekonsiliasi transaksi retur Reksa Dana terlampir.");
            para.SpacingBefore = 10f;
            pdfDoc.Add(para);

            //Paragraf 4
            para = new Paragraph();
            para.Add("Demikian Berita Acara Klarifikasi ini dibuat dengan sebenar-benarnya untuk dapat dipergunakan sebagaimana mestinya");
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            #region TTD
            //Table 5
            table = new PdfPTable(3);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 20f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("Pelaksana"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            //cell = new PdfPCell(new Phrase(String.Format("Mengetahui"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Approval"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Mengetahui"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            //cell = new PdfPCell(new Phrase(String.Format("Pelaksana"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);


            //Cell Row 2
            //cell = new PdfPCell(new Phrase(String.Format(mengetahui.Nama), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell = new PdfPCell(new Phrase(String.Format(pelaksana), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            //cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.PaddingTop = 100f;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(String.Format(approval), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.PaddingTop = 100f;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            //cell = new PdfPCell(new Phrase(String.Format(pelaksana), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.PaddingTop = 100f;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            #endregion

            #region Lampiran
            pdfDoc.SetPageSize(PageSize.A4.Rotate());
            pdfDoc.SetMargins(30f, 30f, 30f, 30f);
            pdfDoc.NewPage();

            para = new Paragraph();
            para.Add("Lampiran Berita Acara Rekonsiliasi");
            para.SpacingBefore = 10f;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            //Top Heading
            Chunk chunk = new Chunk("Summary Rekonsiliasi Transaksi Retur Reksa Dana", FontFactory.GetFont("Arial", 16, Font.BOLD, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            //Horizontal Line
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            //Table
            table = new PdfPTable(10);
            widths = new float[] { 10f, 55f, 90f, 55f, 15f, 15f, 15f, 30f, 30f, 30f };
            var backcolour = new BaseColor(255, 159, 64);
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;

            #region Header Tabel
            //Cell
            cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format("No"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("SA Name"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Fund Name"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("MI Name"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Data Subscription"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Amount Subscription"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("All"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Match"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Reject"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("All"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Match"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Reject"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);
            #endregion            

            #region Isi Tabel
            int i = 1;
            //decimal alldana = 0, matchdana = 0, rejectdana = 0;
            if (datas != null)
            {
                foreach (var data in datas)
                {
                    cell = new PdfPCell(new Phrase(String.Format(i.ToString()), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.SA), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.Fund), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.MI), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalData.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDataMatch.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDataReject.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDana.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);
                    //alldana = alldana + data.TotalDana;

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDanaMatch.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);
                    //matchdana = matchdana + data.TotalDanaMatch;

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalDanaReject.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);
                    //rejectdana = rejectdana + data.TotalDanaReject;

                    i++;
                }
            }
            else
            {
                cell = new PdfPCell(new Phrase(String.Format("Data Tidak tersedia"), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                cell.Colspan = 10;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }
            #endregion

            #region Footer Tabel
            cell = new PdfPCell(new Phrase(String.Format("Total"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(alldata.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(matchdata.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(rejectdata.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(alldana.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(matchdana.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(rejectdana.ToString("n2")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_RIGHT;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);
            #endregion

            pdfDoc.Add(table);
            #endregion

            #region Lampiran2
            pdfDoc.SetPageSize(PageSize.A4.Rotate());
            pdfDoc.SetMargins(30f, 30f, 30f, 30f);
            pdfDoc.NewPage();

            para = new Paragraph();
            para.Add("Lampiran Berita Acara Rekonsiliasi");
            para.SpacingBefore = 10f;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            //Top Heading
            //Chunk chunk = new Chunk("Summary Rekonsiliasi Transaksi Retur Reksa Dana", FontFactory.GetFont("Arial", 16, Font.BOLD, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            //Horizontal Line
            //Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            //Table
            table = new PdfPTable(8);
            widths = new float[] { 10f, 55f, 90f, 55f, 90f, 30f, 30f, 30f };
            //var backcolour = new BaseColor(255, 159, 64);
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;

            #region Header Tabel
            //Cell
            cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format("No"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Rekening Debit"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Nama Reksadana"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Rekening Kredit"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Nama Investor"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Nominal"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("SA Refrence"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Status"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);
            #endregion

            #region Isi Tabel
            i = 1;
            //decimal alldana = 0, matchdana = 0, rejectdana = 0;
            if (datas2 != null)
            {
                string namarek = "";
                foreach (var data in datas2)
                {
                    cell = new PdfPCell(new Phrase(String.Format(i.ToString()), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.NoRekening), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    var getNamaRek = _context.Rekening.Where(x => x.NoRek == data.NoRekening).Select(x => x.NamaRek).FirstOrDefault();

                    if (getNamaRek == null)
                        namarek = "-";
                    else
                        namarek = getNamaRek;

                    cell = new PdfPCell(new Phrase(String.Format(namarek), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.RekeningNasabah == null ? "-" : data.RekeningNasabah), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.NamaNasabah), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.Nominal.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.SaRefrence == null ? "Tidak Tersedia" : data.SaRefrence), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.Status), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    i++;
                }
            }
            else
            {
                cell = new PdfPCell(new Phrase(String.Format("Data Tidak tersedia"), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                cell.Colspan = 10;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }
            #endregion

            pdfDoc.Add(table);

            //pdfDoc.SetPageSize(PageSize.A4.Rotate());
            //pdfDoc.SetMargins(30f, 30f, 30f, 30f);
            //pdfDoc.NewPage();

            //Table
            table = new PdfPTable(3);
            widths = new float[] { 10f, 90f, 55f };

            pdfDoc.Add(table);
            table.SetWidths(widths);
            table.WidthPercentage = 40;
            table.HorizontalAlignment = Element.ALIGN_LEFT;
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;

            #region Header Tabel
            //Cell
            cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format("SUMMARY"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format("No"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Nama Rekening"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Total Nominal"), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);
            #endregion

            #region Isi Tabel
            i = 1;
            //decimal alldana = 0, matchdana = 0, rejectdana = 0;
            if (datas3 != null)
            {
                string namarek = "";
                foreach (var data in datas3)
                {
                    cell = new PdfPCell(new Phrase(String.Format(i.ToString()), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.NoRekening), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(data.TotalNominal.ToString("n0")), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    i++;
                }
            }
            else
            {
                cell = new PdfPCell(new Phrase(String.Format("Data Tidak tersedia"), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                cell.Colspan = 10;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }
            #endregion

            pdfDoc.Add(table);
            #endregion

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Berita Acara Retur " + datevm.startdate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID")) + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }
    }
}