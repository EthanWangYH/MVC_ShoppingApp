using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using ShoppingApp.Models.ViewModels;

namespace ShoppingApp.Infrastrcuture.Components
{
    public class SmallCartViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke() {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");
            SmallCartViewModel smallCartVM;
            if (cart == null||cart.Count==0) {
                smallCartVM = null;
            }
            else
            {
                smallCartVM = new SmallCartViewModel()
                {
                    NumberOfItems = cart.Sum(c => c.Quantity),
                    TotalAmount =  cart.Sum(c => c.Quantity*c.Price)
                };
            }
            return View(smallCartVM);
        }
    }
}
