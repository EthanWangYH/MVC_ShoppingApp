using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Infrastrcuture;
using ShoppingApp.Models;

namespace ShoppingApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductsController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index(int p = 1)
        {
            int pagesize = 3;
            ViewBag.PageNumber = p;
            ViewBag.PageRange = pagesize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)_dataContext.Products.Count() / pagesize);
            var productList1 = await _dataContext.Products
                .OrderByDescending(p => p.Id)
                .Include(p => p.Category)
                .Skip((p - 1) * pagesize)
                .Take(pagesize)
                .ToListAsync();
            return View(productList1);
        }
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            if (ModelState.IsValid)
            {
                product.Slug = product.Name.ToLower().Replace(" ", "-");
                var slug =await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The Slug already exists");
                    return View(product);
                }
                if (product.ImageUpload is not null)
                {
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string filepath = Path.Combine(uploadDir, imageName);
                    FileStream fs = new FileStream(filepath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    product.Image = imageName;
                }
                _dataContext.Products.Add(product);
                await _dataContext.SaveChangesAsync();
                TempData["Success"] = "The editedProduct has been created";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(long id)
        {
            Product product =await _dataContext.Products.FindAsync(id);
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.Category);
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id,Product editedProduct)
        {
            if(editedProduct.Id !=id)
            {
                return NotFound();
            }
            
            
            if(ModelState.IsValid)
            {
                //editedProduct.Slug = editedProduct.Name.ToLower().Replace(" ", "_");
                //var slug =await  _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == editedProduct.Slug);
                //if (slug is not null)
                //{
                //    ModelState.AddModelError("", "The Slug already exists");
                //    return View(editedProduct);
                //}
                var existingdProduct = await _dataContext.Products.FindAsync(id);
                existingdProduct.Name = editedProduct.Name;
                existingdProduct.Price= editedProduct.Price;
                existingdProduct.Description = editedProduct.Description;
                existingdProduct.CategoryId = editedProduct.CategoryId;
                existingdProduct.Category = editedProduct.Category;
                if (editedProduct.ImageUpload is not null)
                {
                    string imageName = Guid.NewGuid().ToString() + "_" + editedProduct.ImageUpload.FileName;
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string filepath = Path.Combine(uploadDir, imageName);
                    FileStream fs = new FileStream(filepath, FileMode.Create);
                    await editedProduct.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    existingdProduct.Image = imageName;
                }
                _dataContext.Update(existingdProduct);
                await _dataContext.SaveChangesAsync();
                TempData["Success"] = "The editedProduct has been edited";
            }
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", editedProduct.CategoryId);
            return View(editedProduct);
        }
     

        public async Task<IActionResult> Delete(long id)
        {
            var product = await _dataContext.Products.FindAsync(id);
            if (product != null)
            {
                if(!string.Equals(product.Image,"NoImage.png"))
                {
                    var uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    var oldPath = Path.Combine(uploadDir, product.Image);
                    if(System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
                _dataContext.Remove(product);
                await _dataContext.SaveChangesAsync();
                TempData["Success"] = "The product has been deleted";
            }
            return RedirectToAction("Index");
        }
    }
}
