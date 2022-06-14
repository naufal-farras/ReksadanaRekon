using Dapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ReksadanaRekon.Models;
using ReksadanaRekon.Models.Master;
using ReksadanaRekon.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _excel = Microsoft.Office.Interop.Excel;

namespace ReksadanaRekon.Controllers.Master
{
    public class RekeningsController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["ReksadanaRekon"].ToString());
        // GET: Rekenings
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get()
        {
            var result = _context.Rekening
                .Include("SA")
                .Include("MI")
                .Include("Fund").ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int id)
        {
            var result = _context.Rekening.SingleOrDefault(x => x.Id == id);

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
        public JsonResult Save(Rekening rek)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            if (rek.Id == 0)
            {
                rek.CreateDate = DateTime.Now;
                rek.UserId = currentUser.Id;
                _context.Rekening.Add(rek);
            }
            else
            {
                var rekInDb = _context.Rekening.Single(m => m.Id == rek.Id);
                rekInDb.NoRek = rek.NoRek;
                rekInDb.NamaRek = rek.NamaRek;
                rekInDb.SAId = rek.SAId;
                rekInDb.FundId = rek.FundId;
                rekInDb.MIId = rek.MIId;
                rekInDb.UserId = currentUser.Id;
                rekInDb.UpdateDate = DateTime.Now;
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            bool result = false;
            var u = _context.Rekening.Where(x => x.Id == id).FirstOrDefault();
            if (u != null)
            {
                _context.Rekening.Remove(u);
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

                                file.SaveAs(path);

                                workbook = workbooks.Open(path);
                                worksheet = workbook.ActiveSheet;
                                range = worksheet.UsedRange;

                                string judul = ((_excel.Range)range.Cells[1, 1]).Text;

                                if (judul == "Template Input Data Rekening Reksadana Aplikasi RENA")
                                {
                                    int csuccess = 0, cfails = 0;

                                    #region Aplikasi
                                    for (int row = 4; row <= range.Rows.Count; row++)
                                    {
                                        int SAId = 0, FundId = 0, MIId = 0;

                                        string NoRek = ((_excel.Range)range.Cells[row, 2]).Text;
                                        string NamaRek = ((_excel.Range)range.Cells[row, 3]).Text;

                                        string SACode = ((_excel.Range)range.Cells[row, 4]).Text;
                                        string SAName = ((_excel.Range)range.Cells[row, 5]).Text;
                                        string FundCode = ((_excel.Range)range.Cells[row, 6]).Text;
                                        string FundName = ((_excel.Range)range.Cells[row, 7]).Text;
                                        string MICode = ((_excel.Range)range.Cells[row, 8]).Text;
                                        string MIName = ((_excel.Range)range.Cells[row, 9]).Text;

                                        if (NoRek == "" || SACode == "" || FundCode == "" || MICode == "")
                                        {
                                            continue;
                                        }

                                        #region CheckMasterSA
                                        var resultSA = _con.QueryFirstOrDefault<SA>("SELECT Id FROM SAs WHERE Code = @SACode", new { SACode = SACode });

                                        if (resultSA == null)
                                        {

                                            var SA_Id = _con.QuerySingle<int>("INSERT INTO SAs (Code, Nama, CreateDate, UserId) OUTPUT INSERTED.Id VALUES (@SACode, @SAName, @DateNow, @UserId);", new { SACode = SACode, SAName = SAName, DateNow = DateTime.Now, UserId = currentUser.Id });

                                            SAId = SA_Id;
                                        }
                                        else
                                        {
                                            SAId = resultSA.Id;
                                        }

                                        #endregion

                                        #region CheckMasterFund
                                        var resultFund = _con.QueryFirstOrDefault<Fund>("SELECT Id FROM Funds WHERE Code = @FundCode AND Nama = @FundName", new { FundCode = FundCode, FundName = FundName });
                                        if (resultFund == null)
                                        {
                                            var Fund_Id = _con.QuerySingle<int>("INSERT INTO Funds (Code, Nama, CreateDate, UserId) OUTPUT INSERTED.Id VALUES (@FundCode, @FundName, @DateNow, @UserId);", new { FundCode = FundCode, FundName = FundName, DateNow = DateTime.Now, UserId = currentUser.Id });

                                            FundId = Fund_Id;
                                        }
                                        else
                                        {
                                            FundId = resultFund.Id;
                                        }
                                        #endregion

                                        #region CheckMasterMI
                                        var resultMI = _con.QueryFirstOrDefault<MI>("SELECT Id FROM MIs WHERE Code = @MICode AND Nama = @MIName", new { MICode = MICode, MIName = MIName });

                                        if (resultMI == null)
                                        {
                                            var MI_Id = _con.QuerySingle<int>("INSERT INTO MIs (Code, Nama, CreateDate, UserId) OUTPUT INSERTED.Id VALUES (@MICode, @MIName, @DateNow, @UserId);", new { MICode = MICode, MIName = MIName, DateNow = DateTime.Now, UserId = currentUser.Id });

                                            MIId = MI_Id;
                                        }
                                        else
                                        {
                                            MIId = resultMI.Id;
                                        }
                                        #endregion

                                        #region CheckDataRekening
                                        var checkData = _con.QueryFirst<int>("SELECT COUNT(*) FROM Rekenings WHERE NoRek = @Norek AND SAId = @SAId AND FundId = @FundId AND MIId = @MIId",
                                                        new { NoRek, SAId, FundId, MIId });

                                        if (checkData == 0)
                                        {
                                            var CreateDate = DateTime.Now;
                                            var UserId = currentUser.Id;
                                            bool IsDelete = false;

                                            string sql = "INSERT INTO Rekenings (NoRek, NamaRek, SAId, FundId, MIId, CreateDate, UserId, IsDelete) VALUES (@NoRek, @NamaRek, @SAId, @FundId, @MIId, @CreateDate, @UserId, @IsDelete)";
                                            _con.Execute(sql, new { NoRek, NamaRek, SAId, FundId, MIId, CreateDate, UserId, IsDelete });
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

                                workbooks.Close();
                                application.Quit();
                                System.IO.File.Delete(path);



                                //Marshal.ReleaseComObject(workbook);
                                //Marshal.ReleaseComObject(workbooks);
                                //Marshal.ReleaseComObject(application);

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