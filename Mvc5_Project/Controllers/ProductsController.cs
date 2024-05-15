using Mvc5_Project.Models;
using Mvc5_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using X.PagedList;
using System.Data.Entity;
using Microsoft.Owin.Security.Provider;
using System.Threading;

namespace Mvc5_Project.Controllers
{
    [Authorize(Roles = "Admin, Customers")]
    public class ProductsController : Controller
    {
        private readonly ProductDbContext db = new ProductDbContext();
        // GET: Products
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult ProductTable(int pg = 1)
        {
            
            var data = db.Products
                .Include(x => x.Sales)
                .Include(x => x.Seller)
                .OrderBy(x => x.ProductId)
                .ToPagedList(pg, 5);
            Thread.Sleep(1000);
            return PartialView("_ProductTable", data);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {

            return View();
        }
        public ActionResult CreateForm()
        {
            ProductInputModel model = new ProductInputModel();
            model.Sales.Add(new Sale());
            ViewBag.Sellers = db.Sellers.ToList();
            return PartialView("_CreateForm", model);
        }

        [HttpPost]
        public ActionResult Create(ProductInputModel model, string act = "")
        {
            if (act == "add")
            {
                model.Sales.Add(new Sale());
                foreach (var e in ModelState.Values)
                {
                    e.Errors.Clear();
                    e.Value = null;
                }
            }
            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                model.Sales.RemoveAt(index);
                foreach (var e in ModelState.Values)
                {
                    e.Errors.Clear();
                    e.Value = null;
                }
            }
            if (act == "insert")
            {
                if (ModelState.IsValid)
                {
                    var product = new Product
                    {
                        SellerId = model.SellerId,
                        ProductName = model.ProductName,
                        Price = model.Price,
                        ExpireDate = model.ExpireDate,
                        InStock=model.InStock,
                    };
                    string ext = Path.GetExtension(model.Picture.FileName);
                    string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                    string savePath = Path.Combine(Server.MapPath("~/Pictures"), f);
                    model.Picture.SaveAs(savePath);
                    product.Picture = f;
                    

                    db.Products.Add(product);
                    db.SaveChanges();
                    foreach (var s in model.Sales)
                    {
                        //product.Sales.Add(s);
                        db.Database.ExecuteSqlCommand($@"EXEC InsertSale '{s.SaleDate}', {(int)s.Quantity}, {product.ProductId}");
                    }
                    
                    ProductInputModel newmodel = new ProductInputModel() ;
                    newmodel.Sales.Add(new Sale());
                    ViewBag.Sellers = db.Sellers.ToList();
                    foreach (var e in ModelState.Values)
                    {

                        e.Value = null;
                    }
                    return View("_CreateForm", newmodel);
                }
            }
            ViewBag.Sellers = db.Sellers.ToList();
            return View("_CreateForm", model);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {

            ViewBag.Id = id;
            return View();
        }
        public ActionResult EditForm(int id)
        {
            var data = db.Products.FirstOrDefault(x => x.ProductId == id);
            if (data == null) return new HttpNotFoundResult();
            db.Entry(data).Collection(x => x.Sales).Load();
            ProductEditModel model = new ProductEditModel
            {
                ProductId = id,
                SellerId = data.SellerId,
                ProductName = data.ProductName,
                Price = data.Price,
                ExpireDate = data.ExpireDate,
                InStock = data.InStock,
                Sales = data.Sales.ToList()
            };
            ViewBag.Sellers = db.Sellers.ToList();
            ViewBag.CurrentPic = data.Picture;
            return PartialView("_EditForm", model);
        }

        [HttpPost]
        public ActionResult Edit(ProductEditModel model, string act = "")
        {
            if (act == "add")
            {
                model.Sales.Add(new Sale());
                foreach (var e in ModelState.Values)
                {
                    e.Errors.Clear();
                    e.Value = null;
                }
            }
            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                model.Sales.RemoveAt(index);
                foreach (var e in ModelState.Values)
                {
                    e.Errors.Clear();
                    e.Value = null;
                }
            }
            if (act == "update")
            {
                if (ModelState.IsValid)
                {
                    var product = db.Products.FirstOrDefault(x => x.ProductId == model.ProductId);
                    if (product == null) { return new HttpNotFoundResult(); }
                    product.ProductName = model.ProductName;
                    product.Price = model.Price;
                    product.ExpireDate = model.ExpireDate;
                    product.InStock = model.InStock;
                    product.SellerId = model.SellerId;
                    if (model.Picture != null)
                    {
                        string ext = Path.GetExtension(model.Picture.FileName);
                        string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                        string savePath = Path.Combine(Server.MapPath("~/Pictures"), f);
                        model.Picture.SaveAs(savePath);
                        product.Picture = f;
                    }

                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand($"EXEC DeleteSaleByProductId {product.ProductId}");
                    foreach (var s in model.Sales)
                    {
                        db.Database.ExecuteSqlCommand($"EXEC InsertSale '{s.SaleDate}', {(int)s.Quantity}, {product.ProductId}");
                    }
                }
            }
            ViewBag.Sellers = db.Sellers.ToList();
            ViewBag.CurrentPic = db.Products.FirstOrDefault(x => x.ProductId == model.ProductId)?.Picture;
            return View("_EditForm", model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult CreateSale()
        {
            var model = new Sale();
            ViewBag.Products = db.Products.ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult CreateSale(Sale model)
        {

            if (ModelState.IsValid)
            {
                var sale = new Sale
                {
                    ProductId = model.ProductId,
                    SaleDate = model.SaleDate,
                    Quantity = model.Quantity,
                };
                db.Sales.Add(sale);
                db.SaveChanges();

                return RedirectToAction("Index");

            }
            ViewBag.Products = db.Products.ToList();
            return View(model);
        }
        
        //[Authorize(Roles = "Admin")]
        //public ActionResult EditSale(int id)
        //{
        //    var data = db.Sales.FirstOrDefault(x => x.SaleId == id);
        //    if (data == null) return new HttpNotFoundResult();
        //    ViewBag.Products = db.Products.ToList();
        //    return View(data);
        //}
        //[HttpPost]
        //public ActionResult EditSale(Sale model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(model).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //        return PartialView("_Success");
        //    }
        //    return PartialView("_Fail");
        //}
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var sale = db.Products.FirstOrDefault(x => x.ProductId == id);
            if (sale == null) return new HttpNotFoundResult();
            db.Products.Remove(sale);
            db.SaveChanges();
            return Json(new { success = true });
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteSale(int id)
        {
            var sale = db.Sales.FirstOrDefault(x => x.SaleId == id);
            if (sale == null) return new HttpNotFoundResult();
            db.Sales.Remove(sale);
            db.SaveChanges();
            return Json(new { success = true });
        }
    }

}