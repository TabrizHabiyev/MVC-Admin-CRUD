using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FrontToBack.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public AccountController(
            UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager ,IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = new AppUser
            {
                FullName = register.FullName,
                UserName = register.UserName,
                Email = register.Email

            };
            //user.IsActive = true;
            IdentityResult identityResult = await _userManager.CreateAsync(user, register.Password);

            if (!identityResult.Succeeded)
            {

                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(user, "Member");
            await _signInManager.SignInAsync(user, true);

            return RedirectToAction("Index", "Home");
        }
        public IActionResult CheckSignIn()
        {
            return Content(User.Identity.IsAuthenticated.ToString());
        }


        public IActionResult Login()
        {

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid) return View();
            AppUser dbUser = await _userManager.FindByNameAsync(login.UserName);

            if (dbUser == null)
            {
                ModelState.AddModelError("", "UserName or Password invalid");
                return View();
            }

            var singInResult = await _signInManager.PasswordSignInAsync(dbUser, login.Password, true, true);

            if (!dbUser.IsActive)
            {
                ModelState.AddModelError("", "user is deactive");
                return View();
            }

            if (singInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "is lockout");
                return View();
            }
            if (!singInResult.Succeeded)
            {
                ModelState.AddModelError("", "UserName or Password invalid");
                return View();
            }

            var roles = await _userManager.GetRolesAsync(dbUser);
            if (roles[0] == "Admin")
            {
                return RedirectToAction("Index", "Dashboard", new { area = "AdminArea" });
            };

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task CreateRole()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
            }
            if (!await _roleManager.RoleExistsAsync("Member"))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "Member" });
            }
        }


        //Forget password
        public IActionResult ForgetPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgotPassword model)
        {
            AppUser user = await _userManager.FindByEmailAsync(model.User.Email);
            if (user == null) return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var link = Url.Action(nameof(ResetPassword), "Account", new {email= user.Email,token },Request.Scheme);

            using (MailMessage mail = new MailMessage())
            {
                string mailFrom = _config["SMTP_CONNECTION_STRING:SmtpMail"];
                string mailTo = model.User.Email;
                string smtpClient = _config["SMTP_CONNECTION_STRING:SmtpClient"];
                string smtpMailPassword = _config["SMTP_CONNECTION_STRING:SmtpMailPassword"];
                int smtpPort = Convert.ToInt32(_config["SMTP_CONNECTION_STRING:SmtpPort"]);

                mail.From = new MailAddress(mailFrom);
                mail.To.Add(mailTo);
                mail.Subject = "Reset Password";
                mail.Body = $"<a href={link}>Got to reset password</a>";
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient(smtpClient, smtpPort))
               {
                smtp.Credentials = new NetworkCredential(mailFrom, smtpMailPassword);
                smtp.EnableSsl = true;
                smtp.Send(mail);
                }

                return View("Index","Home");
            }
        }

        public async Task<IActionResult> ResetPassword(string email,string token)
        {
           

            AppUser user = await _userManager.FindByEmailAsync(email);

            if (user == null) return NotFound();

            ForgotPassword forgetPassword = new ForgotPassword
            {
                Token = token,
                User =  user
            };

            return View(forgetPassword);
        }



        [HttpPost]
        [ActionName("ResetPassword")]
        public async Task<IActionResult> Reset(ForgotPassword model)
        {
            AppUser user = await _userManager.FindByEmailAsync(model.User.Email);
            if (user == null) return NotFound();

            ForgotPassword forgetPassword = new ForgotPassword
            {
                Token = model.Token,
                User = model.User
            };

           IdentityResult result =  await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }
            return RedirectToAction("Index", "Home");
        }

    }
}