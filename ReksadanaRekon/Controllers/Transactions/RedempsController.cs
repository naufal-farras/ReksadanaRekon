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
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using _excel = Microsoft.Office.Interop.Excel;

namespace ReksadanaRekon.Controllers.Transactions
{
    public class RedempsController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["ReksadanaRekon"].ToString());

        // GET: Reds
        public ActionResult Index()
        {
            return View();
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
                                #region Init
                                _excel.Application application = new _excel.Application(); ;
                                _excel.Workbooks workbooks = application.Workbooks;
                                _excel.Workbook workbook = null;
                                _excel.Worksheet worksheet = null;
                                _excel.Range range_ = null;
                                string path = path = Server.MapPath("~/Content/Files/" + file.FileName);

                                //try
                                //{
                                file.SaveAs(path);

                                workbook = workbooks.Open(path);
                                worksheet = workbook.ActiveSheet;
                                range_ = worksheet.UsedRange;

                                string judul = ((_excel.Range)range_.Cells[1, 1]).Text;
                                string judul2 = ((_excel.Range)range_.Cells[1, 2]).Text;
                                string judul3 = ((_excel.Range)range_.Cells[1, 3]).Text;
                                string judul4 = ((_excel.Range)range_.Cells[8, 1]).Text;
                                string judul5 = ((_excel.Range)range_.Cells[8, 2]).Text;
                                judul = judul.ToLower().Replace(" ", "").Replace("\"", "").Replace("\\", "").Replace(",", "");
                                judul2 = judul2.Replace(" ", "").ToLower();
                                judul3 = judul3.Replace(" ", "").ToLower();
                                judul4 = judul4.Replace(" ", "").ToLower();
                                judul5 = judul5.Replace(" ", "").ToLower();
                                #endregion

                                if (judul == "nomorreferensi" && judul2 == "rekeningdebet" && judul3 == "namapengirim")
                                {
                                    #region Aplikasi Red / Swi - Kliring
                                    int csuccess = 0, cfails = 0;
                                    var rek = file.FileName;
                                    int RekeningId = 0;

                                    var list = new List<object>();
                                    var GetMIs = _con.Query<MI>("SELECT Id, Code FROM MIs");
                                    var GetFunds = _con.Query<Fund>("SELECT Id, Code FROM Funds");

                                    string[] getDate = rek.Replace(" ", String.Empty).Split('-');
                                    DateTime TanggalFund = DateTime.ParseExact(getDate[1].ToLower().Replace(".xls", "").Replace(".xlsx", ""), "ddMMyyyy", CultureInfo.InvariantCulture);

                                    for (int row = 2; row < range_.Rows.Count + 1; row++)
                                    {

                                        int MIId = 0, FundId = 0;

                                        string NoRek = ((_excel.Range)range_.Cells[row, 2]).Text;

                                        if (NoRek == "")
                                            continue;

                                        string noref = ((_excel.Range)range_.Cells[row, 1]).Text;
                                        Int64 payamount = Convert.ToInt64(((_excel.Range)range_.Cells[row, 5]).Value);
                                        string message = ((_excel.Range)range_.Cells[row, 6]).Text;
                                        string receiver_name = ((_excel.Range)range_.Cells[row, 9]).Text;


                                        #region CheckMasterRekening
                                        var resultRek = _con.QueryFirstOrDefault<Rekening>("SELECT Id FROM Rekenings WHERE NoRek = @NoRek", new { NoRek });
                                        if (resultRek == null)
                                        {

                                            var RekId = _con.QuerySingle<int>("INSERT INTO Rekenings (NoRek, CreateDate, UserId, IsDelete) OUTPUT INSERTED.Id VALUES (@NoRek, @DateNow, @UserId, @IsDelete);", new { NoRek = NoRek, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false });

                                            RekeningId = RekId;
                                        }
                                        else
                                        {
                                            RekeningId = resultRek.Id;
                                        }
                                        #endregion

                                        #region CheckMasterMI
                                        var GetMIId = 0;

                                        var GetMICode = "";

                                        foreach (var item in GetMIs)
                                        {
                                            var a = message.IndexOf(item.Code);

                                            if (a != -1)
                                            {
                                                GetMIId = item.Id;
                                                break;
                                            }
                                        }

                                        if (GetMIId == 0)
                                        {
                                            var GetNewMI = message.Substring(5, message.Length);

                                            char[] charMessage = GetNewMI.ToCharArray();

                                            int value = 0, index = 5;

                                            foreach (var item in charMessage)
                                            {
                                                var checkInt = int.TryParse(item.ToString(), out value);
                                                index++;
                                                if (!checkInt)
                                                {
                                                    GetMICode = GetNewMI.Substring(index, 5); break;
                                                }
                                            }

                                            var MI_Id = _con.QuerySingle<int>("INSERT INTO MIs (Code, CreateDate, UserId, IsDelete) OUTPUT INSERTED.Id VALUES (@Code, @DateNow, @UserId, @IsDelete);", new { Code = GetMICode, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false });

                                            MIId = MI_Id;
                                        }
                                        else
                                        {
                                            MIId = GetMIId;
                                        }
                                        #endregion

                                        #region CheckMasterFund
                                        var GetFundId = 0;

                                        var GetFundCode = "";

                                        foreach (var item in GetFunds)
                                        {
                                            var a = message.IndexOf(item.Code);

                                            if (a != -1)
                                            {
                                                GetFundId = item.Id;
                                                break;
                                            }
                                        }

                                        if (GetFundId == 0)
                                        {
                                            var GetNewFund = message.Substring(5, message.Length);

                                            char[] charMessage = GetNewFund.ToCharArray();

                                            int value = 0, index = 0;

                                            foreach (var item in charMessage)
                                            {
                                                var checkInt = int.TryParse(item.ToString(), out value);
                                                index++;
                                                if (!checkInt)
                                                {
                                                    GetFundCode = GetNewFund.Substring(index, GetNewFund.Length - index); break;
                                                }
                                            }

                                            var Fund_Id = _con.QuerySingle<int>("INSERT INTO Funds (Code, CreateDate, UserId, IsDelete) OUTPUT INSERTED.Id VALUES (@Code, @DateNow, @UserId, @IsDelete);", new { Code = GetFundCode, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false });

                                            FundId = Fund_Id;
                                        }
                                        else
                                        {
                                            FundId = GetFundId;
                                        }
                                        #endregion

                                        #region CheckDataAplikasi
                                        var checkData = _con.QueryFirst<int>("SELECT COUNT(*) FROM DataRedemps WHERE FundId = @FundId AND MIId = @MIId AND Nasabah = @Nasabah AND Nominal = @Nominal AND TransDate = @TransDate AND ReferenceNo = @ReferenceNo",
                                                        new { FundId, MIId, Nasabah = receiver_name, Nominal = payamount, TransDate = TanggalFund, ReferenceNo = noref });

                                        if (checkData == 0)
                                        {
                                            string sender = ((_excel.Range)range_.Cells[row, 3]).Text;
                                            string sender_res = ((_excel.Range)range_.Cells[row, 4]).Text;
                                            string bic = ((_excel.Range)range_.Cells[row, 7]).Text;
                                            string receiver_rek = ((_excel.Range)range_.Cells[row, 8]).Text;
                                            string receiver_bank = ((_excel.Range)range_.Cells[row, 10]).Text;
                                            string receiver_res = ((_excel.Range)range_.Cells[row, 11]).Text;
                                            string receiver_type = ((_excel.Range)range_.Cells[row, 12]).Text;

                                            string sql = "INSERT INTO DataRedemps (ReferenceNo, TransDate, Nominal, Nasabah, MIId, FundId, CreateDate, UserId, MatchingId, IsDelete) VALUES (@ReferenceNo, @TransDate, @Nominal, @Nasabah, @MIId, @FundId, @CreateDate, @UserId, @MatchingId, @IsDelete)";
                                            _con.Execute(sql, new { ReferenceNo = noref, TransDate = TanggalFund, Nominal = payamount, Nasabah = receiver_name, MIId, FundId, CreateDate = DateTime.Now, UserId = currentUser.Id, MatchingId = 1, IsDelete = false });

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
                                else if (judul == "reference-no" && judul2 == "sender-account" && judul3 == "sender-name-1")
                                {
                                    #region Aplikasi Red / Swi - RTGS
                                    int csuccess = 0, cfails = 0;
                                    var rek = file.FileName;
                                    int RekeningId = 0;

                                    var list = new List<object>();
                                    var GetMIs = _con.Query<MI>("SELECT Id, Code FROM MIs");
                                    var GetFunds = _con.Query<Fund>("SELECT Id, Code FROM Funds");

                                    string[] getDate = rek.Split('-');
                                    DateTime TanggalFund = DateTime.ParseExact(getDate[1].ToLower().Replace(".xls", "").Replace(".xlsx", ""), "ddMMyyyy", CultureInfo.InvariantCulture);

                                    for (int row = 2; row < range_.Rows.Count + 1; row++)
                                    {

                                        int MIId = 0, FundId = 0;

                                        string NoRek = ((_excel.Range)range_.Cells[row, 2]).Text;

                                        if (NoRek == "")
                                            continue;

                                        string noref = ((_excel.Range)range_.Cells[row, 1]).Text;
                                        Int64 payamount = Convert.ToInt64(((_excel.Range)range_.Cells[row, 7]).Value);
                                        string receiver_name = ((_excel.Range)range_.Cells[row, 8]).Text;
                                        string message = ((_excel.Range)range_.Cells[row, 18]).Text;


                                        #region CheckMasterRekening
                                        var resultRek = _con.QueryFirstOrDefault<Rekening>("SELECT Id FROM Rekenings WHERE NoRek = @NoRek", new { NoRek });
                                        if (resultRek == null)
                                        {

                                            var RekId = _con.QuerySingle<int>("INSERT INTO Rekenings (NoRek, CreateDate, UserId, IsDelete) OUTPUT INSERTED.Id VALUES (@NoRek, @DateNow, @UserId, @IsDelete);", new { NoRek = NoRek, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false });

                                            RekeningId = RekId;
                                        }
                                        else
                                        {
                                            RekeningId = resultRek.Id;
                                        }
                                        #endregion

                                        #region CheckMasterMI
                                        var GetMIId = 0;

                                        var GetMICode = "";

                                        foreach (var item in GetMIs)
                                        {
                                            var a = message.IndexOf(item.Code);

                                            if (a != -1)
                                            {
                                                GetMIId = item.Id;
                                                break;
                                            }
                                        }

                                        if (GetMIId == 0)
                                        {
                                            var GetNewMI = message.Substring(0, 5);

                                            var MI_Id = _con.QuerySingle<int>("INSERT INTO MIs (Code, CreateDate, UserId, IsDelete) OUTPUT INSERTED.Id VALUES (@Code, @DateNow, @UserId, @IsDelete);", new { Code = GetMICode, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false });

                                            MIId = MI_Id;
                                        }
                                        else
                                        {
                                            MIId = GetMIId;
                                        }
                                        #endregion

                                        #region CheckMasterFund
                                        var GetFundId = 0;

                                        var GetFundCode = "";

                                        foreach (var item in GetFunds)
                                        {
                                            var a = message.IndexOf(item.Code);

                                            if (a != -1)
                                            {
                                                GetFundId = item.Id;
                                                break;
                                            }
                                        }

                                        if (GetFundId == 0)
                                        {
                                            var GetNewFund = message;

                                            var Fund_Id = _con.QuerySingle<int>("INSERT INTO Funds (Code, CreateDate, UserId, IsDelete) OUTPUT INSERTED.Id VALUES (@Code, @DateNow, @UserId, @IsDelete);", new { Code = GetFundCode, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false });

                                            FundId = Fund_Id;
                                        }
                                        else
                                        {
                                            FundId = GetFundId;
                                        }
                                        #endregion

                                        #region CheckDataAplikasi
                                        var checkData = _con.QueryFirst<int>("SELECT COUNT(*) FROM DataRedemps WHERE FundId = @FundId AND MIId = @MIId AND Nasabah = @Nasabah AND Nominal = @Nominal AND TransDate = @TransDate AND ReferenceNo = @ReferenceNo",
                                                        new { FundId, MIId, Nasabah = receiver_name, Nominal = payamount, TransDate = TanggalFund, ReferenceNo = noref });

                                        if (checkData == 0)
                                        {
                                            string sender = ((_excel.Range)range_.Cells[row, 3]).Text;
                                            string sender_res = ((_excel.Range)range_.Cells[row, 4]).Text;
                                            string bic = ((_excel.Range)range_.Cells[row, 7]).Text;
                                            string receiver_rek = ((_excel.Range)range_.Cells[row, 8]).Text;
                                            string receiver_bank = ((_excel.Range)range_.Cells[row, 10]).Text;
                                            string receiver_res = ((_excel.Range)range_.Cells[row, 11]).Text;
                                            string receiver_type = ((_excel.Range)range_.Cells[row, 12]).Text;

                                            string sql = "INSERT INTO DataRedemps (ReferenceNo, TransDate, Nominal, Nasabah, MIId, FundId, CreateDate, UserId, MatchingId, IsDelete) VALUES (@ReferenceNo, @TransDate, @Nominal, @Nasabah, @MIId, @FundId, @CreateDate, @UserId, @MatchingId, @IsDelete)";
                                            _con.Execute(sql, new { ReferenceNo = noref, TransDate = TanggalFund, Nominal = payamount, Nasabah = receiver_name, MIId, FundId, CreateDate = DateTime.Now, UserId = currentUser.Id, MatchingId = 1, IsDelete = false });

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
                                else if (judul4 == "fundtransfer" && judul5 == "fundbatch#")
                                {
                                    #region Aplikasi Red / Swi - Pindah Buku
                                    int csuccess = 0, cfails = 0;
                                    var rek = file.FileName;
                                    int RekeningId = 0;

                                    var list = new List<object>();

                                    for (int row = 9; row < range_.Rows.Count + 1; row++)
                                    {
                                        int MIId = 0, FundId = 0;

                                        string datetext = ((_excel.Range)range_.Cells[row, 5]).Text;
                                        DateTime TanggalFund = DateTime.ParseExact(datetext, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                        string message = ((_excel.Range)range_.Cells[row, 6]).Text;
                                        string receiver_name = ((_excel.Range)range_.Cells[row, 7]).Text;
                                        string noref = ((_excel.Range)range_.Cells[row, 10]).Text;
                                        Int64 payamount = Convert.ToInt64(((_excel.Range)range_.Cells[row, 13]).Value);
                                        string NoRek = ((_excel.Range)range_.Cells[row, 18]).Text;
                                        string NamaRek = ((_excel.Range)range_.Cells[row, 19]).Text;

                                        #region CheckMasterRekening
                                        var resultRek = _con.QueryFirstOrDefault<Rekening>("SELECT Id FROM Rekenings WHERE NoRek = @NoRek", new { NoRek });
                                        if (resultRek == null)
                                        {

                                            var RekId = _con.QuerySingle<int>("INSERT INTO Rekenings (NoRek, NamaRek, CreateDate, UserId, IsDelete) OUTPUT INSERTED.Id VALUES (@NoRek, @NamaRek, @DateNow, @UserId, @IsDelete);", new { NoRek = NoRek, NamaRek = NamaRek, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false });

                                            RekeningId = RekId;
                                        }
                                        else
                                        {
                                            RekeningId = resultRek.Id;
                                        }
                                        #endregion

                                        #region CheckMasterMI
                                        var GetMICode = message.Substring(0, 5);

                                        var resultMI = _con.QueryFirstOrDefault<Rekening>("SELECT Id FROM MIs WHERE Code = @GetMICode", new { GetMICode });

                                        if (resultMI == null)
                                        {
                                            var MI_Id = _con.QuerySingle<int>("INSERT INTO MIs (Code, CreateDate, UserId, IsDelete) OUTPUT INSERTED.Id VALUES (@Code, @DateNow, @UserId, @IsDelete);", new { Code = GetMICode, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false });

                                            MIId = MI_Id;
                                        }
                                        else
                                        {
                                            MIId = resultMI.Id;
                                        }
                                        #endregion

                                        #region CheckMasterFund
                                        var GetFundCode = message;
                                        var GetFundName = NamaRek;

                                        var resultFund = _con.QueryFirstOrDefault<Rekening>("SELECT Id FROM Funds WHERE Code = @GetFundCode", new { GetFundCode });

                                        if (resultFund == null)
                                        {
                                            var Fund_Id = _con.QuerySingle<int>("INSERT INTO Funds (Code, Nama, CreateDate, UserId, IsDelete) OUTPUT INSERTED.Id VALUES (@Code, @Nama, @DateNow, @UserId, @IsDelete);", new { Code = GetFundCode, Nama = GetFundName, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false });

                                            FundId = Fund_Id;
                                        }
                                        else
                                        {
                                            FundId = resultFund.Id;
                                        }
                                        #endregion

                                        #region CheckDataAplikasi
                                        var checkData = _con.QueryFirst<int>("SELECT COUNT(*) FROM DataRedemps WHERE FundId = @FundId AND MIId = @MIId AND Nasabah = @Nasabah AND Nominal = @Nominal AND TransDate = @TransDate AND ReferenceNo = @ReferenceNo",
                                                        new { FundId, MIId, Nasabah = receiver_name, Nominal = payamount, TransDate = TanggalFund, ReferenceNo = noref });

                                        if (checkData == 0)
                                        {
                                            string sql = "INSERT INTO DataRedemps (ReferenceNo, TransDate, Nominal, Nasabah, MIId, FundId, CreateDate, UserId, MatchingId, IsDelete) VALUES (@ReferenceNo, @TransDate, @Nominal, @Nasabah, @MIId, @FundId, @CreateDate, @UserId, @MatchingId, @IsDelete)";
                                            _con.Execute(sql, new { ReferenceNo = noref, TransDate = TanggalFund, Nominal = payamount, Nasabah = receiver_name, MIId, FundId, CreateDate = DateTime.Now, UserId = currentUser.Id, MatchingId = 1, IsDelete = false });

                                            csuccess++;
                                        }
                                        else
                                        {
                                            cfails++;
                                        }

                                        #endregion
                                    }
                                    #endregion
                                }
                                else if ((judul == "postdate" && judul2 == "valuedate" && judul3 == "branch") || judul == "postdatevaluedatebranchjournalno.descriptiondebitcredit")
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

                                    if (judul == "postdate" && judul2 == "valuedate" && judul3 == "branch")
                                    {
                                        for (int row = 2; row < range_.Rows.Count + 1; row++)
                                        {
                                            DateTime Tanggal = DateTime.ParseExact(((_excel.Range)range_.Cells[row, 2]).Text, "dd/MM/yy HH.mm.ss", CultureInfo.InvariantCulture);
                                            string Keterangan = ((_excel.Range)range_.Cells[row, 5]).Text;
                                            double Debit = ((_excel.Range)range_.Cells[row, 6]).Value2;
                                            double Credit = ((_excel.Range)range_.Cells[row, 7]).Value2;

                                            //SKIP semua biaya RTGS & Kliring
                                            //if ((Keterangan.Contains("BIAYA TRANSAKSI KLIRING") && (Debit == 2900 || Credit == 2900)) || (Keterangan.Contains("BIAYA TRANSAKSI RTGS") && (Debit == 30000 || Credit == 30000)))
                                            //{
                                            //    continue;
                                            //}

                                            var check = _con.QueryFirst<int>("SELECT COUNT(*) FROM DataFundReds WHERE RekeningId = @RekeningId AND Debit = @Debit AND Credit = @Credit AND Tanggal = @Tanggal AND Keterangan = @Keterangan",
                                                                new { RekeningId, Debit, Credit, Tanggal, Keterangan });

                                            if (check == 0)
                                            {
                                                var KeteranganDua = Keterangan.Replace(" ", "").Replace(",", "").Replace(".", "").Replace("'", "").ToUpper();

                                                string sql = "INSERT INTO DataFundReds (RekeningId, Tanggal, Keterangan, KeteranganDua, Debit, Credit, CreateDate, UserId, MatchingId, IsDelete) VALUES (@RekeningId, @Tanggal, @Keterangan, @KeteranganDua, @Debit, @Credit, @CreateDate, @UserId, @MatchingId, @IsDelete)";
                                                _con.Execute(sql, new { RekeningId, Tanggal, Keterangan, KeteranganDua, Debit, Credit, CreateDate = DateTime.Now, UserId = currentUser.Id, MatchingId = 1, IsDelete = false });

                                                csuccess++;
                                            }
                                            else
                                            {
                                                cfails++;
                                            }


                                        }

                                        resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Success", warna = "success", success = csuccess, fails = cfails });
                                    }
                                    else if (judul == "postdatevaluedatebranchjournalno.descriptiondebitcredit")
                                    {
                                        for (int row = 2; row < range_.Rows.Count + 1; row++)
                                        {

                                            string data = (range_.Cells[row, 1] as _excel.Range).Text;
                                            string[] separator = { ",\"", "\"" };
                                            string[] strlist_ = data.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);

                                            string[] strDate = strlist_[0].Split(',');

                                            DateTime Tanggal = DateTime.ParseExact(strDate[1], "dd/MM/yy HH.mm.ss", CultureInfo.InvariantCulture);
                                            string Keterangan = strlist_[1];
                                            var strDebit = strlist_[2] == ".00" ? "0" : strlist_[2];
                                            var strCredit = strlist_[2] == ".00" ? "0" : strlist_[3];
                                            double Debit = Convert.ToDouble(strDebit);
                                            double Credit = Convert.ToDouble(strCredit);

                                            //SKIP semua biaya RTGS & Kliring
                                            //if ((Keterangan.Contains("BIAYA TRANSAKSI KLIRING") && (Debit == 2900 || Credit == 2900)) || (Keterangan.Contains("BIAYA TRANSAKSI RTGS") && (Debit == 30000 || Credit == 30000)))
                                            //{
                                            //    continue;
                                            //}

                                            var check = _con.QueryFirst<int>("SELECT COUNT(*) FROM DataFundReds WHERE RekeningId = @RekeningId AND Debit = @Debit AND Credit = @Credit AND Tanggal = @Tanggal AND Keterangan = @Keterangan",
                                                                new { RekeningId, Debit, Credit, Tanggal, Keterangan });

                                            if (check == 0)
                                            {
                                                var KeteranganDua = Keterangan.Replace(" ", "").Replace(",", "").Replace(".", "").Replace("'", "").ToUpper();

                                                string sql = "INSERT INTO DataFundReds (RekeningId, Tanggal, Keterangan, KeteranganDua, Debit, Credit, CreateDate, UserId, MatchingId, IsDelete) VALUES (@RekeningId, @Tanggal, @Keterangan, @KeteranganDua, @Debit, @Credit, @CreateDate, @UserId, @MatchingId, @IsDelete)";
                                                _con.Execute(sql, new { RekeningId, Tanggal, Keterangan, KeteranganDua, Debit, Credit, CreateDate = DateTime.Now, UserId = currentUser.Id, MatchingId = 1, IsDelete = false });

                                                csuccess++;
                                            }
                                            else
                                            {
                                                cfails++;
                                            }
                                        }

                                        resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Success", warna = "success", success = csuccess, fails = cfails });
                                    }

                                    #endregion
                                }
                                else if (judul == "informasirekening-mutasirekening")
                                {
                                    #region FundBCA

                                    int csuccess = 0, cfails = 0;
                                    string norek = ((_excel.Range)range_.Cells[3, 1]).Text;
                                    string namarek = ((_excel.Range)range_.Cells[4, 1]).Text;
                                    string periode = ((_excel.Range)range_.Cells[5, 1]).Text;
                                    string NoRek = norek.Substring(15);
                                    string NamaRek = namarek.Substring(7);
                                    int RekeningId = 0;

                                    #region CheckMasterRekening
                                    var resultRek = _con.QueryFirstOrDefault<Rekening>("SELECT Id FROM Rekenings WHERE NoRek = @NoRek", new { NoRek });
                                    if (resultRek == null)
                                    {

                                        var RekId = _con.QuerySingle<int>("INSERT INTO Rekenings (NoRek, NamaRek, CreateDate, UserId, IsDelete) OUTPUT INSERTED.Id VALUES (@NoRek, @NamaRek, @DateNow, @UserId, @IsDelete);", new { NoRek = NoRek, NamaRek = NamaRek, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false });

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

                                    #region ParsingFund
                                    for (int row = 8; row <= range_.Rows.Count; row++)
                                    {
                                        string tanggal = (range_.Cells[row, 1] as _excel.Range).Text;
                                        if (tanggal != "")
                                        {
                                            int cek;
                                            bool success = int.TryParse(tanggal.Substring(0, 1), out cek);
                                            if (success)
                                            {
                                                if (tanggal.Length > 6)
                                                {
                                                    #region FundSekolom
                                                    string data = (range_.Cells[row, 1] as _excel.Range).Text;
                                                    string[] spearator = { ",\"" };
                                                    string[] strlist = data.Split(spearator, System.StringSplitOptions.RemoveEmptyEntries);

                                                    #region Tanggal
                                                    string date = strlist[0].Replace("\"", "").Replace("\\", "");
                                                    day = Convert.ToInt16(date.Substring(0, 2));
                                                    bnow = Convert.ToInt16(date.Substring(3, 2));

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

                                                    DateTime Tanggal = DateTime.ParseExact(tanggal, "d/M/yyyy", CultureInfo.InvariantCulture);
                                                    #endregion

                                                    long Debit = 0, Credit = 0;

                                                    string jml = strlist[3].Replace("\"", "").Replace("\\", "");
                                                    string crdb = jml.Substring((jml.Length - 2), 2);
                                                    long Jumlah = Convert.ToInt64(jml.Substring(0, (jml.Length - 6)).Replace(",", ""));

                                                    if (crdb == "DB")
                                                        Debit = Jumlah;
                                                    if (crdb == "CR")
                                                        Credit = Jumlah;

                                                    string sld = strlist[4].Replace("\"", "").Replace("\\", "").Replace(",", "");
                                                    long Saldo = Convert.ToInt64(sld.Substring(0, (sld.Length - 3)));

                                                    string Keterangan = strlist[1].Replace("\"", "").Replace("\\", "");

                                                    var checkFund = _con.QueryFirst<int>("SELECT COUNT(*) FROM DataFundReds WHERE RekeningId = @RekeningId AND Keterangan = @Keterangan AND Debit = @Debit AND Credit = @Credit AND Saldo = @Saldo AND Tanggal = @Tanggal",
                                                                    new { RekeningId, Keterangan, Debit, Credit, Saldo, Tanggal });

                                                    if (checkFund == 0)
                                                    {
                                                        var KeteranganDua = Keterangan.Replace(" ", "").Replace(",", "").Replace(".", "").Replace("'", "").ToUpper();

                                                        string sql = "INSERT INTO DataFundReds (RekeningId, Tanggal, Keterangan, KeteranganDua, Debit, Credit, Saldo, CreateDate, UserId, MatchingId, IsDelete) VALUES (@RekeningId, @Tanggal, @Keterangan, @KeteranganDua, @Debit, @Credit, @Saldo, @CreateDate, @UserId, @MatchingId, @IsDelete)";
                                                        _con.Execute(sql, new { RekeningId, Tanggal, Keterangan, KeteranganDua, Debit, Credit, Saldo, CreateDate = DateTime.Now, UserId = currentUser.Id, MatchingId = 1, IsDelete = false });

                                                        csuccess++;
                                                    }
                                                    else
                                                    {
                                                        cfails++;
                                                    }
                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region FundBedaKolom

                                                    #region Tanggal
                                                    var formatTanggal = ((_excel.Range)range_.Cells[row, 1]).NumberFormat;
                                                    if (formatTanggal == "General")
                                                    {
                                                        string date = ((_excel.Range)range_.Cells[row, 1]).Text;
                                                        day = Convert.ToInt16(date.Substring(0, 2));
                                                        bnow = Convert.ToInt16(date.Substring(3, 2));
                                                    }
                                                    else if (formatTanggal == "d-mmm")
                                                    {
                                                        string textdate = ((_excel.Range)range_.Cells[row, 1]).Text;
                                                        DateTime d = DateTime.ParseExact(textdate, "d-MMM", CultureInfo.InvariantCulture);
                                                        string Text = Convert.ToString(d.Month) + "/" + Convert.ToString(d.Day);
                                                        day = Convert.ToInt16(Convert.ToString(d.Month));
                                                        bnow = Convert.ToInt16(Convert.ToString(d.Day));
                                                    }
                                                    else
                                                    {
                                                        string Text = ((_excel.Range)range_.Cells[row, 1]).Text;
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

                                                    DateTime Tanggal = DateTime.ParseExact(tanggal, "d/M/yyyy", CultureInfo.InvariantCulture);
                                                    #endregion

                                                    long Debit = 0, Credit = 0;

                                                    string jml = ((_excel.Range)range_.Cells[row, 4]).Text;
                                                    string crdb = jml.Substring((jml.Length - 2), 2);
                                                    long Jumlah = Convert.ToInt64(jml.Substring(0, (jml.Length - 6)).Replace(",", ""));

                                                    if (crdb == "DB")
                                                        Debit = Jumlah;
                                                    if (crdb == "CR")
                                                        Credit = Jumlah;

                                                    var sld = ((_excel.Range)range_.Cells[row, 5]).Value2;
                                                    Int64 Saldo = Convert.ToInt64(sld);

                                                    string Keterangan = ((_excel.Range)range_.Cells[row, 2]).Text;

                                                    var checkFund = _con.QueryFirst<int>("SELECT COUNT(*) FROM DataFundReds WHERE RekeningId = @RekeningId AND Keterangan = @Keterangan AND Jumlah = @Jumlah AND Saldo = @Saldo AND Tanggal = @Tanggal",
                                                                        new { RekeningId, Keterangan, Jumlah, Saldo, Tanggal });

                                                    if (checkFund == 0)
                                                    {
                                                        var KeteranganDua = Keterangan.Replace(" ", "").Replace(",", "").Replace(".", "").Replace("'", "").ToUpper();

                                                        string sql = "INSERT INTO DataFundReds (RekeningId, Tanggal, Keterangan, KeteranganDua, Debit, Credit, Saldo, CreateDate, UserId, MatchingId, IsDelete) VALUES (@RekeningId, @Tanggal, @Keterangan, @KeteranganDua, @Debit, @Credit, @Saldo, @CreateDate, @UserId, @MatchingId, @IsDelete)";
                                                        _con.Execute(sql, new { RekeningId, Tanggal, Keterangan, KeteranganDua, Debit, Credit, Saldo, CreateDate = DateTime.Now, UserId = currentUser.Id, MatchingId = 1, IsDelete = false });

                                                        csuccess++;
                                                    }
                                                    else
                                                    {
                                                        cfails++;
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
                                else
                                {
                                    resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Fails", warna = "danger", success = 0, fails = 0 });
                                }

                                #region (Not Used) Aplikasi Red / Swi
                                //if (judul == String.Empty)
                                //{
                                //int csuccess = 0, cfails = 0;
                                //var rek = file.FileName;

                                //foreach (_excel.Worksheet sheet in workbook.Worksheets)
                                //{
                                //    _excel.Range range = sheet.UsedRange;

                                //    DateTime DateofFund = DateTime.ParseExact(((_excel.Range)range.Cells[8, 4]).Text, "dd-MMM-yyyy", new CultureInfo("id-ID"));

                                //    var checkData = _con.QueryFirst<int>("SELECT COUNT(*) FROM DataAplikasiReds WHERE CreateDate = @DateofFund", new { DateofFund });

                                //    if (checkData == 0)
                                //    {
                                //        int MIId = 0, FundId = 0, FundTypeId = 0;

                                //        #region MI
                                //        string FundMgr = ((_excel.Range)range.Cells[5, 4]).Text;

                                //        //var resultMI = _con.QueryFirstOrDefault<MI>("SELECT Id FROM MIs WHERE Nama = @FundMgr", new { FundMgr });
                                //        var resultMI = _context.MI.Where(x => x.Nama.Replace(".", "").ToLower() == FundMgr.Replace(".", "").ToLower()).Select(x => x.Id).FirstOrDefault();

                                //        if (resultMI == 0)
                                //        {
                                //            var MI_Id = _con.QuerySingle<int>("INSERT INTO MIs (Nama, CreateDate, UserId, IsDelete) OUTPUT INSERTED.Id VALUES (@FundMgr, @DateNow, @UserId, @IsDelete);", new { FundMgr, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false });

                                //            MIId = MI_Id;
                                //        }
                                //        else
                                //        {
                                //            MIId = resultMI;
                                //        }
                                //        #endregion

                                //        #region Fund
                                //        string FundName = ((_excel.Range)range.Cells[6, 4]).Text;

                                //        var resultFund = _con.QueryFirstOrDefault<Fund>("SELECT Id FROM Funds WHERE Nama = @FundName", new { FundName });

                                //        if (resultFund == null)
                                //        {
                                //            var Fund_Id = _con.QuerySingle<int>("INSERT INTO Funds (Nama, CreateDate, UserId, IsDelete) OUTPUT INSERTED.Id VALUES (@FundName, @DateNow, @UserId, @IsDelete);", new { FundName, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false });

                                //            FundId = Fund_Id;
                                //        }
                                //        else
                                //        {
                                //            FundId = resultFund.Id;
                                //        }
                                //        #endregion

                                //        #region FundType
                                //        string FundType = ((_excel.Range)range.Cells[7, 4]).Text;

                                //        var resultFundType = _con.QueryFirstOrDefault<FundType>("SELECT Id FROM FundTypes WHERE Nama = @FundType", new { FundType });

                                //        if (resultFundType == null)
                                //        {
                                //            var FundType_Id = _con.QuerySingle<int>("INSERT INTO FundTypes (Nama, CreateDate, UserId, IsDelete) OUTPUT INSERTED.Id VALUES (@FundType, @DateNow, @UserId, @IsDelete);", new { FundType, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false });

                                //            FundTypeId = FundType_Id;
                                //        }
                                //        else
                                //        {
                                //            FundTypeId = resultFundType.Id;
                                //        }
                                //        #endregion

                                //        int SAId = 0;
                                //        string SACode = String.Empty;

                                //        var usedRowCount = sheet.UsedRange.Rows.Count;

                                //        for (int row = 13; row < usedRowCount; row++)
                                //        {
                                //            string FirstCol = ((_excel.Range)range.Cells[row, 1]).Text;

                                //            int n;
                                //            var isNumeric = int.TryParse(FirstCol, out n);

                                //            if (!FirstCol.ToLower().Contains("total") && !isNumeric)
                                //            {
                                //                #region SA
                                //                var SAs = FirstCol.Split('-');
                                //                SACode = SAs[0];

                                //                var resultSA = _con.QueryFirstOrDefault<SA>("SELECT Id FROM SAs WHERE Code = @SACode", new { SACode = SACode });

                                //                if (resultSA == null)
                                //                {
                                //                    var SA_Id = _con.QuerySingle<int>("INSERT INTO SAs (Code, Nama, CreateDate, UserId, IsDelete) OUTPUT INSERTED.Id VALUES (@SACode, @SAName, @DateNow, @UserId, @IsDelete);", new { SACode, SAName = SAs[1], DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false });

                                //                    SAId = SA_Id;
                                //                }
                                //                else
                                //                {
                                //                    SAId = resultSA.Id;
                                //                }
                                //                #endregion
                                //            }

                                //            if (isNumeric)
                                //            {
                                //                string CIF = ((_excel.Range)range.Cells[row, 2]).Text;
                                //                string CIF_APERD = ((_excel.Range)range.Cells[row + 1, 2]).Text;
                                //                string AccNum = ((_excel.Range)range.Cells[row, 4]).Text;
                                //                string HolderName = ((_excel.Range)range.Cells[row + 1, 4]).Text;
                                //                string RedempNo = ((_excel.Range)range.Cells[row, 6]).Text;
                                //                DateTime TransDate = DateTime.ParseExact(((_excel.Range)range.Cells[row, 7]).Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                //                double PayAmount = ((_excel.Range)range.Cells[row, 8]).Value2;

                                //                string _UnitRedeem = ((_excel.Range)range.Cells[row, 9]).Text;

                                //                double a;
                                //                var isNumericUR = double.TryParse(_UnitRedeem, out a); //cek apakah Cell UnitRedeem berisi double

                                //                double UnitRedeem;
                                //                if (isNumericUR)
                                //                    UnitRedeem = a;
                                //                else
                                //                    UnitRedeem = 0;

                                //                string sql = "INSERT INTO DataAplikasiReds (CIF, CIF_APERD, AccNum, HolderName, RedempNo, TransDate, PayAmount, UnitRedeem, SAId, FundId, MIId, CreateDate, UserId, MatchingId, IsDelete) VALUES (@CIF, @CIF_APERD, @AccNum, @HolderName, @RedempNo, @TransDate, @PayAmount, @UnitRedeem, @SAId, @FundId, @MIId, @CreateDate, @UserId, @MatchingId, @IsDelete)";
                                //                _con.Execute(sql, new { CIF, CIF_APERD, AccNum, HolderName, RedempNo, TransDate, PayAmount, UnitRedeem, SAId, FundId, MIId, CreateDate = DateTime.Now, UserId = currentUser.Id, MatchingId = 1, IsDelete = false });

                                //                csuccess++;

                                //                row++;
                                //            }
                                //        }
                                //    }
                                //    else
                                //    {
                                //        cfails++;
                                //    }
                                //}

                                //resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Success", warna = "success", success = csuccess, fails = cfails });
                                //}
                                #endregion

                                //}
                                //catch 
                                //{
                                //}
                                //finally
                                //{
                                //    // ---=== Start of Closing Document & Collecting Garbage ===---//
                                workbooks.Close();
                                application.Quit();
                                System.IO.File.Delete(path);

                                //    //Marshal.ReleaseComObject(workbook);
                                //    //Marshal.ReleaseComObject(workbooks);
                                //    //Marshal.ReleaseComObject(application);
                                //    // ---=== End of Closing Document & Collecting Garbage ===---//
                                //}

                                GC.Collect();
                                GC.WaitForPendingFinalizers();
                            }
                            else
                            {
                                resultupload.Add(new ResultUpload() { namafile = file.FileName, status = "Fails", warna = "danger", success = 0, fails = 0 });
                            }
                        }
                    }
                    #endregion

                    #region Rekon
                    var listData = new List<int[]>();

                    var getAplikasi = _con.Query<DataRedemp>("SELECT Id, Nasabah, Nominal FROM DataRedemps WHERE MatchingId = @MatchingId", new { MatchingId = 1 });

                    var insertTrans = "INSERT INTO TransRedemps (MatchingId, CreateDate, InputerId, IsDelete) OUTPUT INSERTED.Id VALUES (@MatchingId, @DateNow, @UserId, @IsDelete)";
                    var insertDataApl = "INSERT INTO TrRedAplikasis (DataRedempId, CreateDate, TransRedempId) VALUES (@DataRedempId, @DateNow, @TransRedempId)";
                    var insertDataFund = "INSERT INTO TrRedFunds (DataFundRedId, CreateDate, TransRedempId) VALUES (@DataFundRedId, @DateNow, @TransRedempId)";
                    var updateDataApl = "UPDATE DataRedemps SET MatchingId = @MatchingId, UpdateDate = @UpdateDate WHERE Id = @AppId";
                    var updateDataFund = "UPDATE DataFundReds SET MatchingId = @MatchingId, UpdateDate = @UpdateDate WHERE Id = @FundId";

                    foreach (var app in getAplikasi)
                    {
                        var getFund = _context.DataFundRed.Where(x => x.Keterangan.Contains(app.Nasabah.Substring(0, 23)) && (x.Debit == app.Nominal || x.Credit == app.Nominal)).FirstOrDefault();

                        if (getFund != null)
                        {
                            _con.Open();
                            using (var transaction = _con.BeginTransaction())
                            {
                                var TransId = _con.QuerySingle<int>(insertTrans, new { MatchingId = 3, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false }, transaction: transaction);
                                _con.Execute(insertDataApl, new { DataRedempId = app.Id, DateNow = DateTime.Now, TransRedempId = TransId }, transaction: transaction);
                                _con.Execute(insertDataFund, new { DataFundRedId = getFund.Id, DateNow = DateTime.Now, TransRedempId = TransId }, transaction: transaction);

                                _con.Execute(updateDataApl, new { MatchingId = 3, UpdateDate = DateTime.Now, AppId = app.Id }, transaction: transaction);
                                _con.Execute(updateDataFund, new { MatchingId = 3, UpdateDate = DateTime.Now, FundId = getFund.Id }, transaction: transaction);

                                transaction.Commit();
                            }
                            _con.Close();
                        }
                    }
                    #endregion

                    #region Old Rekon
                    //var listData = new List<int[]>();

                    //var getAplikasi = _con.Query<DataAplikasiRed>("SELECT Id, HolderName, PayAmount FROM DataAplikasiReds WHERE MatchingId = @MatchingId", new { MatchingId = 1 });

                    //var insertTrans = "INSERT INTO TransRedemps (MatchingId, CreateDate, InputerId, IsDelete) OUTPUT INSERTED.Id VALUES (@MatchingId, @DateNow, @UserId, @IsDelete)";
                    //var insertDataApl = "INSERT INTO TrRedAplikasis (DataAplikasiRedId, CreateDate, TransRedempId) VALUES (@DataAplikasiRedId, @DateNow, @TransRedempId)";
                    //var insertDataFund = "INSERT INTO TrRedFunds (DataFundRedId, CreateDate, TransRedempId) VALUES (@DataFundRedId, @DateNow, @TransRedempId)";
                    //var updateDataApl = "UPDATE DataAplikasiReds SET MatchingId = @MatchingId, UpdateDate = @UpdateDate WHERE Id = @AppId";
                    //var updateDataFund = "UPDATE DataFundReds SET MatchingId = @MatchingId, UpdateDate = @UpdateDate WHERE Id = @FundId";

                    //foreach (var app in getAplikasi)
                    //{
                    //    long Jumlah = (long)Math.Round((decimal)app.PayAmount);

                    //    //var getFund = _con.Query<DataFundRed>("SELECT df.* FROM DataFundReds df JOIN Rekenings rk on df.RekeningId = rk.Id WHERE df.MatchingId = @MatchingId AND rk.SAId = @SAId AND rk.FundId = @FundId AND rk.MIId = @MIId AND df.Jumlah = @Jumlah",
                    //    //new { MatchingId = 1, SAId = app.SAId, FundId = app.FundId, MIId = app.MIId });

                    //    var getFund2 = _context.DataFundRed.Where(x => x.MatchingId == 1 && x.Keterangan.ToLower().Contains(app.HolderName.ToLower()));

                    //    int fundId = 0;

                    //    foreach (var fund in getFund2)
                    //    {
                    //        long getFundAmount = 0;

                    //        if (fund.Credit != 0)
                    //            getFundAmount = isKliringRTGS(fund.Credit);
                    //        else if (fund.Debit != 0)
                    //            getFundAmount = isKliringRTGS(fund.Debit);

                    //        if (app.PayAmount == getFundAmount)
                    //        {
                    //            fundId = fund.Id;
                    //            break;
                    //        }
                    //        //Alternatif tambahan logic
                    //        //else if (app.PayAmount == fund.Credit || app.PayAmount == fund.Debit) {
                    //        //    fundId = fund.Id;
                    //        //    break;
                    //        //}
                    //    }

                    //    if (fundId != 0)
                    //    {
                    //        _con.Open();
                    //        using (var transaction = _con.BeginTransaction())
                    //        {
                    //            var TransId = _con.QuerySingle<int>(insertTrans, new { MatchingId = 3, DateNow = DateTime.Now, UserId = currentUser.Id, IsDelete = false }, transaction: transaction);
                    //            _con.Execute(insertDataApl, new { DataAplikasiRedId = app.Id, DateNow = DateTime.Now, TransRedempId = TransId }, transaction: transaction);
                    //            _con.Execute(insertDataFund, new { DataFundRedId = fundId, DateNow = DateTime.Now, TransRedempId = TransId }, transaction: transaction);

                    //            _con.Execute(updateDataApl, new { MatchingId = 3, UpdateDate = DateTime.Now, AppId = app.Id }, transaction: transaction);
                    //            _con.Execute(updateDataFund, new { MatchingId = 3, UpdateDate = DateTime.Now, FundId = fundId }, transaction: transaction);

                    //            transaction.Commit();
                    //        }
                    //        _con.Close();
                    //    }
                    //}
                    #endregion
                }

                return Json(new { ResultUpload = resultupload }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult Input()
        {
            return View();
        }

        public ActionResult GetList()
        {
            var result = _context.DataRedemp
                    .Include("Matching")
                    .Include("SA")
                    .Include("MI")
                    .Include("Fund")
                    .Where(x => x.MatchingId == 1 && x.ByInput == true).Select(x => new { Id = x.Id, TransDate = x.TransDate, SA = x.SA.Nama, Fund = x.Fund.Nama, MI = x.MI.Nama, NamaNasabah = x.Nasabah, Nominal = x.Nominal, KeteranganRetur = x.KeteranganRetur, MatchingNama = x.Matching.Nama, MatchingWarna = x.Matching.Warna }).OrderByDescending(x => x.Id).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int id)
        {
            var result = _context.DataRedemp.SingleOrDefault(x => x.Id == id);

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

        [HttpPost]
        public JsonResult Save(DataRedemp data)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            if (data.Id == 0)
            {
                data.CreateDate = DateTime.Now;
                data.UserId = currentUser.Id;
                data.MatchingId = 1;
                _context.DataRedemp.Add(data);
            }
            else
            {
                var redemp = _context.DataRedemp.Single(m => m.Id == data.Id);
                redemp.TransDate = data.TransDate;
                redemp.Nasabah = data.Nasabah;
                redemp.Nominal = data.Nominal;
                redemp.KeteranganRetur = data.KeteranganRetur;
                redemp.SAId = data.SAId;
                redemp.FundId = data.FundId;
                redemp.MIId = data.MIId;
                redemp.UserId = currentUser.Id;
                redemp.UpdateDate = DateTime.Now;
                //_context.Entry(retur).State = EntityState.Modified;
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            bool result = false;
            var u = _context.DataRedemp.Where(x => x.Id == id).FirstOrDefault();
            if (u != null)
            {
                _context.DataRedemp.Remove(u);
                _context.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}