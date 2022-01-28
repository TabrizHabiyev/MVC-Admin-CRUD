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

            if (comment.Text == null || comment.Text.Length < 25)
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


        // POST: CommentController/Edit/comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Comments comment)
        {
           string userId = String.Empty;

           if (User.Identity.IsAuthenticated)
                userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
           else
                return RedirectToAction("Login", "Account");


            Comments _comment = _context.CommentProduct.FirstOrDefault(c => c.Id == comment.Id);
            if (comment == null) return RedirectToAction("Index", "Home");


            try
            {
                if (_comment.UserId == userId)
                {
                    _comment.Text = comment.Text;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("detail", "product", new { id = comment.ProductId });
                };

                return RedirectToAction("detail", "product", new { id = comment.ProductId });
            }
            catch
            {
                return RedirectToAction("Index", "Home");
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
