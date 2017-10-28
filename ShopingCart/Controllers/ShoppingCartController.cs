using ShopingCart.Models;
using ShopingCart.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopingCart.Controllers
{
    public class ShoppingCartController : Controller
    {
        Model1 db;
        public ShoppingCartController()
        {
            db = new Model1();
        }
        // GET: ShoppingCart
        public ActionResult Index()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()

            };
            return View(viewModel);
        }
        public ActionResult AddToCart(int id)
        {
            var addedItem = db.Item.Single(i => i.ItemId == id);
          
            var cart = ShoppingCart.GetCart(this.HttpContext);

            cart.AddToCart(addedItem);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            string itemName = db.Cart.Single(i => i.RecordId == id).Item.Title;
            int itemCount = cart.RemoveFromCart(id);
            var result = new ShoppingCartRemoveViewModel
            {
                Message=Server.HtmlEncode(itemName)+"從購物車移除",
                CartTotal =cart.GetTotal(),
                CartCount =cart.GetCount(),
                ItemCount=itemCount
            };
            return RedirectToAction("Index");
        }
        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            ViewData["CartCount"] = cart.GetCount();
            return PartialView("CartSummary");
        }
    }
}