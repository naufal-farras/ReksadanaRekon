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
    public class SAsController : Controller
    {
        private ApplicationDbContext _context;

        public SAsController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get()
        {
            var result = _context.SA.ToList();

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetById(int id)
        {
            var result = _context.SA.SingleOrDefault(c => c.Id == id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(SA sa)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            if (sa.Id == 0)
            {
                sa.UserId = currentUser.Id;
                sa.CreateDate = DateTime.Now;
                _context.SA.Add(sa);
            }
            else
            {
                var sainDb = _context.SA.Single(m => m.Id == sa.Id);
                sainDb.Nama = sa.Nama;
                sainDb.Code = sa.Code;
                sainDb.UserId = currentUser.Id;
                sainDb.UpdateDate = DateTime.Now;
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            bool result = false;
            var u = _context.SA.Where(x => x.Id == id).FirstOrDefault();
            if (u != null)
            {
                _context.SA.Remove(u);
                _context.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}