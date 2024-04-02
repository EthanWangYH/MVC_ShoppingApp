using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Infrastrcuture;
using ShoppingApp.Models;
using ShoppingApp.Models.ViewModels;

namespace ShoppingApp.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            CartViewModel cartViewModel = new CartViewModel()
            {
                CartItems = cart,
                GrandTotal = cart.Sum(c => c.Price * c.Quantity)
            };
            return View(cartViewModel);
        }

        public async Task<IActionResult> Add(long id)
        {
            Product product = await _dataContext.Products.FindAsync(id);
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            CartItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();
            if (cartItem == null)
            {
                cart.Add(new CartItem(product));
            }
            else
            {
                cartItem.Quantity += 1;
            }
            HttpContext.Session.SetJson("Cart", cart);
            TempData["Success"] = "The product has been added";
            return Redirect(Request.Headers["Referer"].ToString());
        }
        public IActionResult Decrease(long id)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");
            CartItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();
            if (cartItem.Quantity > 0)
            {
                cartItem.Quantity -= 1;
                TempData["Success"] = "The product has been minused";
            }
            else
            {
                cart.RemoveAll(c => c.ProductId == id);
                TempData["Success"] = "The product has been removed";
            }
            HttpContext.Session.SetJson("Cart", cart);
           
            return RedirectToAction("Index");
        }
        public  IActionResult Remove(long id)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");
            cart.RemoveAll(c => c.ProductId == id);
            if (cart.Count == 0) {
                HttpContext.Session.Remove("Cart");
            }else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            TempData["Success"] = "The product has been removed";
            return RedirectToAction("Index");
        }

        public IActionResult Clear() {
           HttpContext.Session.Remove("Cart");
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
