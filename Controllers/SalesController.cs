using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FrontToBack.Controllers
{
    public class SalesController : Controller
    {
        private readonly Context _context;
        public SalesController(Context context)
        {
            _context = context;
        }

        // GET: SalesController
        public ActionResult Index()
        {
        
            string basket = Request.Cookies["basket"];

            if (!User.Identity.IsAuthenticated) return RedirectToAction("index", "home");

            // Get id of the logged in user
            string UserID = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Get baskets product from users cookie
            

            List<BasketProduct> products = new List<BasketProduct>();

            if (basket != null)
            {

                products = JsonConvert.DeserializeObject<List<BasketProduct>>(basket);

                // Check special basket for logged in user
                var IsExsist = products.FirstOrDefault(x=>x.UserId == UserID);
                if (IsExsist==null) return RedirectToAction("index", "home");


                foreach (var item in products)
                {

                    Product product = _context.Products.FirstOrDefault(p => p.Id == item.Id);
                    item.Price = product.Price;
                    item.ImageUrl = product.ImageUrl;
                    item.Name = product.Name;
                }

                Response.Cookies.Append("basket", JsonConvert.SerializeObject(products), new CookieOptions { MaxAge = TimeSpan.FromMinutes(14) });

            }

            return View(products);
        }

        // GET: SalesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SalesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SalesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SalesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SalesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SalesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SalesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
