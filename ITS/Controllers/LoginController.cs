using Core.Utilities.MailService.Abstract;
using Core.Utilities.MailService.Models;
using Core.Utilities.Security.Hashing;
using KobiAsITS.Data;
using KobiAsITS.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IO = System.IO;

namespace KobiAsITS.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMailService _mailService;
        public LoginController(ApplicationDbContext context, IMailService mailService)
        {
            _context = context;
            _mailService = mailService;
        }

        public IActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string email, string password)
        {
            var user = _context.Users.Include(p => p.Role).Include(p => p.Department).FirstOrDefault(x => x.Email == email);
            if (user == null)
            {
                ViewBag.LoginError = "Bu bilgiler ile sisteme kayıtlı kullanıcı bulunamamıştır.";
                return View();
            }

            if (!HashingHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                ViewBag.LoginError = "Hatalı şifre!";
                return View();
            }




            var expires = DateTimeOffset.Now.AddDays(1);
            List<Claim> userClaims = new List<Claim>();
            userClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Uuid));
            userClaims.Add(new Claim(ClaimTypes.Name, $"{user.UserFirstName} {user.UserLastName}"));
            userClaims.Add(new Claim(ClaimTypes.Role, user.Role.Name));
            userClaims.Add(new Claim("Departmant", user.Department.DepartmentName));
            userClaims.Add(new Claim("Uuid", user.Uuid));
            userClaims.Add(new Claim("UserId", user.Id.ToString()));
            userClaims.Add(new Claim("DepartmentId", user.Department.Id.ToString()));

            var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                //ExpiresUtc = expires
            };

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            Response.Cookies.Append("UserId", user.Id.ToString(), new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = expires,
                HttpOnly = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict
            });

            Response.Cookies.Append("Uuid", user.Uuid, new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = expires,
                HttpOnly = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict
            });

            //var userId = Request.Cookies["UserId"]; // Cookie okuma

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies.ContainsKey("UserId"))
                Response.Cookies.Delete("UserId");
            if (Request.Cookies.ContainsKey("Uuid"))
                Response.Cookies.Delete("Uuid");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(p => p.Email == resetPasswordViewModel.Email);
                if (user != null)
                {
                    Guid resetPasswordCode = Guid.NewGuid();
                    user.ResetPasswordCode = resetPasswordCode;
                    user.ResetPasswordExpired = DateTime.Now.AddHours(24);
                    await _context.SaveChangesAsync();
                    
                    string templateFilePath = IO.Path.Combine(Environment.CurrentDirectory, "wwwroot", "assets", "email_templates", "ResetPasswordTemplate.html");
                    if (IO.File.Exists(templateFilePath))
                    {
                        string host = $"{Request.Scheme}://{Request.Host}";
                        string urlPath = Url.Action("PasswordChange", "Login", new { code = resetPasswordCode.ToString() });
                        string fullName = $"{user.UserFirstName} {user.UserLastName}";
                        string body = IO.File.ReadAllText(templateFilePath);
                        body = body.Replace("{{Name}}", fullName);
                        body = body.Replace("{{ActionUrl}}", $"{host}{urlPath}");

                        var send = _mailService.SendMail(new MailInformation()
                        {
                            MailPerson = new List<MailPerson>()
                            {
                                 new MailPerson()
                                 {
                                      EmailAddress = user.Email,
                                      FullName = fullName
                                 }
                            },
                            Subject = "Şifre Sıfırlama | Kobi Talep Takip Sistemi",
                            Body = body
                        });
                        if (send)
                            ViewBag.Success = "E-mail adresinize şifre sıfırlama bağlantısı gönderilmiştir.";
                        else
                            ViewBag.Error = "Sistemde oluşan bir hata nedeniyle şifre sıfırlama bağlantısı gönderilemedi. Lütfen daha sonra tekrar deneyiniz.";
                    }
                }
                else
                    ViewBag.Error = "E-mail adresi sistemde bulunamadı!";
            }
            else
                ViewBag.Error = "Mail Adresi boş geçilemez!";

            return View();
        }

        public async Task<IActionResult> PasswordChange([FromQuery] Guid code)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ResetPasswordCode == code && u.ResetPasswordExpired > DateTime.Now);
            if (user == null)
            {
                ViewBag.Error = "Geçersiz sıfırlama kodu! Lütfen yeniden şifre sıfırlama isteği yapınız.";
            }
            return View(new PasswordChangeViewModel()
            {
                ResetPasswordCode = code
            });
        }


        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel passwordChangeViewModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ResetPasswordCode == passwordChangeViewModel.ResetPasswordCode && u.ResetPasswordExpired > DateTime.Now);
            if (user != null)
            {
                if (passwordChangeViewModel.Password == passwordChangeViewModel.PasswordRepeat)
                {
                    byte[] passwordHash, passwordSalt;
                    HashingHelper.CreatePasswordHash(passwordChangeViewModel.Password, out passwordHash, out passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.ResetPasswordCode = null;
                    user.ResetPasswordExpired = null;
                    await _context.SaveChangesAsync();
                    ViewBag.Success = "Şifreniz başarıyla sıfırlandı. Giriş yapabilirsiniz.";
                }
                else
                    ViewBag.Error = "Şifreler eşleşmiyor!";
            }
            return View();
        }

    }
}