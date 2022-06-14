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
    public class MIsController : Controller
    {
        private ApplicationDbContext _context;

        public MIsController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get()
        {
            var result = _context.MI.ToList();

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetById(int id)
        {
            var result = _context.MI.SingleOrDefault(c => c.Id == id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(MI mi)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            if (mi.Id == 0)
            {
                mi.UserId = currentUser.Id;
                mi.CreateDate = DateTime.Now;
                _context.MI.Add(mi);
            }
            else
            {
                var miinDb = _context.MI.Single(m => m.Id == mi.Id);
                miinDb.Nama = mi.Nama;
                miinDb.Code = mi.Code;
                miinDb.UserId = currentUser.Id;
                miinDb.UpdateDate = DateTime.Now;
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            bool result = false;
            var u = _context.MI.Where(x => x.Id == id).FirstOrDefault();
            if (u != null)
            {
                _context.MI.Remove(u);
                _context.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}