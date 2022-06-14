using Dapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ReksadanaRekon.Models;
using ReksadanaRekon.Models.Data;
using ReksadanaRekon.Models.Master;
using ReksadanaRekon.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _excel = Microsoft.Office.Interop.Excel;

namespace ReksadanaRekon.Controllers.Transactions
{
    public class RetursController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["ReksadanaRekon"].ToString());
        // GET: Returns
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList()
        {
            var result = _context.DataRetur
                    .Include("Matching")
                    .Include("SA")
                    .Include("MI")
                    .Include("Fund")
                    .Where(x => x.MatchingId == 1).Select(x => new { Id = x.Id, TransDate = x.TransDate, SA = x.SA.Nama, Fund = x.Fund.Nama, MI = x.MI.Nama, NoRekening = x.NoRekening, RekeningNasabah = x.RekeningNasabah, NamaNasabah = x.NamaNasabah, Bank = x.NamaBank, Nominal = x.Nominal, KeteranganRetur = x.KeteranganRetur, IFUA = x.IFUA, PaymentDate = x.PaymentDate, NoJurnal = x.NoJurnal, MatchingNama = x.Matching.Nama, MatchingWarna = x.Matching.Warna }).OrderByDescending(x => x.Id).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int id)
        {
            var result = _context.DataRetur.SingleOrDefault(x => x.Id == id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSA()
        {
            var result = _context.SA.OrderBy(x => x.Nama).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFund()
        {
            var result = _context.Fund.OrderBy(x => x.Nama).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMI()
        {
            var result = _context.MI.OrderBy(x => x.Nama).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBank()
        {
            var result = _context.Bank.OrderBy(x => x.Nama).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Save(DataRetur data)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            //var RekeningId = 0;

            //var checkRek = _context.Rekening.Where(x => x.NoRek == data.NoRekening && x.SAId == data.SAId && x.FundId == data.SAId && x.MIId == data.MIId).FirstOrDefault();

            //if(checkRek == null)
            //{
            //    var rekening = new Rekening();
            //    rekening.NoRek = data.NoRekening;
            //    rekening.NamaRek = data.NoRekening;
            //    rekening.SAId = data.SAId;
            //    rekening.FundId = data.FundId;
            //    rekening.MIId = data.MIId;
            //    rekening.UserId = currentUser.Id;
            //    rekening.CreateDate = DateTime.Now;
            //    rekening.IsDelete = false;

            //    _context.SaveChanges();

                //RekeningId = rekening.Id;

            //}
            //else
            //{
            //    RekeningId = checkRek.Id;
            //}

            if (data.Id == 0)
            {
                data.CreateDate = DateTime.Now;
                data.UserId = currentUser.Id;
                data.MatchingId = 1;
                _context.DataRetur.Add(data);
            }
            else
            {
                var retur = _context.DataRetur.Single(m => m.Id == data.Id);
                retur.TransDate = data.TransDate;
                retur.NoRekening = data.NoRekening;               
                retur.RekeningNasabah = data.RekeningNasabah;
                retur.NamaNasabah = data.NamaNasabah;
                retur.NamaBank = data.NamaBank;
                retur.Nominal = data.Nominal;
                retur.KeteranganRetur = data.KeteranganRetur;
                retur.SAId = data.SAId;
                retur.FundId = data.FundId;
                retur.MIId = data.MIId;
                retur.UserId = currentUser.Id;
                retur.UpdateDate = DateTime.Now;                
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            bool result = false;
            var u = _context.DataRetur.Where(x => x.Id == id).FirstOrDefault();
            if (u != null)
            {
                _context.DataRetur.Remove(u);
                _context.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadFiles(HttpPostedFileBase[] files)
        {
            if (User.Identity.IsAuthenticated)
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = manager.FindById(User.Identity.GetUserId());

                var resultupload = new List<ResultUpload>();
                if (ModelState.IsValid)
                {
                    #region Upload
                    foreach (HttpPostedFileBase file in files)
                    {
                        if (file != null)
                        {
                            if (file.FileName.EndsWith("xlsx") || file.FileName.EndsWith("xls") || file.FileName.EndsWith("csv"))
                            {
                                _excel.Application application = new _excel.Application(); ;
                                _excel.Workbooks workbooks = application.Workbooks;
                                _excel.Workbook workbook = null;
                                _excel.Worksheet worksheet = null;
                                _excel.Range range = null;
                                string path = path = Server.MapPath("~/Content/Files/" + file.FileName);

                                try
                                {
                                    file.SaveAs(path);

                                    workbook = workbooks.Open(path);
                                    worksheet = workbook.ActiveSheet;
                                    range = worksheet.UsedRange;

                                    //try {
                                    //}
                                    //catch (Exception ex) {
                                    //    //show message with ex.Message
                                    //    //return Json(new { Message = ex.Message }, JsonRequestBehavior.AllowGet);
                                    //}
                                    //finally { 
                                    //}

                                    string judul = ((_excel.Range)range.Cells[1, 1]).Text;

                                    if (judul == "Template Input Data Retur Reksadana Aplikasi RENA")
                                    {
                                        int csuccess = 0, cfails = 0;

                                        #region Aplikasi
                                        for (int row = 4; row <= range.Rows.Count; row++)
                                        {
                                            int SAId = 0, FundId = 0, MIId = 0;

                                            string FundName = ((_excel.Range)range.Cells[row, 4]).Text;
                                            string SAName = ((_excel.Range)range.Cells[row, 6]).Text;
                                            string MIName = ((_excel.Range)range.Cells[row, 12]).Text;

                                            if (FundName == "" || SAName == "" || MIName == "")
                                                continue;

                                            string ReksadanaNo = ((_excel.Range)range.Cells[row, 3]).Text;
                                            string JurnalNo = ((_excel.Range)range.Cells[row, 5]).Text;
                                            string IFUA = ((_excel.Range)range.Cells[row, 7]).Text;
                                            string InvestorName = ((_excel.Range)range.Cells[row, 8]).Text;
                                            string InvestorAcc = ((_excel.Range)range.Cells[row, 9]).Text;
                                            string Bank = ((_excel.Range)range.Cells[row, 10]).Text;
                                            double Nominal = ((_excel.Range)range.Cells[row, 11]).Value2;
                                            string Keterangan = ((_excel.Range)range.Cells[row, 14]).Text;

                                            //DateTime TransDate = DateTime.ParseExact(((_excel.Range)range.Cells[row, 2]).Text, "d/MM/yyyy", CultureInfo.InvariantCulture);
                                            //DateTime PaymentDate = DateTime.ParseExact(((_excel.Range)range.Cells[row, 13]).Text, "d/MM/yyyy", CultureInfo.InvariantCulture);

                                            DateTime TransDate = DateTime.ParseExact(((_excel.Range)range.Cells[row, 2]).Text, "yyyyMMd", CultureInfo.InvariantCulture);
                                            DateTime PaymentDate = DateTime.ParseExact(((_excel.Range)range.Cells[row, 13]).Text, "yyyyMMd", CultureInfo.InvariantCulture);

                                            #region GetBankId
                                            //var BankId = _con.QueryFirstOrDefault<int>("SELECT Id FROM Banks WHERE Nama = @Bank", new { Bank });
                                            #endregion

                                            #region CheckMasterSA
                                            var resultSA = _con.QueryFirstOrDefault<SA>("SELECT Id FROM SAs WHERE Nama = @SAName", new { SAName });

                                            if (resultSA == null)
                                            {

                                                var SA_Id = _con.QuerySingle<int>("INSERT INTO SAs (Nama, CreateDate, UserId) OUTPUT INSERTED.Id VALUES (@SAName, @DateNow, @UserId);", new { SAName, DateNow = DateTime.Now, UserId = currentUser.Id });

                                                SAId = SA_Id;
                                            }
                                            else
                                            {
                                                SAId = resultSA.Id;
                                            }

                                            #endregion

                                            #region CheckMasterFund
                                            var resultFund = _con.QueryFirstOrDefault<Fund>("SELECT Id FROM Funds WHERE Nama = @FundName", new { FundName });
                                            if (resultFund == null)
                                            {
                                                var Fund_Id = _con.QuerySingle<int>("INSERT INTO Funds (Nama, CreateDate, UserId) OUTPUT INSERTED.Id VALUES (@FundName, @DateNow, @UserId);", new { FundName, DateNow = DateTime.Now, UserId = currentUser.Id });

                                                FundId = Fund_Id;
                                            }
                                            else
                                            {
                                                FundId = resultFund.Id;
                                            }
                                            #endregion

                                            #region CheckMasterMI
                                            var resultMI = _con.QueryFirstOrDefault<MI>("SELECT Id FROM MIs WHERE Nama = @MIName", new { MIName });

                                            if (resultMI == null)
                                            {
                                                var MI_Id = _con.QuerySingle<int>("INSERT INTO MIs (Nama, CreateDate, UserId) OUTPUT INSERTED.Id VALUES (@MIName, @DateNow, @UserId);", new { MIName, DateNow = DateTime.Now, UserId = currentUser.Id });

                                                MIId = MI_Id;
                                            }
                                            else
                                            {
                                                MIId = resultMI.Id;
                                            }
                                            #endregion

                                            #region CheckDataRekening
                                            var checkData = _con.QueryFirst<int>("SELECT COUNT(*) FROM DataReturs WHERE TransDate = @TransDate AND NoRekening = @NoRekening AND NamaNasabah = @NamaNasabah AND NamaBank = @NamaBank AND KeteranganRetur = @KeteranganRetur AND SAId = @SAId AND FundId = @FundId AND MIId = @MIId AND Nominal = @Nominal ",
                                                            new { TransDate = TransDate, NoRekening = ReksadanaNo, NamaNasabah = InvestorName, NamaBank = Bank, KeteranganRetur = Keterangan, SAId, FundId, MIId, Nominal });

                                            if (checkData == 0)
                                            {
                                                var MatchingId = 1;
                                                var CreateDate = DateTime.Now;
                                                var UserId = currentUser.Id;
                                                bool IsDelete = false;


                                                string sql = "INSERT INTO DataReturs (TransDate, NoRekening, IFUA, NamaNasabah, Nominal, MatchingId, SAId, FundId, MIId, NamaBank, RekeningNasabah, KeteranganRetur, PaymentDate, CreateDate, UserId, IsDelete) VALUES (@TransDate, @NoRekening, @IFUA, @NamaNasabah, @Nominal, @MatchingId, @SAId, @FundId, @MIId, @NamaBank, @RekeningNasabah, @KeteranganRetur, @PaymentDate, @CreateDate, @UserId, @IsDelete)";
                                                _con.Execute(sql, new { TransDate = TransDate, NoRekening = ReksadanaNo, IFUA = IFUA, NamaNasabah = InvestorName, Nominal, MatchingId, SAId, FundId, MIId, NamaBank = Bank, RekeningNasabah = InvestorAcc, KeteranganRetur = Keterangan, PaymentDate = PaymentDate, CreateDate, UserId, IsDelete });
                                                csuccess++;
                                            }
                                            else
                                            {
                                                cfails++;
                                            }
                                            #endregion
                                        }

                                        resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Success", warna = "success", success = csuccess, fails = cfails });
                                        #endregion
                                    }
                                    else
                                    {
                                        resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Fails", warna = "danger", success = 0, fails = 0 });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    return new HttpStatusCodeResult(500, ex.Message);
                                }
                                finally 
                                { 
                                    workbooks.Close();
                                    application.Quit();
                                    System.IO.File.Delete(path);

                                    GC.Collect();
                                    GC.WaitForPendingFinalizers();
                                }



                                //Marshal.ReleaseComObject(workbook);
                                //Marshal.ReleaseComObject(workbooks);
                                //Marshal.ReleaseComObject(application);

                                //GC.Collect();
                                //GC.WaitForPendingFinalizers();
                            }
                            else
                            {
                                resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Fails", warna = "danger", success = 0, fails = 0 });
                            }
                        }
                    }
                    #endregion
                }

                return Json(new { ResultUpload = resultupload }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}