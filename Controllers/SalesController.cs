using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FrontToBack.Controllers
{
    public class SalesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly Context _context;
        private readonly IConfiguration _config;
        public SalesController(Context context, UserManager<AppUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _context = context;
            _config = config;
        }


        // GET: SalesController
        public async Task<ActionResult> Index()
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

            ViewBag.User =await  _userManager.FindByIdAsync(UserID);
            return View(products.Where(x => x.UserId == UserID).ToList());
        }



        // POST: SalesController/Sales 
        [HttpPost]
        public async Task<ActionResult> Sales(Sales sales)
        {

            if (!User.Identity.IsAuthenticated) return RedirectToAction("index", "home");

            string UserID = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            AppUser _user = await _userManager.FindByIdAsync(UserID);

            Sales _sales = new Sales();

            _sales.CustomerName = sales.CustomerName;
            _sales.CustomerSurname = sales.CustomerSurname;
            _sales.CustomerEmail = sales.CustomerEmail;
            _sales.CustomerAddress = sales.CustomerAddress;
            _sales.CustomerAddress2 = sales.CustomerAddress2;
            _sales.CustomerZipCode = sales.CustomerZipCode;
            _sales.SaleDate = DateTime.Now;
            _sales.AppUserId = _user.Id;


            List<BasketProduct> UserBaket = JsonConvert.DeserializeObject<List<BasketProduct>>(Request.Cookies["basket"]);
            List<BasketProduct> basketProducts = UserBaket.Where(x => x.UserId == UserID).ToList();

            List<SalesProduct> _salesProducts = new List<SalesProduct>();

            List<Product> dbProducts = new List<Product>();

            foreach (var item in basketProducts)
            {
                Product dbProduct = await _context.Products.FindAsync(item.Id);
                if (dbProduct.Count < item.Count)
                {
                    return RedirectToAction("Index", "Sales");
                }
                dbProducts.Add(dbProduct);
            }


            double total = 0;
            foreach (var item in basketProducts)
            {
                Product dbProduct = dbProducts.Find(x => x.Id == item.Id);

                await UpdateProductCount(dbProduct,item);

                SalesProduct salesProduct = new SalesProduct();
                salesProduct.SalesId = _sales.Id;
                salesProduct.ProductId = dbProduct.Id;
                _salesProducts.Add(salesProduct);
                total += item.Price * dbProduct.Price;
            }

            _sales.SalesProducts = _salesProducts;
            _sales.Total = total;

            await  _context.Sales.AddAsync(_sales);
            await _context.SaveChangesAsync();

            // Send email 
            SendMail(sales.CustomerEmail, _sales);

            return RedirectToAction("Index", "Home");
        }

        private async Task UpdateProductCount(Product product,BasketProduct basketProduct)
        {
            product.Count = product.Count - basketProduct.Count;
            await _context.SaveChangesAsync();
        }


        private void SendMail(string email, Sales sales)
        {
           using (MailMessage mail = new MailMessage())
            {

                string messageBody = ReadEmailTemplate("C:\\Users\\Tabriz\\Desktop\\FrontToBack\\EmailTemplates\\SalesEmail.html");

                AlternateView alternateView = AlternateView.CreateAlternateViewFromString(messageBody,null,"text/html");


                string mailFrom = _config["SMTP_CONNECTION_STRING:SmtpMail"];
                string mailTo = email;
                string smtpClient = _config["SMTP_CONNECTION_STRING:SmtpClient"];
                string smtpMailPassword = _config["SMTP_CONNECTION_STRING:SmtpMailPassword"];
                int smtpPort = Convert.ToInt32(_config["SMTP_CONNECTION_STRING:SmtpPort"]);


                mail.From = new MailAddress(mailFrom);
                mail.To.Add(mailTo);
                mail.Subject = "Invoice";
                mail.Body = messageBody;
                mail.IsBodyHtml = true;
             
                using (SmtpClient smtp = new SmtpClient(smtpClient, smtpPort))
                {
                    smtp.Credentials = new NetworkCredential(mailFrom, smtpMailPassword);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }

        private string ReadEmailTemplate(string TemplatePath)
        {
            string result = "";
            StreamReader streamReader = new StreamReader(TemplatePath);
            result = streamReader.ReadToEnd();
            streamReader.Close();
            return result;
        }


    }
}
