using ShopingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopingCart.Controllers
{
    public class CheckOutController : Controller
    {
        // GET: CheckOut
        public Model1 db;
        const string PromoCode = "free";
        public CheckOutController()
        {
            db = new Model1();
        }
        public ActionResult AddressAndPayment()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddressAndPayment(FormCollection values)
        {
            var order = new Order();
            TryUpdateModel(order);
            try
            {
                if(string.Equals(values["PromoCode"], PromoCode, StringComparison.OrdinalIgnoreCase) == false)
                {
                    return View(order);
                }else
                {
                    var cart = ShoppingCart.GetCart(this.HttpContext);
                    order.UserName = User.Identity.Name;
                    order.OrderDate = DateTime.Now;
                    order.Total = cart.GetTotal();
                    db.Order.Add(order);
                    db.SaveChanges();
                    cart.CreateOrder(order);
                    return RedirectToAction("Complete");
                }
            }catch
            {
                return View(order);
            }

        }
        public ActionResult Complete()
        {
            bool isValid = db.Order.Any(o =>  o.UserName == User.Identity.Name);
            if (isValid)
            {
                var order=db.Order.Include("OrderDetail").Where(o => o.UserName == User.Identity.Name).ToList();
                return View(order);
            }else
            {
                return View("Error");
            }
        }

    }
}