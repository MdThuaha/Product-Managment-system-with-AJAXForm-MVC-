using Mvc5_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using X.PagedList;

namespace Mvc5_Project.Controllers
{
    [Authorize(Roles = "Admin, Customers")]

    public class SellersController : Controller
    {

        private readonly ProductDbContext db = new ProductDbContext();
        // GET: Sellers
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Seller model)
        {
            if (ModelState.IsValid)
            {
                db.Sellers.Add(model);
                db.SaveChanges();
                return PartialView("_Success");
            }
            return PartialView("_Fail");
        }

        [Authorize(Roles = "Admin")]

        public ActionResult Edit(int id)
        {
            var data = db.Sellers.FirstOrDefault(x => x.SellerId == id);
            if (data == null) return new HttpNotFoundResult();
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(Seller model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return PartialView("_Success");
            }
            return PartialView("_Fail");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var seller = db.Sellers.FirstOrDefault(x => x.SellerId == id);
            if (seller == null) return new HttpNotFoundResult();
            db.Sellers.Remove(seller);
            db.SaveChanges();
            return Json(new { success = true });
        }
        public PartialViewResult SellersTable(int pg = 1)
        {
            var data = db.Sellers.OrderBy(x => x.SellerId).ToPagedList(pg, 5);
            return PartialView("_Sellers", data);
        }
    }
}