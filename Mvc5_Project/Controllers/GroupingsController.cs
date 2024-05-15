using Mvc5_Project.Models;
using Mvc5_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Mvc5_Project.Controllers
{
    public class GroupingsController : Controller
    {
        private readonly ProductDbContext db = new ProductDbContext();
        // GET: Groupings
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GroupingByProductName()
        {
            var data = db.Sales.Include(x => x.Product).ToList()
                .GroupBy(s => s.Product.ProductName)
                .Select(g => new GroupedData { Key = g.Key, Data = g.Select(x => x) })
                .ToList();

            return View(data);
        }
        public ActionResult GroupingBySaleDate()
        {
            var data = db.Sales
                .ToList()
               .GroupBy(s => s.SaleDate)
               .Select(g => new GroupedData { Key = g.Key.ToString("yyyy-MM-dd"), Data = g.Select(x => x) })
               .ToList();
            return View(data);
        }
    }
}