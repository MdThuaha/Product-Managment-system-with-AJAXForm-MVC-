using Mvc5_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Mvc5_Project.Controllers
{
    public class AggregatesController : Controller
    {
        private readonly ProductDbContext db = new ProductDbContext();
        // GET: Aggregates
        public ActionResult Index()
        {
            var data = db.Products.Include(x => x.Sales).ToList();
            return View(data);
        }
    }
}