using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Infrastrcuture;
using ShoppingApp.Models;

namespace ShoppingApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DataContext _dataContext;

        public ProductsController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IActionResult> Index(string categorySlug="",int p=1)
        {
            int pageSize = 3;
            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
           // ViewBag.CategorySlug = categorySlug;
            if(categorySlug=="")
            {
                ViewBag.TotalPages = (int)Math.Ceiling((decimal)_dataContext.Products.Count() / pageSize);
                var productList = await _dataContext.Products.OrderByDescending(p => p.Id).Skip((p - 1) * pageSize).Take(pageSize).ToListAsync();
                return View(productList);
            }
            Category category =await _dataContext.Categories.Where(c=>c.Slug==categorySlug).FirstOrDefaultAsync();
            if(category==null)
                return RedirectToAction("Index");

            var productsByCategory = _dataContext.Products.Where(p => p.CategoryId == category.Id);
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)productsByCategory.Count() / pageSize);
            var productsInCategory = await productsByCategory.OrderByDescending(p => p.Id).Skip((p - 1) * pageSize).Take(pageSize).ToListAsync();
            return View(productsInCategory);
        }
    }
}
