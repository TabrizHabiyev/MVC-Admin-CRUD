using FrontToBack.DAL;
using FrontToBack.Extensions;
using FrontToBack.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class ProductController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _env;
        public ProductController(Context context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            
            return View();
        }
        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult>Create(Product product)
        {
           if (ModelState["Photo"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
           {
               ModelState.AddModelError("Photo", "Do not empty");
           }
           if (!product.Photos.IsImage())
           {
               ModelState.AddModelError("Photo", "only image");
               return View();
           }
           if (product.Photos.IsCorrectSize(300))
           {
               ModelState.AddModelError("Photo", "300den yuxari ola bilmez");
               return View();
           }
           Product newProduct = new Product();
           string fileName = await product.Photos.SaveImageAsync(_env.WebRootPath, "img");
           newProduct.ImageUrl = fileName;
           newProduct.Name = product.Name;
           newProduct.Price = product.Price;
           newProduct.CategoryId = product.CategoryId;
           
           await  _context.Products.AddAsync(newProduct);
           await _context.SaveChangesAsync();
           return Ok("ok");
        }
      
    }
}
