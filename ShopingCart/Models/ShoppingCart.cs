using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopingCart.Models
{
    public class ShoppingCart
    {
        public Model1 db = new Model1();
        string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";
        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);//從session取出guid or username
            return cart;
        }
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }
        public string GetCartId(HttpContextBase context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] = context.User.Identity.Name;
                }
                else
                {
                    Guid tempCartId = Guid.NewGuid();
                    context.Session[CartSessionKey] = tempCartId.ToString();
                }

            }
            return context.Session[CartSessionKey].ToString();

        }
        public void AddToCart(Item item)
        {
            var cartItem = db.Cart.SingleOrDefault(c => c.CartId == ShoppingCartId && c.ItemId == item.ItemId);
            if (cartItem == null)
            {
                cartItem = new Cart
                {
                    ItemId = item.ItemId,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                db.Cart.Add(cartItem);
            }else
            {
                cartItem.Count++;
            }
            db.SaveChanges();
        }
        public int RemoveFromCart(int id)
        {
            //recordid is primary key
            //cartid == guid or username like session
            //every cart is different item
            var cartItem = db.Cart.Single(cart => cart.CartId == ShoppingCartId && cart.RecordId == id);
            int itemCount = 0;
            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }else
                {
                    db.Cart.Remove(cartItem);
                }
                db.SaveChanges();
            }
            return itemCount;
        }
        public void EmptyCart()
        {
            var cartItems = db.Cart.Where(cart => cart.CartId == ShoppingCartId);
            //找出username or guid 是某人的
            foreach(var cartItem in cartItems)
            {
                db.Cart.Remove(cartItem);
            }
            //一個一個移除
            db.SaveChanges();

        }
        public List<Cart> GetCartItems()
        {
            return db.Cart.Where(cart => cart.CartId == ShoppingCartId).ToList();
        }
        public int GetCount()
        {
            int? count = (from cartItems in db.Cart
                          where cartItems.CartId == ShoppingCartId
                          select cartItems.Count).Sum();
            return count ?? 0;
        }
        public decimal GetTotal()
        {
            decimal? total= (from cartItems in db.Cart
                             where cartItems.CartId == ShoppingCartId
                             select (decimal?)cartItems.Item.Price).Sum();
            return total ?? decimal.Zero;

        }
        public int CreateOrder(Order order)
        {
            decimal orderTotal = 0;
            var cartItems = GetCartItems();
            foreach(var item in cartItems)
            {
                // 每個OrderDetail是item
                var orderDetail = new OrderDetail
                {
                    ItemId = item.ItemId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Item.Price,
                    Quantity = item.Count
                };
                orderTotal += (item.Count * item.Item.Price);
                db.OrderDetail.Add(orderDetail);
            }
            order.Total = orderTotal;
            db.SaveChanges();
            EmptyCart();
            return order.OrderId;
        }

        public void MigrateCart(string email)
        {
            var shoppingCart = db.Cart.Where(c => c.CartId == ShoppingCartId);
            foreach(Cart item in shoppingCart)
            {
                item.CartId =email;
            }
            db.SaveChanges();
        }
    }
}