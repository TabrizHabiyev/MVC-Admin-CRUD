using FrontToBack.DAL;
using FrontToBack.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FrontToBack.Controllers
{
    
    public class CommentController : Controller
    {
        private readonly Context _context;
        public CommentController( Context context)
        {
            _context = context;
        }

        // POST: CommentController/Create
        [HttpPost]
        public async Task<ActionResult> Create(Comments comment)
        {

            if (comment.Text.Length < 25)
              return RedirectToAction("detail", "product", new { id = comment.ProductId });

            string userId = null;
            if (User.Identity.IsAuthenticated)
               userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            else
            {
              return RedirectToAction("Login", "Account");
            }

            try
            {
                Comments _comment = new Comments
                {
                    Text = comment.Text,
                    UserId = userId,
                    ProductId = comment.ProductId,
                    Date = DateTime.Now
                };

                await _context.CommentProduct.AddAsync(_comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("detail","product", new { id = comment.ProductId });
            }
            catch
            {
                return View();
            }
        }




        // GET: CommentController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CommentController/Edit/5
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


        // POST: CommentController/Delete/5
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
