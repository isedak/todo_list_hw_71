using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Homework_71_izida_kubekova.Models;
using Homework_71_izida_kubekova.Models.ViewModels;
using Homework_71_izida_kubekova.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Homework_71_izida_kubekova.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User()
                {
                    Email = model.Email,
                    UserName = model.UserName
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "user");
                    await _signInManager.SignInAsync(user, false);
                    
                    EmailService service = new EmailService();
                    string text = $"Уважаемый <b>{user.UserName}</b>!<br>Рады приветстсвовать Вас на нашем сайте!<br>" +
                                  $"Ваше Имя пользователя: {user.UserName}<br>" +
                                  $"Email: {user.Email}<br>" +
                                  $"Ссылка на профиль: http://localhost:5000/Users/UserDetails/{user.Id}";
                    await service.SendEmailAsync(user.Email, "Добро пожаловать на сайт!", text);
                    return RedirectToAction("Index", "Task");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel() {ReturnUrl = returnUrl});
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            List<User> users = _userManager.Users.ToList();
            if (users.Any(u => u.Email == model.Email))
            {
                User user = await _userManager.FindByEmailAsync(model.Email);
                SignInResult result = await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    model.RememberMe,
                    false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        return Redirect(model.ReturnUrl);
                    return RedirectToAction("Index", "Task");
                }

                ModelState.AddModelError(string.Empty, "Неправильный логин или пароль");
            }

            ViewBag.Message = "Такой пользователь не зарегистрирован";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}