using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Homework_71_izida_kubekova.Extentions;
using Homework_71_izida_kubekova.Models;
using Homework_71_izida_kubekova.Models.Data;
using Homework_71_izida_kubekova.Models.ViewModels;
using Homework_71_izida_kubekova.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Homework_71_izida_kubekova.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly TaskContext _db;

        public UsersController(UserManager<User> userManager, TaskContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendMeProfile(string id)
        {
            User foundUser = await _userManager.FindByIdAsync(id);
            if (foundUser is null)
                return NotFound();
            int countClosedTasks = _db.Tasks.Count(t => t.WorkerId == foundUser.Id && t.TaskState == TaskState.Closed);
            int countCreatedTasks = _db.Tasks.Count(t => t.CreatorId == foundUser.Id);
            string title = @$"Профиль {foundUser.UserName}";
            string bodyText =
                @$"Имя пользователя: {foundUser.UserName} <br>Количество созданных заданий: {countCreatedTasks} <br>" +
                @$"Количество закрытых заданий: {countClosedTasks} <br>Список взятых в работу задач: <br>";
            IEnumerable<TaskModel> tasks = await _db.Tasks.Include(t => t.Creator)
                .Where(t => t.WorkerId == foundUser.Id && t.TaskState != TaskState.Closed)
                .ToListAsync();
            tasks = tasks.OrderByDescending(t => t.DateOfCreation).ToList();
            EmailService service = new EmailService();
            User currentUser = await _userManager.GetUserAsync(User);
            string listText = string.Empty;
            foreach (var task in tasks)
            {
                listText +=
                    $"Название: {task.Name} | Приоритет: {task.Priority.GetDisplayName()} |" +
                    $" Дата создания: {task.DateOfCreation?.ToString("dd.MM.yyyy HH:mm")} | Создал: {task.Creator.UserName} | Статус: {task.TaskState.GetDisplayName()} <br>";
            }

            bodyText += listText;
            await service.SendEmailAsync(currentUser.Email, title, bodyText);
            ViewBag.Message = "Письмо отправлено";
            UserDetailsViewModel model = new UserDetailsViewModel()
            {
                Id = foundUser.Id,
                Email = foundUser.Email,
                UserName = foundUser.UserName,
                Tasks = tasks,
                ClosedTasks = countClosedTasks,
                CreatedTasks = countCreatedTasks
                
            };
            return View("UserDetails", model);
    }

        [Authorize]
        public async Task<IActionResult> ChangePassword(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            ChangePasswordViewModel model = new ChangePasswordViewModel {Id = user.Id, Email = user.Email};
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    var _passwordValidator =
                        HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as
                            IPasswordValidator<User>;
                    var _passwordHasher =
                        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;

                    IdentityResult result =
                        await _passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
                    if (result.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                        await _userManager.UpdateAsync(user);
                        EmailService service = new EmailService();
                        string time = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                        string text = $"Уважаемый <b>{user.UserName}</b>, {time} Вы поменяли пароль.<br>" +
                                      $"Ссылка на Ваш профиль: http://localhost:5000/Users/UserDetails/{user.Id}";
                        await service.SendEmailAsync(user.Email, "Вы поменяли пароль", text);
                        return RedirectToAction("UserDetails", new {id = user.Id});
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                ModelState.AddModelError(string.Empty, "Пользователь не найден");
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> UserDetails(string id)
        {
            List<User> users = _userManager.Users.ToList();
            var foundUser = users.FirstOrDefault(u => u.Id == id);
            if (foundUser is null)
                return NotFound();
            IEnumerable<TaskModel> tasks = await _db.Tasks.Include(t => t.Creator)
                .Where(t => t.WorkerId == foundUser.Id && t.TaskState != TaskState.Closed)
                .ToListAsync();
            tasks = tasks.OrderByDescending(t => t.DateOfCreation);
            int countClosedTasks = _db.Tasks.Count(t => t.WorkerId == foundUser.Id && t.TaskState == TaskState.Closed);
            int countCreatedTasks = _db.Tasks.Count(t => t.CreatorId == foundUser.Id);
            UserDetailsViewModel model = new UserDetailsViewModel()
            {
                Id = foundUser.Id,
                Email = foundUser.Email,
                UserName = foundUser.UserName,
                Tasks = tasks,
                ClosedTasks = countClosedTasks,
                CreatedTasks = countCreatedTasks
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            return View(_userManager.Users.ToList());
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();
            IQueryable<TaskModel> workerTasks = _db.Tasks.Where(t => t.WorkerId == id);
            IQueryable<TaskModel> creatorTasks = _db.Tasks.Where(t => t.CreatorId == id);
            if (workerTasks.Any() || creatorTasks.Any())
            {
                ViewBag.Message =
                    "Возьмите/закройте все открытые задачи работника или отредактируйте, если работник был создателем";
            }

            return View(user);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            User foundUser = await _userManager.FindByIdAsync(id);
            if (foundUser is null)
                return NotFound();
            User user = await _userManager.GetUserAsync(User);
            if (foundUser.Id == user.Id)
            {
                EditUserViewModel model = new EditUserViewModel()
                {
                    Id = foundUser.Id,
                    UserName = foundUser.UserName,
                    Email = foundUser.Email
                };
                return View(model);
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            User user = await _userManager.FindByIdAsync(model.Id);
            if (user is null)
                return NotFound();
            if (ModelState.IsValid)
            {
                user.UserName = model.UserName;
                user.Email = model.Email;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    SendEditEmail(user);
                    return RedirectToAction("UserDetails", new {id = user.Id});
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            EditUserViewModel newModel = new EditUserViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
            return View(newModel);
        }
        
        [NonAction]
        private async Task SendEditEmail(User user)
        {
            EmailService service = new EmailService();
            string time = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            string text = $"Уважаемый <b>{user.UserName}</b>!<br>{time} Вы отредактировали профиль.<br>" +
                          $"Имя пользователя: {user.UserName}<br>" +
                          $"Email: {user.Email}<br>" +
                          $"Ссылка на Ваш профиль: http://localhost:5000/Users/UserDetails/{user.Id}";
            await service.SendEmailAsync(user.Email, "Вы отредактировали профиль", text);
        }
    }
}