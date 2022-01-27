using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Controllers
{
    public class ProductController : Controller
    {
        private readonly Context _context;
        public ProductController(Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.ProductCount = _context.Products.Count();
            List<Product> products = _context.Products.Include(p=>p.Category).Take(8).ToList();
            return View();
        }

        public IActionResult LoadMore(int skip)
        {
            IEnumerable<Product> products = _context.Products.Include(c=>c.Category).Skip(skip).Take(8).ToList();
            return PartialView("_ProductPartial",products);
        }



        public IActionResult Search(string search)
        {
            IEnumerable<Product> products = _context.Products
                .Include(c => c.Category)
                .Where(p => p.Name.ToLower().Contains(search.ToLower()))
                .OrderByDescending(p => p.Id)
                .Take(10)
                .ToList();
            return PartialView("_SearchPartial", products);

        }
        public IActionResult Detail(int id)
        {

            IEnumerable<Comments> comments = _context.Comment
                .Where(c => c.ProductId == id);
          
            Product product = _context.Products
                .Include(c=>c.Category)
                .FirstOrDefault(p => p.Id == id);

            ProductDetailVM _product = new ProductDetailVM
            {
                Name = product.Name,
                Price = product.Price,
                Category = product.Category.Name,
                ImageUrl = product.ImageUrl,
                Comments = comments,
            };
            return View(_product);
        }
          

        // Comment controller

    }
}
