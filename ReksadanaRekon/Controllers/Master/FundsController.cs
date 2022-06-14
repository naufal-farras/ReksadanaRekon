using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ReksadanaRekon.Models;
using ReksadanaRekon.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReksadanaRekon.Controllers.Master
{
    public class FundsController : Controller
    {
        private ApplicationDbContext _context;

        public FundsController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get()
        {
            var result = _context.Fund.ToList();

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetById(int id)
        {
            var result = _context.Fund.SingleOrDefault(c => c.Id == id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(Fund fu)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            if (fu.Id == 0)
            {
                fu.UserId = currentUser.Id;
                fu.CreateDate = DateTime.Now;
                _context.Fund.Add(fu);
            }
            else
            {
                var fuinDb = _context.Fund.Single(m => m.Id == fu.Id);
                fuinDb.Nama = fu.Nama;
                fuinDb.Code = fu.Code;
                fuinDb.UserId = currentUser.Id;
                fuinDb.UpdateDate = DateTime.Now;
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            bool result = false;
            var u = _context.Fund.Where(x => x.Id == id).FirstOrDefault();
            if (u != null)
            {
                _context.Fund.Remove(u);
                _context.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}