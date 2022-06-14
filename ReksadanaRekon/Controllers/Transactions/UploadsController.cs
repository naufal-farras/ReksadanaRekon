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
using System.Data.SqlClient;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using _excel = Microsoft.Office.Interop.Excel;

namespace ReksadanaRekon.Controllers.Transactions
{
    public class UploadsController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["ReksadanaRekon"].ToString());

        // GET: Uploads
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFiles2(HttpPostedFileBase[] files, int category)
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
                        DataBalikVM result = new DataBalikVM();
                        bool IsApp = false;
                        if (file != null)
                        {
                            if (category == 1 && file.FileName.EndsWith("csv"))
                            {
                                #region File Fund BCA
                                string path = Server.MapPath("~/Content/Files/DataFund.csv");

                                try
                                {
                                    if (System.IO.File.Exists(path))
                                    {
                                        System.IO.File.Delete(path);
                                    }

                                    file.SaveAs(path);


                                    result = _con.QueryFirst<DataBalikVM>("EXEC SP_InsertDataFund @UserId", new { UserId = currentUser.Id }, null, 360);                                    
                                }
                                catch (Exception ex)
                                {
                                    return new HttpStatusCodeResult(500, ex.Message);

                                }
                                finally
                                {
                                    System.IO.File.Delete(path);

                                    GC.Collect();
                                    GC.WaitForPendingFinalizers();
                                }
                                #endregion
                            }
                            else if (category == 2 && file.FileName.EndsWith("csv"))
                            {
                                #region File Fund BNI

                                var NaRek = file.FileName;
                                char[] spearator = { ' ', '.' };
                                string[] strlist = NaRek.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                                var NoRek = strlist[0];

                                string path = Server.MapPath("~/Content/Files/DataFund.csv");

                                try
                                {
                                    if (System.IO.File.Exists(path))
                                    {
                                        System.IO.File.Delete(path);
                                    }

                                    file.SaveAs(path);


                                    result = _con.QueryFirst<DataBalikVM>("EXEC SP_InsertDataFundBNI @UserId,@NoRek,@NaRek", new { UserId = currentUser.Id , NoRek,NaRek }, null, 360);
                                }
                                catch (Exception ex)
                                {
                                    return new HttpStatusCodeResult(500, ex.Message);

                                }
                                finally
                                {
                                    System.IO.File.Delete(path);

                                    GC.Collect();
                                    GC.WaitForPendingFinalizers();
                                }

                                #endregion
                            }
                            else if (category == 3 && file.FileName.EndsWith("csv"))
                            {
                                #region File Fund Mandiri
                                var NaRek = file.FileName;
                                char[] spearator = { ' ' };
                                string[] strlist = NaRek.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                                var NoRek = strlist[0];

                                string path = Server.MapPath("~/Content/Files/DataFund.csv");

                                try
                                {
                                    if (System.IO.File.Exists(path))
                                    {
                                        System.IO.File.Delete(path);
                                    }

                                    file.SaveAs(path);


                                    result = _con.QueryFirst<DataBalikVM>("EXEC SP_InsertDataFundMANDIRI @UserId,@NoRek,@NaRek", new { UserId = currentUser.Id, NoRek, NaRek }, null, 360);
                                }
                                catch (Exception ex)
                                {
                                    return new HttpStatusCodeResult(500, ex.Message);

                                }
                                finally
                                {
                                    System.IO.File.Delete(path);

                                    GC.Collect();
                                    GC.WaitForPendingFinalizers();
                                }

                                #endregion
                            }
                            else if (category == 4  && file.FileName.EndsWith("csv"))
                            {
                                #region File Fund OCBC

                                #endregion
                            }
                            else if (category == 5 && file.FileName.EndsWith("txt"))
                            {
                                #region File Fund Danamon

                                #endregion
                            }
                            else if (category == 6 && file.FileName.EndsWith("xls"))
                            {
                                #region File Aplikasi
                                string path = Server.MapPath("~/Content/Files/DataAplikasi.xls");

                                try
                                {
                                    if (System.IO.File.Exists(path))
                                    {
                                        System.IO.File.Delete(path);
                                    }

                                    file.SaveAs(path);
                                                                        
                                    result = _con.QueryFirst<DataBalikVM>("EXEC SP_InsertDataAplikasi @UserId", new { UserId = currentUser.Id }, null, 360);
                                  

                                }
                                catch (Exception ex)
                                {
                                    return new HttpStatusCodeResult(500, ex.Message);

                                }
                                finally
                                {
                                    System.IO.File.Delete(path);

                                    GC.Collect();
                                    GC.WaitForPendingFinalizers();
                                }
                                #endregion

                            }
                        }                        

                        if (result.DBAwal != result.DBAkhir)
                        {
                            #region Berhasil
                            if (IsApp)
                            {
                                resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Success Aplikasi", warna = "success", success = result.Success, fails = result.Fails });
                            }
                            else
                            {

                                resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Success Fund", warna = "success", success = result.Success, fails = result.Fails });
                            }
                            #endregion
                        }
                        else
                        {
                            #region Gagal
                            if (IsApp)
                            {
                                resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Fails Aplikasi", warna = "danger", success = result.Success, fails = result.Fails });
                            }
                            else
                            {
                                resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Fails Fund", warna = "danger", success = result.Success, fails = result.Fails });
                            }                            
                            #endregion
                        }
                    }
                    #endregion

                    Rekon();
                }

                return Json(new { ResultUpload = resultupload }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
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
                                string  path = Server.MapPath("~/Content/Files/" + file.FileName);

                                try
                                {
                                    file.SaveAs(path);

                                    workbook = workbooks.Open(path);
                                    worksheet = workbook.ActiveSheet;
                                    range = worksheet.UsedRange;

                                    string judul = ((_excel.Range)range.Cells[1, 1]).Text;
                                    string judul2 = ((_excel.Range)range.Cells[1, 2]).Text;
                                    string judul3 = ((_excel.Range)range.Cells[1, 3]).Text;
                                    string judul9 = ((_excel.Range)range.Cells[1, 9]).Text;
                                    string judul37 = ((_excel.Range)range.Cells[1, 37]).Text;
                                    judul = judul.ToLower().Replace(" ", "").Replace("\"", "").Replace("\\", "").Replace(",", "");
                                    judul2 = judul2.Replace(" ", "").ToLower();
                                    judul3 = judul3.Replace(" ", "").ToLower();
                                    judul9 = judul9.Replace(" ", "").ToLower();
                                    judul37 = judul37.Replace(" ", "").ToLower();

                                    if (judul == "no." && judul9 == "saname" && judul37 == "sareferenceno.")
                                    {
                                        #region Aplikasi
                                        string lastSACode = "Undefined", lastFundCode = "Undefined", lastIMCode = "Undefined";
                                        int lastSAId = 0, lastFundId = 0, lastIMId = 0;
                                        int csuccess = 0, cfails = 0;
                                        for (int row = 2; row <= range.Rows.Count; row++)
                                        {
                                            DateTime TransactionDate = DateTime.Now;
                                            string InvestorFundUnitNo = ((_excel.Range)range.Cells[row, 10]).Text;
                                            Int64 AmountNominal = Convert.ToInt64(((_excel.Range)range.Cells[row, 20]).Text);
                                            string SAReference = ((_excel.Range)range.Cells[row, 37]).Text;
                                            string ReferenceNo = ((_excel.Range)range.Cells[row, 4]).Text;

                                            int SAId = 0, FundId = 0, MIId = 0;
                                            string trandate = ((_excel.Range)range.Cells[row, 2]).Text;
                                            if (trandate != "")
                                            {
                                                var trandate2 = trandate.Substring(6, 2) + "/" + trandate.Substring(4, 2) + "/" + trandate.Substring(0, 4);
                                                TransactionDate = DateTime.ParseExact(trandate2, "d/MM/yyyy", CultureInfo.InvariantCulture);
                                            }

                                            string SACode = ((_excel.Range)range.Cells[row, 8]).Text;
                                            string SAName = ((_excel.Range)range.Cells[row, 9]).Text;

                                            #region CheckMasterSA
                                            if (lastSACode == SACode)
                                            {
                                                SAId = lastSAId;
                                            }
                                            else
                                            {
                                                var resultSA = _con.QueryFirstOrDefault<SA>("SELECT Id FROM SAs WHERE Code = @SACode AND Nama = @SAName", new { SACode = SACode, SAName = SAName });

                                                if (resultSA == null)
                                                {

                                                    var SA_Id = _con.QuerySingle<int>("INSERT INTO SAs (Code, Nama, CreateDate, UserId) OUTPUT INSERTED.Id VALUES (@SACode, @SAName, @DateNow, @UserId);", new { SACode = SACode, SAName = SAName, DateNow = DateTime.Now, UserId = currentUser.Id });

                                                    SAId = SA_Id;
                                                    lastSAId = SA_Id;
                                                    lastSACode = SACode;
                                                }
                                                else
                                                {
                                                    SAId = resultSA.Id;
                                                    lastSAId = resultSA.Id;
                                                    lastSACode = SACode;
                                                }
                                            }
                                            #endregion
                                            string FundCode = ((_excel.Range)range.Cells[row, 13]).Text;
                                            string FundName = ((_excel.Range)range.Cells[row, 14]).Text;

                                            #region CheckMasterFund
                                            if (lastFundCode == FundCode)
                                            {
                                                FundId = lastFundId;
                                            }
                                            else
                                            {
                                                var resultFund = _con.QueryFirstOrDefault<Fund>("SELECT Id FROM Funds WHERE Code = @FundCode AND Nama = @FundName", new { FundCode = FundCode, FundName = FundName });
                                                if (resultFund == null)
                                                {
                                                    var Fund_Id = _con.QuerySingle<int>("INSERT INTO Funds (Code, Nama, CreateDate, UserId) OUTPUT INSERTED.Id VALUES (@FundCode, @FundName, @DateNow, @UserId);", new { FundCode = FundCode, FundName = FundName, DateNow = DateTime.Now, UserId = currentUser.Id });

                                                    FundId = Fund_Id;
                                                    lastFundId = Fund_Id;
                                                    lastFundCode = FundCode;
                                                }
                                                else
                                                {
                                                    FundId = resultFund.Id;
                                                    lastFundId = resultFund.Id;
                                                    lastFundCode = FundCode;
                                                }
                                            }
                                            #endregion

                                            string IMCode = ((_excel.Range)range.Cells[row, 15]).Text;
                                            string IMName = ((_excel.Range)range.Cells[row, 16]).Text;

                                            #region CheckMasterMI
                                            if (lastIMCode == IMCode)
                                            {
                                                MIId = lastIMId;
                                            }
                                            else
                                            {
                                                var resultMI = _con.QueryFirstOrDefault<MI>("SELECT Id FROM MIs WHERE Code = @IMCode AND Nama = @IMName", new { IMCode = IMCode, IMName = IMName });

                                                if (resultMI == null)
                                                {
                                                    var MI_Id = _con.QuerySingle<int>("INSERT INTO MIs (Code, Nama, CreateDate, UserId) OUTPUT INSERTED.Id VALUES (@IMCode, @IMName, @DateNow, @UserId);", new { IMCode = IMCode, IMName = IMName, DateNow = DateTime.Now, UserId = currentUser.Id });

                                                    MIId = MI_Id;
                                                    lastIMId = MI_Id;
                                                    lastIMCode = IMCode;
                                                }
                                                else
                                                {
                                                    MIId = resultMI.Id;
                                                    lastIMId = resultMI.Id;
                                                    lastIMCode = IMCode;
                                                }
                                            }
                                            #endregion

                                            #region CheckDataAplikasi
                                            var checkData = _con.QueryFirst<int>("SELECT COUNT(Id) FROM DataAplikasis WHERE SAId = @SAId AND FundId = @FundId AND MIId = @MIId AND InvestorFundUnitNo = @InvestorFundUnitNo AND AmountNominal = @AmountNominal AND SAReference = @SAReference AND TransactionDate = @TransactionDate AND ReferenceNo = @ReferenceNo",
                                                            new { SAId, FundId, MIId, InvestorFundUnitNo, AmountNominal, SAReference, TransactionDate, ReferenceNo });

                                            if (checkData == 0)
                                            {
                                                DataAplikasi dataApp = new DataAplikasi();
                                                var TransactionType = ((_excel.Range)range.Cells[row, 3]).Text;
                                                var Status = ((_excel.Range)range.Cells[row, 5]).Text;
                                                var IMFeeAmendment = ((_excel.Range)range.Cells[row, 6]).Text;
                                                var IMPaymentDate = ((_excel.Range)range.Cells[row, 7]).Text;
                                                var InvestorFundUnitName = ((_excel.Range)range.Cells[row, 11]).Text;
                                                var SID = ((_excel.Range)range.Cells[row, 12]).Text;
                                                var CBCode = ((_excel.Range)range.Cells[row, 17]).Text;
                                                var CBName = ((_excel.Range)range.Cells[row, 18]).Text;
                                                var FundCCY = ((_excel.Range)range.Cells[row, 19]).Text;
                                                var AmountUnit = ((_excel.Range)range.Cells[row, 21]).Text;
                                                var AmountAll = ((_excel.Range)range.Cells[row, 22]).Text;
                                                var FeeNominal = ((_excel.Range)range.Cells[row, 23]).Text;
                                                var FeeUnit = ((_excel.Range)range.Cells[row, 24]).Text;
                                                var FeePercent = ((_excel.Range)range.Cells[row, 25]).Text;
                                                var TransferPath = ((_excel.Range)range.Cells[row, 26]).Text;
                                                var REDMSequentialCode = ((_excel.Range)range.Cells[row, 27]).Text;
                                                var REDMBICCode = ((_excel.Range)range.Cells[row, 28]).Text;
                                                var REDMBIMemberCode = ((_excel.Range)range.Cells[row, 29]).Text;
                                                var REDMBankName = ((_excel.Range)range.Cells[row, 30]).Text;
                                                var REDMNo = ((_excel.Range)range.Cells[row, 31]).Text;
                                                var REDMName = ((_excel.Range)range.Cells[row, 32]).Text;
                                                var PaymentDate = new DateTime?();
                                                string paydate = ((_excel.Range)range.Cells[row, 33]).Text;
                                                if (paydate != "")
                                                {
                                                    var paydate2 = paydate.Substring(6, 2) + "/" + paydate.Substring(4, 2) + "/" + paydate.Substring(0, 4);
                                                    DateTime date2 = DateTime.ParseExact(paydate2, "d/MM/yyyy", CultureInfo.InvariantCulture);
                                                    PaymentDate = date2;
                                                }
                                                else
                                                {
                                                    PaymentDate = null;
                                                }
                                                var TransferType = ((_excel.Range)range.Cells[row, 34]).Text;
                                                var InputDate = new DateTime?();
                                                string indate = ((_excel.Range)range.Cells[row, 35]).Text;
                                                if (indate != "")
                                                {
                                                    var indate2 = indate.Substring(6, 2) + "/" + indate.Substring(4, 2) + "/" + indate.Substring(0, 4);
                                                    DateTime date3 = DateTime.ParseExact(indate2, "d/MM/yyyy", CultureInfo.InvariantCulture);
                                                    InputDate = date3;
                                                }
                                                else
                                                {
                                                    InputDate = null;
                                                }
                                                var UploadReference = ((_excel.Range)range.Cells[row, 36]).Text;
                                                var IMStatus = ((_excel.Range)range.Cells[row, 38]).Text;
                                                var CBStatus = ((_excel.Range)range.Cells[row, 39]).Text;
                                                var CBCompletionStatus = ((_excel.Range)range.Cells[row, 40]).Text;
                                                var CreateDate = DateTime.Now;
                                                var UserId = currentUser.Id;
                                                var MatchingId = 1;
                                                bool IsDelete = false;

                                                string sql = "INSERT INTO DataAplikasis (TransactionDate, TransactionType, ReferenceNo, Status, IMFeeAmendment, IMPaymentDate, SAId, InvestorFundUnitNo, InvestorFundUnitName, SID, FundId, MIId, CBCode, CBName, FundCCY, AmountNominal, AmountUnit, AmountAll, FeeNominal, FeeUnit, FeePercent, TransferPath, REDMSequentialCode, REDMBICCode, REDMBIMemberCode, REDMBankName, REDMNo, REDMName, PaymentDate, TransferType, InputDate, UploadReference, SAReference, IMStatus, CBStatus, CBCompletionStatus, CreateDate, UserId, MatchingId, IsDelete) VALUES (@TransactionDate, @TransactionType, @ReferenceNo, @Status, @IMFeeAmendment, @IMPaymentDate, @SAId, @InvestorFundUnitNo, @InvestorFundUnitName, @SID, @FundId, @MIId, @CBCode, @CBName, @FundCCY, @AmountNominal, @AmountUnit, @AmountAll, @FeeNominal, @FeeUnit, @FeePercent, @TransferPath, @REDMSequentialCode, @REDMBICCode, @REDMBIMemberCode, @REDMBankName, @REDMNo, @REDMName, @PaymentDate, @TransferType, @InputDate, @UploadReference, @SAReference, @IMStatus, @CBStatus, @CBCompletionStatus, @CreateDate, @UserId, @MatchingId, @IsDelete)";
                                                _con.Execute(sql, new { TransactionDate, TransactionType, ReferenceNo, Status, IMFeeAmendment, IMPaymentDate, SAId, InvestorFundUnitNo, InvestorFundUnitName, SID, FundId, MIId, CBCode, CBName, FundCCY, AmountNominal, AmountUnit, AmountAll, FeeNominal, FeeUnit, FeePercent, TransferPath, REDMSequentialCode, REDMBICCode, REDMBIMemberCode, REDMBankName, REDMNo, REDMName, PaymentDate, TransferType, InputDate, UploadReference, SAReference, IMStatus, CBStatus, CBCompletionStatus, CreateDate, UserId, MatchingId, IsDelete });

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
                                    else if (judul == "informasirekening-mutasirekening")
                                    {
                                        #region FundBCA

                                        int csuccess = 0, cfails = 0;
                                        string norek = ((_excel.Range)range.Cells[3, 1]).Text;

                                        if (norek == "Tidak ada rekening yang tersedia.") //Skip file
                                            continue;

                                        string namarek = ((_excel.Range)range.Cells[4, 1]).Text;
                                        string periode = ((_excel.Range)range.Cells[5, 1]).Text;
                                        string NoRek = norek.Substring(15);
                                        string NamaRek = namarek.Substring(7);
                                        int RekeningId = 0;

                                        #region CheckMasterRekening
                                        var resultRek = _con.QueryFirstOrDefault<Rekening>("SELECT Id FROM Rekenings WHERE NoRek = @NoRek AND NamaRek = @NamaRek", new { NoRek, NamaRek });
                                        if (resultRek == null)
                                        {

                                            var RekId = _con.QuerySingle<int>("INSERT INTO Rekenings (NoRek, NamaRek, CreateDate, UserId) OUTPUT INSERTED.Id VALUES (@NoRek, @NamaRek, @DateNow, @UserId);", new { NoRek = NoRek, NamaRek = NamaRek, DateNow = DateTime.Now, UserId = currentUser.Id });

                                            RekeningId = RekId;
                                        }
                                        else
                                        {
                                            RekeningId = resultRek.Id;
                                        }
                                        #endregion

                                        string Periode = periode.Substring(10);
                                        int tahunfirst = Convert.ToInt16(periode.Substring(16, 4));
                                        int tahunlast = Convert.ToInt16(periode.Substring((periode.Length - 4), 4));
                                        int blast = 0, bnow = 0, day = 0;
                                        string ccy = ((_excel.Range)range.Cells[6, 1]).Text;
                                        string currency = ccy.Substring(17, (ccy.Length - 17));

                                        #region ParsingFund
                                        for (int row = 8; row <= range.Rows.Count; row++)
                                        {
                                            string tanggal = (range.Cells[row, 1] as _excel.Range).Text;
                                            if (tanggal != "")
                                            {
                                                int cek;
                                                bool success = int.TryParse(tanggal.Substring(0, 1), out cek);
                                                if (success)
                                                {
                                                    if (tanggal.Length > 6)
                                                    {
                                                        #region FundSekolom
                                                        string data = (range.Cells[row, 1] as _excel.Range).Text;
                                                        string[] spearator = { ",\"" };
                                                        string[] strlist = data.Split(spearator, System.StringSplitOptions.RemoveEmptyEntries);

                                                        string jml = strlist[3].Replace("\"", "").Replace("\\", ""); ;
                                                        string crdb = jml.Substring((jml.Length - 2), 2);
                                                        if (crdb == "CR")
                                                        {
                                                            string Keterangan = strlist[1].Replace("\"", "").Replace("\\", "");
                                                            string jumlah = jml.Substring(0, (jml.Length - 6)).Replace(",", "");
                                                            Int64 Jumlah = Convert.ToInt64(jumlah);
                                                            var sld = strlist[4].Replace("\"", "").Replace("\\", "").Replace(",", "");
                                                            Int64 Saldo = Convert.ToInt64(sld.Substring(0, (sld.Length - 3)));

                                                            #region Tanggal
                                                            string date = strlist[0].Replace("\"", "").Replace("\\", "");
                                                            day = Convert.ToInt16(date.Substring(0, 2));
                                                            bnow = Convert.ToInt16(date.Substring(3, 2));

                                                            if (bnow >= blast)
                                                            {
                                                                tanggal = day.ToString().PadLeft(2, '0') + "/" + bnow.ToString().PadLeft(2, '0') + "/" + Convert.ToString(tahunfirst);
                                                            }
                                                            else
                                                            {
                                                                tahunfirst = tahunfirst + 1;
                                                                tanggal = day.ToString().PadLeft(2, '0') + "/" + bnow.ToString().PadLeft(2, '0') + "/" + Convert.ToString(tahunfirst);
                                                            }
                                                            blast = bnow;

                                                            DateTime TanggalFund = DateTime.ParseExact(tanggal, "d/MM/yyyy", CultureInfo.InvariantCulture);
                                                            #endregion

                                                            var checkFund = _con.QueryFirst<int>("SELECT COUNT(Id) FROM DataFunds WHERE RekeningId = @RekeningId AND Keterangan = @Keterangan AND Jumlah = @Jumlah AND Saldo = @Saldo AND Tanggal = @TanggalFund",
                                                                            new { RekeningId, Keterangan, Jumlah, Saldo, TanggalFund });

                                                            if (checkFund == 0)
                                                            {
                                                                var CCY = currency;
                                                                var Tanggal = TanggalFund;
                                                                var KeteranganDua = Keterangan.Replace(" ", "").Replace(",", "").Replace(".", "").Replace("'", "").ToUpper();
                                                                var CreateDate = DateTime.Now;
                                                                var UserId = currentUser.Id;
                                                                var MatchingId = 1;

                                                                string sql = "INSERT INTO DataFunds (RekeningId, CCY, Tanggal, Keterangan, KeteranganDua, Jumlah, Saldo, CreateDate, UserId, MatchingId) VALUES (@RekeningId, @CCY, @Tanggal, @Keterangan, @KeteranganDua, @Jumlah, @Saldo, @CreateDate, @UserId, @MatchingId)";
                                                                _con.Execute(sql, new { RekeningId, CCY, Tanggal, Keterangan, KeteranganDua, Jumlah, Saldo, CreateDate, UserId, MatchingId });

                                                                csuccess++;
                                                            }
                                                            else
                                                            {
                                                                cfails++;
                                                            }
                                                        }
                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        #region FundBedaKolom
                                                        string jml = ((_excel.Range)range.Cells[row, 4]).Text;
                                                        string crdb = jml.Substring((jml.Length - 2), 2);
                                                        if (crdb == "CR")
                                                        {
                                                            string Keterangan = ((_excel.Range)range.Cells[row, 2]).Text;
                                                            string jumlah = jml.Substring(0, (jml.Length - 6)).Replace(",", "");
                                                            Int64 Jumlah = Convert.ToInt64(jumlah);
                                                            var sld = ((_excel.Range)range.Cells[row, 5]).Value2;
                                                            Int64 Saldo = Convert.ToInt64(sld);

                                                            #region Tanggal
                                                            var formatTanggal = ((_excel.Range)range.Cells[row, 1]).NumberFormat;
                                                            var val = ((_excel.Range)range.Cells[row, 1]).Value;
                                                            var val2 = ((_excel.Range)range.Cells[row, 1]).Value2;
                                                            if (formatTanggal == "General")
                                                            {
                                                                string date = ((_excel.Range)range.Cells[row, 1]).Text;
                                                                day = Convert.ToInt16(date.Substring(0, 2));
                                                                bnow = Convert.ToInt16(date.Substring(3, 2));
                                                            }
                                                            else if (formatTanggal == "d-mmm")
                                                            {
                                                                //string textdate = ((_excel.Range)range.Cells[row, 1]).Text;
                                                                //DateTime d = DateTime.ParseExact(textdate, "d-MMM", CultureInfo.InvariantCulture);
                                                                ////string Text = Convert.ToString(d.Month) + "/" + Convert.ToString(d.Day);
                                                                //day = Convert.ToInt16(Convert.ToString(d.Day));
                                                                //bnow = Convert.ToInt16(Convert.ToString(d.Month));

                                                                DateTime textdate2 = ((_excel.Range)range.Cells[row, 1]).Value;

                                                                day = textdate2.Month;
                                                                bnow = textdate2.Day;
                                                            }
                                                            else
                                                            {
                                                                string Text = ((_excel.Range)range.Cells[row, 1]).Text;
                                                            }

                                                            if (bnow >= blast)
                                                            {
                                                                tanggal = day + "/" + bnow + "/" + Convert.ToString(tahunfirst);
                                                            }
                                                            else
                                                            {
                                                                tahunfirst = tahunfirst + 1;
                                                                tanggal = day + "/" + bnow + "/" + Convert.ToString(tahunfirst);
                                                            }
                                                            blast = bnow;

                                                            DateTime TanggalFund = DateTime.ParseExact(tanggal, "d/M/yyyy", CultureInfo.InvariantCulture);
                                                            #endregion

                                                            var checkFund = _con.QueryFirst<int>("SELECT COUNT(Id) FROM DataFunds WHERE RekeningId = @RekeningId AND Keterangan = @Keterangan AND Jumlah = @Jumlah AND Saldo = @Saldo AND Tanggal = @TanggalFund",
                                                                                new { RekeningId, Keterangan, Jumlah, Saldo, TanggalFund });

                                                            if (checkFund == 0)
                                                            {
                                                                var CCY = currency;
                                                                var Tanggal = TanggalFund;
                                                                var KeteranganDua = Keterangan.Replace(" ", "").Replace(",", "").Replace(".", "").Replace("'", "").ToUpper();
                                                                var CreateDate = DateTime.Now;
                                                                var UserId = currentUser.Id;
                                                                var MatchingId = 1;

                                                                string sql = "INSERT INTO DataFunds (RekeningId, CCY, Tanggal, Keterangan, KeteranganDua, Jumlah, Saldo, CreateDate, UserId, MatchingId) VALUES (@RekeningId, @CCY, @Tanggal, @Keterangan, @KeteranganDua, @Jumlah, @Saldo, @CreateDate, @UserId, @MatchingId)";
                                                                _con.Execute(sql, new { RekeningId, CCY, Tanggal, Keterangan, KeteranganDua, Jumlah, Saldo, CreateDate, UserId, MatchingId });

                                                                csuccess++;
                                                            }
                                                            else
                                                            {
                                                                cfails++;
                                                            }
                                                        }
                                                        #endregion
                                                    }
                                                }
                                            }
                                        }

                                        resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Success", warna = "success", success = csuccess, fails = cfails });
                                        #endregion
                                        #endregion
                                    }
                                    else if (judul == "postdate" && judul2 == "valuedate" && judul3 == "branch")
                                    {
                                        #region FundBNI
                                        int csuccess = 0, cfails = 0;
                                        var rek = file.FileName;
                                        char[] spearator = { ' ', '.' };
                                        string[] strlist = rek.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                                        var NoRek = strlist[0];
                                        int RekeningId = 0;

                                        #region CheckMasterRekening
                                        var resultRek = _con.QueryFirstOrDefault<Rekening>("SELECT Id FROM Rekenings WHERE NoRek = @NoRek", new { NoRek });

                                        if (resultRek == null)
                                        {
                                            var RekId = _con.QuerySingle<int>("INSERT INTO Rekenings (NoRek, NamaRek, CreateDate, UserId) OUTPUT INSERTED.Id VALUES (@NoRek, @NamaRek, @DateNow, @UserId);", new { NoRek = NoRek, NamaRek = file.FileName, DateNow = DateTime.Now, UserId = currentUser.Id });

                                            RekeningId = RekId;
                                        }
                                        else
                                        {
                                            RekeningId = resultRek.Id;
                                        }
                                        #endregion

                                        for (int row = 2; row <= range.Rows.Count; row++)
                                        {
                                            var sld = ((_excel.Range)range.Cells[row, 7]).Value2;
                                            if (sld != 0)
                                            {
                                                long Saldo = Convert.ToInt64(sld);
                                                string tanggal = ((_excel.Range)range.Cells[row, 2]).Text;
                                                DateTime TanggalFund = DateTime.ParseExact(tanggal, "d/MM/yy HH.mm.ss", CultureInfo.InvariantCulture);
                                                string Keterangan = ((_excel.Range)range.Cells[row, 5]).Text;

                                                var check = _con.QueryFirst<int>("SELECT COUNT(Id) FROM DataFunds WHERE RekeningId = @RekeningId AND Jumlah = @Saldo AND Tanggal = @TanggalFund AND Keterangan = @Keterangan",
                                                                new { RekeningId, Saldo, TanggalFund, Keterangan });

                                                if (check == 0)
                                                {
                                                    var Tanggal = TanggalFund;
                                                    var CCY = "Rp";
                                                    var KeteranganDua = Keterangan.Replace(" ", "").Replace(",", "").Replace(".", "").Replace("'", "").ToUpper();
                                                    var CreateDate = DateTime.Now;
                                                    var UserId = currentUser.Id;
                                                    var MatchingId = 1;

                                                    string sql = "INSERT INTO DataFunds (RekeningId, CCY, Tanggal, Keterangan, KeteranganDua, Jumlah, Saldo, CreateDate, UserId, MatchingId) VALUES (@RekeningId, @CCY, @Tanggal, @Keterangan, @KeteranganDua, @Jumlah, @Saldo, @CreateDate, @UserId, @MatchingId)";
                                                    _con.Execute(sql, new { RekeningId, CCY, Tanggal, Keterangan, KeteranganDua, Jumlah = Saldo, Saldo, CreateDate, UserId, MatchingId }); //Nilai Jumlah == Saldo

                                                    csuccess++;
                                                }
                                                else
                                                {
                                                    cfails++;
                                                }
                                            }
                                        }

                                        resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Success", warna = "success", success = csuccess, fails = cfails });
                                        #endregion
                                    }
                                    else if (judul == "accountno" && judul2 == "date" && judul9 == "credit")
                                    {
                                        #region FundMandiri
                                        int csuccess = 0, cfails = 0;
                                        var rek = file.FileName;
                                        char[] spearator = { ' ' };
                                        string[] strlist = rek.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                                        var NoRek = strlist[0];
                                        int RekeningId = 0;

                                        #region CheckMasterRekening
                                        var resultRek = _con.QueryFirstOrDefault<Rekening>("SELECT Id FROM Rekenings WHERE Norek = @Norek", new { NoRek });

                                        if (resultRek == null)
                                        {
                                            var RekId = _con.QuerySingle<int>("INSERT INTO Rekenings (NoRek, NamaRek, CreateDate, UserId) OUTPUT INSERTED.Id VALUES (@NoRek, @NamaRek, @DateNow, @UserId);", new { NoRek = NoRek, NamaRek = file.FileName, DateNow = DateTime.Now, UserId = currentUser.Id });

                                            RekeningId = RekId;
                                        }
                                        else
                                        {
                                            RekeningId = resultRek.Id;
                                        }
                                        #endregion

                                        for (int row = 2; row <= range.Rows.Count; row++)
                                        {
                                            var sld = ((_excel.Range)range.Cells[row, 9]).Value2;
                                            if (sld != 0)
                                            {
                                                Int64 Saldo = Convert.ToInt64(sld);
                                                string Keterangan = ((_excel.Range)range.Cells[row, 5]).Text;

                                                #region Tanggal
                                                DateTime TanggalFund = DateTime.Now;
                                                var formatTanggal = ((_excel.Range)range.Cells[row, 2]).NumberFormat;
                                                if (formatTanggal == "General")
                                                {
                                                    string tanggal = ((_excel.Range)range.Cells[row, 2]).Value;
                                                    string[] tanggalSplit = tanggal.Split('/');
                                                    if (tanggalSplit[2].Length == 4)
                                                    {
                                                        TanggalFund = DateTime.ParseExact(tanggal, "d/MM/yyyy", CultureInfo.InvariantCulture);
                                                    }
                                                    else
                                                    {
                                                        TanggalFund = DateTime.ParseExact(tanggal, "d/MM/yy", CultureInfo.InvariantCulture);
                                                    }
                                                }
                                                else if (formatTanggal == "m/d/yyyy")
                                                {
                                                    TanggalFund = ((_excel.Range)range.Cells[row, 2]).Value;
                                                    int day = Convert.ToInt16(Convert.ToString(TanggalFund.Month));
                                                    int month = Convert.ToInt16(Convert.ToString(TanggalFund.Day));
                                                    int year = Convert.ToInt16(Convert.ToString(TanggalFund.Year));
                                                    string tanggal = day + "/" + month + "/" + year;
                                                    TanggalFund = DateTime.ParseExact(tanggal, "d/M/yyyy", CultureInfo.InvariantCulture);
                                                }
                                                #endregion

                                                var check = _con.QueryFirst<int>("SELECT COUNT(Id) FROM DataFunds WHERE RekeningId = @RekeningId AND Jumlah = @Saldo AND Keterangan = @Keterangan AND Tanggal = @TanggalFund",
                                                                    new { RekeningId, Saldo, Keterangan, TanggalFund });
                                                if (check == 0)
                                                {
                                                    var Tanggal = TanggalFund;
                                                    var CCY = "Rp";
                                                    var KeteranganDua = Keterangan.Replace(" ", "").Replace(",", "").Replace(".", "").Replace("'", "").ToUpper();
                                                    var CreateDate = DateTime.Now;
                                                    var UserId = currentUser.Id;
                                                    var MatchingId = 1;

                                                    string sql = "INSERT INTO DataFunds (RekeningId, CCY, Tanggal, Keterangan, KeteranganDua, Jumlah, Saldo, CreateDate, UserId, MatchingId) VALUES (@RekeningId, @CCY, @Tanggal, @Keterangan, @KeteranganDua, @Jumlah, @Saldo, @CreateDate, @UserId, @MatchingId)";
                                                    _con.Execute(sql, new { RekeningId, CCY, Tanggal, Keterangan, KeteranganDua, Jumlah = Saldo, Saldo, CreateDate, UserId, MatchingId }); //Nilai Jumlah == Saldo

                                                    csuccess++;
                                                }
                                                else
                                                {
                                                    cfails++;
                                                }
                                            }
                                        }

                                        resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Success", warna = "success", success = csuccess, fails = cfails });
                                        #endregion
                                    }
                                    else if (judul == "period:")
                                    {
                                        #region FundOCBC
                                        string judulocbc = ((_excel.Range)range.Cells[11, 1]).Text;
                                        string judulocbc2 = ((_excel.Range)range.Cells[11, 2]).Text;
                                        string judulocbc7 = ((_excel.Range)range.Cells[11, 7]).Text;
                                        judulocbc = judulocbc.Replace(" ", "").ToLower();
                                        judulocbc2 = judulocbc2.Replace(" ", "").ToLower();
                                        judulocbc7 = judulocbc7.Replace(" ", "").ToLower();

                                        if (judulocbc == "transactiondate" && judulocbc2 == "valuedate" && judulocbc7 == "credit")
                                        {
                                            #region FundOCBC
                                            int csuccess = 0, cfails = 0;
                                            char[] spearator = { ' ' };
                                            char[] spearator2 = { ',' };
                                            string norek = ((_excel.Range)range.Cells[2, 2]).Text;
                                            string namarek = ((_excel.Range)range.Cells[3, 2]).Text;
                                            string[] noreklist = norek.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                                            string[] namareklist = namarek.Split(spearator2, StringSplitOptions.RemoveEmptyEntries);

                                            string NoRek = noreklist[0];
                                            string NamaRek = namareklist[0];
                                            int RekeningId = 0;

                                            #region CheckMasterRekening
                                            var resultRek = _con.QueryFirstOrDefault<Rekening>("SELECT Id FROM Rekenings WHERE NoRek = @NoRek AND NamaRek = @NamaRek", new { NoRek, NamaRek });

                                            if (resultRek == null)
                                            {
                                                var RekId = _con.QuerySingle<int>("INSERT INTO Rekenings (NoRek, NamaRek, CreateDate, UserId) OUTPUT INSERTED.Id VALUES (@NoRek, @NamaRek, @DateNow, @UserId);", new { NoRek = NoRek, NamaRek = NamaRek, DateNow = DateTime.Now, UserId = currentUser.Id });

                                                RekeningId = RekId;
                                            }
                                            else
                                            {
                                                RekeningId = resultRek.Id;
                                            }
                                            #endregion

                                            for (int row = 12; row <= range.Rows.Count; row++)
                                            {
                                                var jml = ((_excel.Range)range.Cells[row, 7]).Value2;
                                                var sld = ((_excel.Range)range.Cells[row, 8]).Value2;
                                                if (jml != 0)
                                                {
                                                    Int64 Jumlah = Convert.ToInt64(jml);
                                                    Int64 Saldo = Convert.ToInt64(sld);
                                                    string Keterangan = ((_excel.Range)range.Cells[row, 5]).Text;

                                                    var check = _con.QueryFirst<int>("SELECT COUNT(Id) FROM DataFunds WHERE RekeningId = @RekeningId AND Jumlah = @Jumlah AND Saldo = @Saldo AND Keterangan = @Keterangan", new { RekeningId, Jumlah, Saldo, Keterangan });
                                                    if (check == 0)
                                                    {
                                                        DateTime date = DateTime.Now;
                                                        var formatTanggal = ((_excel.Range)range.Cells[row, 2]).NumberFormat;
                                                        if (formatTanggal == "General")
                                                        {
                                                            var tanggal = ((_excel.Range)range.Cells[row, 2]).Value;
                                                            date = DateTime.ParseExact(tanggal, "d/MM/yyyy", CultureInfo.InvariantCulture);
                                                        }
                                                        else if (formatTanggal == "m/d/yyyy")
                                                        {
                                                            date = ((_excel.Range)range.Cells[row, 2]).Value;
                                                            int day = Convert.ToInt16(Convert.ToString(date.Month));
                                                            int month = Convert.ToInt16(Convert.ToString(date.Day));
                                                            int year = Convert.ToInt16(Convert.ToString(date.Year));
                                                            string tanggal = day + "/" + month + "/" + year;
                                                            date = DateTime.ParseExact(tanggal, "d/M/yyyy", CultureInfo.InvariantCulture);
                                                        }

                                                        var Tanggal = date;
                                                        var CCY = "Rp";
                                                        var KeteranganDua = Keterangan.Replace(" ", "").Replace(",", "").Replace(".", "").Replace("'", "").ToUpper();
                                                        var CreateDate = DateTime.Now;
                                                        var UserId = currentUser.Id;
                                                        var MatchingId = 1;

                                                        string sql = "INSERT INTO DataFunds (RekeningId, CCY, Tanggal, Keterangan, KeteranganDua, Jumlah, Saldo, CreateDate, UserId, MatchingId) VALUES (@RekeningId, @CCY, @Tanggal, @Keterangan, @KeteranganDua, @Jumlah, @Saldo, @CreateDate, @UserId, @MatchingId)";
                                                        _con.Execute(sql, new { RekeningId, CCY, Tanggal, Keterangan, KeteranganDua, Jumlah, Saldo, CreateDate, UserId, MatchingId });

                                                        csuccess++;
                                                    }
                                                    else
                                                    {
                                                        cfails++;
                                                    }
                                                }
                                                else
                                                {
                                                    cfails++;
                                                }
                                            }

                                            resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Success", warna = "success", success = csuccess, fails = cfails });
                                            #endregion
                                        }
                                        else
                                        {
                                            resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Fails", warna = "danger", success = 0, fails = 0 });
                                        }
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

                                    //Marshal.ReleaseComObject(workbook);
                                    //Marshal.ReleaseComObject(workbooks);
                                    //Marshal.ReleaseComObject(application);
                                    GC.Collect();
                                    GC.WaitForPendingFinalizers();
                                }


                                //GC.Collect();
                                //GC.WaitForPendingFinalizers();
                            }
                            else if (file.FileName.EndsWith("txt") || file.FileName.EndsWith("TXT"))
                            {
                                #region FundDanamon
                                string path = Server.MapPath("~/Content/Files/" + file.FileName);
                                file.SaveAs(path);
                                var lines = System.IO.File.ReadLines(path);
                                List<DataFund> dataFunds = new List<DataFund>();

                                int tahunfirst = 0, tahunlast = 0;
                                int blast = 0, bnow = 0, day = 0;
                                int csuccess = 0, cfails = 0;
                                int RekeningId = 0;
                                string tanggal;
                                string ccy = "";
                                foreach (var line in lines)
                                {
                                    var check = line.Substring(0, 1);
                                    if (check == "H")
                                    {
                                        var NoRek = line.Substring(7, 12).Replace(" ", "");
                                        ccy = line.Substring(19, 3);

                                        #region CheckMasterRekening
                                        var resultRek = _con.QueryFirstOrDefault<Rekening>("SELECT Id FROM Rekenings WHERE NoRek = @NoRek", new { NoRek });

                                        if (resultRek == null)
                                        {
                                            var NamaRek = line.Substring(22, 31);

                                            var RekId = _con.QuerySingle<int>("INSERT INTO Rekenings (NoRek, NamaRek, CreateDate, UserId) OUTPUT INSERTED.Id VALUES (@NoRek, @NamaRek, @DateNow, @UserId);", new { NoRek = NoRek, NamaRek = NamaRek, DateNow = DateTime.Now, UserId = currentUser.Id });

                                            RekeningId = RekId;
                                        }
                                        else
                                        {
                                            RekeningId = resultRek.Id;
                                        }
                                        #endregion

                                        tahunfirst = Convert.ToInt16(line.Substring(73, 2));
                                        tahunlast = Convert.ToInt16(line.Substring(79, 2));
                                    }
                                    else if (check == "D")
                                    {
                                        if (ccy == "IDR")
                                        {
                                            Int64 Jumlah = Convert.ToInt64(line.Substring(120, 13));
                                            Int64 Saldo = Convert.ToInt64(line.Substring(137, 13));
                                            if (Jumlah != 0)
                                            {
                                                string Keterangan = line.Substring(44, 60);
                                                day = Convert.ToInt16(line.Substring(6, 2));
                                                bnow = Convert.ToInt16(line.Substring(8, 2));

                                                if (bnow >= blast)
                                                {
                                                    tanggal = day + "/" + bnow + "/" + tahunfirst;
                                                }
                                                else
                                                {
                                                    tahunfirst = tahunfirst + 1;
                                                    tanggal = day + "/" + bnow + "/" + tahunfirst;
                                                }
                                                blast = bnow;
                                                DateTime TanggalFund = DateTime.ParseExact(tanggal, "d/M/yy", CultureInfo.InvariantCulture);

                                                var checkFund = _con.QueryFirst<int>("SELECT COUNT(Id) FROM DataFunds WHERE RekeningId = @RekeningId AND Jumlah = @Jumlah AND Saldo = @Saldo AND Keterangan = @Keterangan AND Tanggal = @TanggalFund",
                                                                        new { RekeningId, Jumlah, Saldo, Keterangan, TanggalFund });
                                                if (checkFund == 0)
                                                {
                                                    var Tanggal = TanggalFund;
                                                    var CCY = "Rp";
                                                    var KeteranganDua = Keterangan.Replace(" ", "").Replace(",", "").Replace(".", "").Replace("'", "").ToUpper();
                                                    var CreateDate = DateTime.Now;
                                                    var UserId = currentUser.Id;
                                                    var MatchingId = 1;

                                                    string sql = "INSERT INTO DataFunds (RekeningId, CCY, Tanggal, Keterangan, KeteranganDua, Jumlah, Saldo, CreateDate, UserId, MatchingId) VALUES (@RekeningId, @CCY, @Tanggal, @Keterangan, @KeteranganDua, @Jumlah, @Saldo, @CreateDate, @UserId, @MatchingId)";
                                                    _con.Execute(sql, new { RekeningId, CCY, Tanggal, Keterangan, KeteranganDua, Jumlah, Saldo, CreateDate, UserId, MatchingId });

                                                    csuccess++;
                                                }
                                                else
                                                {
                                                    cfails++;
                                                }
                                            }
                                            else
                                            {
                                                cfails++;
                                            }
                                        }
                                        else
                                        {
                                            cfails++;
                                        }
                                    }
                                }

                                System.IO.File.Delete(path);
                                resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Success", warna = "success", success = csuccess, fails = cfails });
                                #endregion
                            }
                            else
                            {
                                resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Fails", warna = "danger", success = 0, fails = 0 });
                            }
                        }
                    }
                    #endregion                    

                    //#region Rekons SAReference
                    //var getAplikasi = _con.Query<DataAplikasi>("SELECT Id, SAReference, SAId, FundId, MIId, AmountNominal FROM DataAplikasis WHERE MatchingId = @MatchingId", new { MatchingId = 1 });

                    //var insertTrans = "INSERT INTO Transaksis (MatchingId, CreateDate, InputerId, IsDelete) OUTPUT INSERTED.Id VALUES (@MatchingId, @DateNow, @UserId, @IsDelete)";
                    //var insertDataApl = "INSERT INTO TrDataAplikasis (DataAplikasiId, CreateDate, TransaksiId) VALUES (@DataAplikasiId, @DateNow, @TransaksiId)";
                    //var insertDataFund = "INSERT INTO TrDataFunds (DataFundId, CreateDate, TransaksiId) VALUES (@DataFundId, @DateNow, @TransaksiId)";
                    //var updateDataApl = "UPDATE DataAplikasis SET MatchingId = @MatchingId, UpdateDate = @UpdateDate WHERE Id = @AppId";
                    //var updateDataFund = "UPDATE DataFunds SET MatchingId = @MatchingId, UpdateDate = @UpdateDate WHERE Id = @FundId";

                    //foreach (var app in getAplikasi)
                    //{
                    //    #region SAReference
                    //    var saRef = app.SAReference;
                    //    if (saRef != "" && saRef != null)
                    //    {
                    //        #region Search dengan SA Full Karakter
                    //        if (saRef.Contains("_") == true)
                    //        {
                    //            char[] spearator = { '_' };
                    //            string[] strlist = saRef.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                    //            saRef = strlist[0];
                    //        }

                    //        var getFund = _con.QueryFirstOrDefault<DataFund>("SELECT df.* FROM DataFunds df JOIN Rekenings rk on df.RekeningId = rk.Id WHERE df.MatchingId = @MatchingId AND rk.SAId = @SAId AND rk.FundId = @FundId AND rk.MIId = @MIId AND df.Jumlah = @Jumlah AND df.Keterangan LIKE '%' + @Keterangan + '%'",
                    //                        new { MatchingId = 1, SAId = app.SAId, FundId = app.FundId, MIId = app.MIId, Jumlah = app.AmountNominal, Keterangan = saRef });
                    //        #endregion

                    //        #region Search SA dipotong satu huruf
                    //        if (getFund == null)
                    //        {
                    //            int length = saRef.Length - 1;
                    //            saRef = saRef.Substring(0, length);

                    //            getFund = _con.QueryFirstOrDefault<DataFund>("SELECT df.* FROM DataFunds df JOIN Rekenings rk on df.RekeningId = rk.Id WHERE df.MatchingId = @MatchingId AND rk.SAId = @SAId AND rk.FundId = @FundId AND rk.MIId = @MIId AND df.Jumlah = @Jumlah AND df.Keterangan LIKE '%' + @Keterangan + '%'",
                    //                        new { MatchingId = 1, SAId = app.SAId, FundId = app.FundId, MIId = app.MIId, Jumlah = app.AmountNominal, Keterangan = saRef });
                    //        }
                    //        #endregion

                    //        if (getFund != null)
                    //        {
                    //            _con.Open();
                    //            using (var transaction = _con.BeginTransaction())
                    //            {
                    //                var TransId = _con.QuerySingle<int>(insertTrans, new { MatchingId = 2, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false }, transaction: transaction);

                    //                _con.Execute(insertDataApl, new { DataAplikasiId = app.Id, DateNow = DateTime.Now, TransaksiId = TransId }, transaction: transaction);
                    //                _con.Execute(insertDataFund, new { DataFundId = getFund.Id, DateNow = DateTime.Now, TransaksiId = TransId }, transaction: transaction);

                    //                _con.Execute(updateDataApl, new { MatchingId = 2, UpdateDate = DateTime.Now, AppId = app.Id }, transaction: transaction);
                    //                _con.Execute(updateDataFund, new { MatchingId = 2, UpdateDate = DateTime.Now, FundId = getFund.Id }, transaction: transaction);

                    //                transaction.Commit();
                    //            }
                    //            _con.Close();
                    //        }
                    //    }
                    //    #endregion
                    //}
                    //#endregion

                    //#region Rekons Nama
                    //var getAplikasiDua = _con.Query<DataAplikasi>("SELECT * FROM DataAplikasis WHERE MatchingId = @MatchingId", new { MatchingId = 1 });

                    //foreach (var appdua in getAplikasiDua)
                    //{
                    //    #region Name
                    //    var namaapp = appdua.InvestorFundUnitName.Replace(" ", "").Replace(",", "").Replace(".", "").Replace("'", "").ToUpper();
                    //    if (appdua.SAId == 1)
                    //    {
                    //        if (namaapp.Length >= 10)
                    //        {
                    //            namaapp = namaapp.Substring(0, 9);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (namaapp.Length >= 12)
                    //        {
                    //            namaapp = namaapp.Substring(0, 11);
                    //        }
                    //    }

                    //    var getFund2 = _con.QueryFirstOrDefault<DataFund>("SELECT df.* FROM DataFunds df JOIN Rekenings rk on df.RekeningId = rk.Id WHERE df.MatchingId = @MatchingId AND rk.SAId = @SAId AND rk.FundId = @FundId AND rk.MIId = @MIId AND df.Jumlah = @Jumlah AND df.KeteranganDua LIKE '%' + @KeteranganDua + '%'",
                    //                    new { MatchingId = 1, SAId = appdua.SAId, FundId = appdua.FundId, MIId = appdua.MIId, Jumlah = appdua.AmountNominal, KeteranganDua = namaapp });

                    //    if (getFund2 != null)
                    //    {
                    //        _con.Open();
                    //        using (var transaction = _con.BeginTransaction())
                    //        {
                    //            var TransId = _con.QuerySingle<int>(insertTrans, new { MatchingId = 3, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false }, transaction: transaction);
                    //            _con.Execute(insertDataApl, new { DataAplikasiId = appdua.Id, DateNow = DateTime.Now, TransaksiId = TransId }, transaction: transaction);
                    //            _con.Execute(insertDataFund, new { DataFundId = getFund2.Id, DateNow = DateTime.Now, TransaksiId = TransId }, transaction: transaction);

                    //            _con.Execute(updateDataApl, new { MatchingId = 3, UpdateDate = DateTime.Now, AppId = appdua.Id }, transaction: transaction);
                    //            _con.Execute(updateDataFund, new { MatchingId = 3, UpdateDate = DateTime.Now, FundId = getFund2.Id }, transaction: transaction);

                    //            transaction.Commit();
                    //        }
                    //        _con.Close();
                    //    }
                    //    #endregion
                    //}
                    //#endregion
                }

                return Json(new { ResultUpload = resultupload }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        public void Rekon()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());
            
            #region Rekons SAReference
            var getAplikasi = _con.Query<DataAplikasi>("SELECT Id, SAReference, SAId, FundId, MIId, AmountNominal FROM DataAplikasis WHERE MatchingId = @MatchingId", new { MatchingId = 1 });

            var insertTrans = "INSERT INTO Transaksis (MatchingId, CreateDate, InputerId, IsDelete) OUTPUT INSERTED.Id VALUES (@MatchingId, @DateNow, @UserId, @IsDelete)";
            var insertDataApl = "INSERT INTO TrDataAplikasis (DataAplikasiId, CreateDate, TransaksiId) VALUES (@DataAplikasiId, @DateNow, @TransaksiId)";
            var insertDataFund = "INSERT INTO TrDataFunds (DataFundId, CreateDate, TransaksiId) VALUES (@DataFundId, @DateNow, @TransaksiId)";
            var updateDataApl = "UPDATE DataAplikasis SET MatchingId = @MatchingId, UpdateDate = @UpdateDate WHERE Id = @AppId";
            var updateDataFund = "UPDATE DataFunds SET MatchingId = @MatchingId, UpdateDate = @UpdateDate WHERE Id = @FundId";

            foreach (var app in getAplikasi)
            {
                #region SAReferenceS
                var saRef = app.SAReference;
                if (saRef != "" && saRef != null)
                {
                    #region Search dengan SA Full Karakter
                    if (saRef.Contains("_") == true)
                    {
                        char[] spearator = { '_' };
                        string[] strlist = saRef.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                        saRef = strlist[0];
                    }

                    var getFund = _con.QueryFirstOrDefault<DataFund>("SELECT df.* FROM DataFunds df JOIN Rekenings rk on df.RekeningId = rk.Id WHERE df.MatchingId = @MatchingId AND rk.SAId = @SAId AND rk.FundId = @FundId AND rk.MIId = @MIId AND df.Jumlah = @Jumlah AND df.Keterangan LIKE '%' + @Keterangan + '%'",
                                    new { MatchingId = 1, SAId = app.SAId, FundId = app.FundId, MIId = app.MIId, Jumlah = app.AmountNominal, Keterangan = saRef });
                    #endregion

                    #region Search SA dipotong satu huruf
                    if (getFund == null)
                    {
                        int length = saRef.Length - 1;
                        saRef = saRef.Substring(0, length);

                        getFund = _con.QueryFirstOrDefault<DataFund>("SELECT df.* FROM DataFunds df JOIN Rekenings rk on df.RekeningId = rk.Id WHERE df.MatchingId = @MatchingId AND rk.SAId = @SAId AND rk.FundId = @FundId AND rk.MIId = @MIId AND df.Jumlah = @Jumlah AND df.Keterangan LIKE '%' + @Keterangan + '%'",
                                    new { MatchingId = 1, SAId = app.SAId, FundId = app.FundId, MIId = app.MIId, Jumlah = app.AmountNominal, Keterangan = saRef });
                    }
                    #endregion

                    if (getFund != null)
                    {
                        _con.Open();
                        using (var transaction = _con.BeginTransaction())
                        {
                            var TransId = _con.QuerySingle<int>(insertTrans, new { MatchingId = 2, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false }, transaction: transaction);

                            _con.Execute(insertDataApl, new { DataAplikasiId = app.Id, DateNow = DateTime.Now, TransaksiId = TransId }, transaction: transaction);
                            _con.Execute(insertDataFund, new { DataFundId = getFund.Id, DateNow = DateTime.Now, TransaksiId = TransId }, transaction: transaction);

                            _con.Execute(updateDataApl, new { MatchingId = 2, UpdateDate = DateTime.Now, AppId = app.Id }, transaction: transaction);
                            _con.Execute(updateDataFund, new { MatchingId = 2, UpdateDate = DateTime.Now, FundId = getFund.Id }, transaction: transaction);

                            transaction.Commit();
                        }
                        _con.Close();
                    }
                }
                #endregion
            }
            #endregion

            #region Rekons Nama
            var getAplikasiDua = _con.Query<DataAplikasi>("SELECT * FROM DataAplikasis WHERE MatchingId = @MatchingId", new { MatchingId = 1 });

            foreach (var appdua in getAplikasiDua)
            {
                #region Name
                var namaapp = appdua.InvestorFundUnitName.Replace(" ", "").Replace(",", "").Replace(".", "").Replace("'", "").ToUpper();
                if (appdua.SAId == 1)
                {
                    if (namaapp.Length >= 10)
                    {
                        namaapp = namaapp.Substring(0, 9);
                    }
                }
                else
                {
                    if (namaapp.Length >= 12)
                    {
                        namaapp = namaapp.Substring(0, 11);
                    }
                }

                var getFund2 = _con.QueryFirstOrDefault<DataFund>("SELECT df.* FROM DataFunds df JOIN Rekenings rk on df.RekeningId = rk.Id WHERE df.MatchingId = @MatchingId AND rk.SAId = @SAId AND rk.FundId = @FundId AND rk.MIId = @MIId AND df.Jumlah = @Jumlah AND df.KeteranganDua LIKE '%' + @KeteranganDua + '%'",
                                new { MatchingId = 1, SAId = appdua.SAId, FundId = appdua.FundId, MIId = appdua.MIId, Jumlah = appdua.AmountNominal, KeteranganDua = namaapp });

                if (getFund2 != null)
                {
                    _con.Open();
                    using (var transaction = _con.BeginTransaction())
                    {
                        var TransId = _con.QuerySingle<int>(insertTrans, new { MatchingId = 3, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false }, transaction: transaction);
                        _con.Execute(insertDataApl, new { DataAplikasiId = appdua.Id, DateNow = DateTime.Now, TransaksiId = TransId }, transaction: transaction);
                        _con.Execute(insertDataFund, new { DataFundId = getFund2.Id, DateNow = DateTime.Now, TransaksiId = TransId }, transaction: transaction);

                        _con.Execute(updateDataApl, new { MatchingId = 3, UpdateDate = DateTime.Now, AppId = appdua.Id }, transaction: transaction);
                        _con.Execute(updateDataFund, new { MatchingId = 3, UpdateDate = DateTime.Now, FundId = getFund2.Id }, transaction: transaction);

                        transaction.Commit();
                    }
                    _con.Close();
                }
                #endregion
            }
            #endregion
        }
    }
}