using ShopingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ShopingCart.Controllers
{
    public class StoreController : Controller
    {
        // GET: Store
        public Model1 db;
        public StoreController()
        {
            db = new Model1();
            
        }
        public ActionResult Index()
        {
            var categories = db.Category.ToList();
            return View(categories);
        }
        public ActionResult Browse(string category)
        {
            var categoryModel = db.Category.Include("Items").Single(c => c.Name == category);
            return View(categoryModel);
        }
        public ActionResult Detail(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var items = db.Category.Include("Items").Single(c=>c.CategoryId==id);
            return View(items);
        }
    }
}