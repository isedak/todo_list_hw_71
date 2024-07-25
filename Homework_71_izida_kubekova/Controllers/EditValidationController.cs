using System.Collections.Generic;
using System.Linq;
using Homework_71_izida_kubekova.Models;
using Homework_71_izida_kubekova.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Homework_71_izida_kubekova.Controllers
{
    public class EditValidationController : Controller
    {
        private readonly UserManager<User> _userManager;

        public EditValidationController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public bool CheckEmail(EditUserViewModel model)
        {
            List<User> users = _userManager.Users.ToList();
            users = users.Where(u => u.Id != model.Id).ToList();
            return !users.Any(u => u.Email.ToLower() == model.Email.ToLower());
        }

        public bool CheckUserName(EditUserViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserName))
                return false;
            List<User> users = _userManager.Users.ToList();
            users = users.Where(u => u.Id != model.Id).ToList();
            return !users.Any(u => u.UserName.ToLower() == model.UserName.ToLower());
        }
    }
}