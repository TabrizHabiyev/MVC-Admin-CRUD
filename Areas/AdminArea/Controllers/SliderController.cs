using FrontToBack.DAL;
using FrontToBack.Extensions;
using FrontToBack.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class SliderController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _env;
        public SliderController(Context context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Slider> sliders = _context.Sliders.ToList();
            return View(sliders);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create(Slider slider)
        {
            if (ModelState["Photo"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                ModelState.AddModelError("Photo", "Do not empty");
            }

            if (!slider.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "only image");
                return View();
            }
            if (slider.Photo.IsCorrectSize(300))
            {
                ModelState.AddModelError("Photo", "300den yuxari ola bilmez");
                return View();
            }

            Slider newSlider = new Slider();

            string fileName = await slider.Photo.SaveImageAsync(_env.WebRootPath, "img");
            newSlider.ImageUrl = fileName;
          
            await  _context.Sliders.AddAsync(newSlider);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index");
        }


    }
}