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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Comments comment)
        {

            if (comment.Text.Length < 25)
              return RedirectToAction("detail", "product", new { id = comment.ProductId });

            string userId = String.Empty;

            if (User.Identity.IsAuthenticated)
               userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            else
               return RedirectToAction("Login", "Account");

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


        // POST: CommentController/Delete/CommentId
        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            string userId = String.Empty;

            if (User.Identity.IsAuthenticated)
                userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            Comments comment = await _context.CommentProduct.FindAsync(id);
            if(comment==null) return RedirectToAction("Index","Home");

            try
            {
                if (comment.UserId == userId)
                {
                    _context.CommentProduct.Remove(comment);
                    await _context.SaveChangesAsync();
                };

                return RedirectToAction("detail", "product", new { id = comment.ProductId });
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
