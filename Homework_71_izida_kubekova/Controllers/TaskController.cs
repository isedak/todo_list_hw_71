using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Homework_71_izida_kubekova.Controllers
{
    public enum SortOrder
    {
        NameAsc,
        NameDesc,
        PriorityAsc,
        PriorityDesc,
        StateAsc,
        StateDesc,
        DateAsc,
        DateDesc
    }

    [Authorize]
    public class TaskController : Controller
    {
        private readonly TaskContext _db;
        private readonly UserManager<User> _userManager;
        private readonly TaskCacheService _taskCacheService;

        public TaskController(TaskContext db, UserManager<User> userManager, TaskCacheService taskCacheService)
        {
            _db = db;
            _userManager = userManager;
            _taskCacheService = taskCacheService;
        }
        
        public async Task<IActionResult> Index(string creatorId, string workerId, string name, string description,
            DateTime? dateFrom, DateTime? dateTill,
            int? priority, int? taskState, string freeTasksForUser, int page = 1, SortOrder order = SortOrder.DateDesc)
        {
            int pageSize = 10;
            IEnumerable<TaskModel> tasks = await _taskCacheService.GetList();
            tasks = FilterTasks(tasks, creatorId, workerId, name, description, dateFrom, dateTill, priority, taskState,
                freeTasksForUser);
            tasks = OrderSwitch(order, tasks);

            var taskModels = tasks.ToList();
            int count = taskModels.Count();
            tasks = taskModels.Skip((page - 1) * pageSize).Take(pageSize);

            var priorities = new SelectList(Enum.GetValues(typeof(Priorities)).Cast<Priorities>().Select(p =>
                new SelectListItem
                {
                    Text = p.GetDisplayName().ToString(), Value = ((int) p).ToString()
                }).ToList(), "Value", "Text");
            var taskStates = new SelectList(Enum.GetValues(typeof(TaskState)).Cast<TaskState>().Select(t =>
                new SelectListItem
                {
                    Text = t.GetDisplayName().ToString(), Value = ((int) t).ToString()
                }).ToList(), "Value", "Text");

            IndexViewModel viewModel = new IndexViewModel()
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(order),
                FilterTaskViewModel = new FilterTaskViewModel(creatorId, workerId, name, description, dateFrom,
                    dateTill, priorities, priority, taskStates, taskState, freeTasksForUser),
                Tasks = tasks
            };
            ViewBag.UserId = _userManager.GetUserId(User);
            return View(viewModel);
        }

        [NonAction]
        private IEnumerable<TaskModel> FilterTasks(IEnumerable<TaskModel> tasks, string creatorId, string workerId,
            string name, string description,
            DateTime? dateFrom, DateTime? dateTill,
            int? priority, int? taskState, string freeTasksForUser)
        {
            if (!string.IsNullOrEmpty(freeTasksForUser))
            {
                return tasks.Where(t => t.TaskState == TaskState.New || t.TaskState == TaskState.Opened &&
                    t.WorkerId != freeTasksForUser);
            }

            if (!string.IsNullOrEmpty(creatorId))
                return tasks.Where(t => t.CreatorId == creatorId);
            if (!string.IsNullOrEmpty(workerId))
                return tasks.Where(t => t.WorkerId == workerId);
            if (!string.IsNullOrEmpty(name))
                return tasks.Where(t => t.Name.ToLower() == name.ToLower());
            if (!string.IsNullOrEmpty(description))
                return tasks.Where(t => t.Description.Contains(description));
            if (priority is >= 1 and <= 3)
                return tasks.Where(t => t.Priority == (Priorities) priority);
            if (taskState is >= 1 and <= 3)
                return tasks.Where(t => t.TaskState == (TaskState) taskState);
            if (dateFrom is not null && dateTill is null)
                return tasks.Where(t => t.DateOfCreation > dateFrom);
            if (dateFrom is null && dateTill is not null)
                return tasks.Where(t => t.DateOfCreation < dateTill);
            if (dateFrom is not null && dateTill is not null)
                return tasks.Where(t => t.DateOfCreation > dateFrom && t.DateOfCreation < dateTill);
            return tasks;
        }

        [NonAction]
        private IEnumerable<TaskModel> OrderSwitch(SortOrder order, IEnumerable<TaskModel> taskModels)
        {
            switch (order)
            {
                case SortOrder.NameAsc:
                    return taskModels.OrderBy(t => t.Name);
                case SortOrder.NameDesc:
                    return taskModels.OrderByDescending(t => t.Name);
                case SortOrder.PriorityAsc:
                    return taskModels.OrderBy(t => t.Priority);
                case SortOrder.PriorityDesc:
                    return taskModels.OrderByDescending(t => t.Priority);
                case SortOrder.StateAsc:
                    return taskModels.OrderBy(t => t.TaskState);
                case SortOrder.StateDesc:
                    return taskModels.OrderByDescending(t => t.TaskState);
                case SortOrder.DateAsc:
                    return taskModels.OrderBy(t => t.DateOfCreation);
                default:
                    return taskModels.OrderByDescending(t => t.DateOfCreation);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            TaskModel task = new TaskModel();
            var user = await _userManager.GetUserAsync(User);
            task.Creator = user;
            var priorities = Enum.GetValues(typeof(Priorities)).Cast<Priorities>()
                .Where(p => p != Priorities.All).ToList();
            AddTaskViewModel model = new AddTaskViewModel(task, priorities);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(TaskModel taskModel)
        {
            if (ModelState.IsValid)
            {
                taskModel.TaskState = TaskState.New;
                taskModel.DateOfCreation = DateTime.Now;
                _db.Tasks.Add(taskModel);
                await _db.SaveChangesAsync();
                return RedirectToAction("Details", new {id = taskModel.Id});
            }

            User user = await _userManager.GetUserAsync(User);
            taskModel.Creator = user;
            var priorities = Enum.GetValues(typeof(Priorities)).Cast<Priorities>()
                .Where(p => p != Priorities.All).ToList();
            var model = new AddTaskViewModel(taskModel, priorities);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            TaskModel task = await _taskCacheService.GetTask(id);
            if (task is null)
                return NotFound();
            User user = await _userManager.GetUserAsync(User);
            if (await _userManager.IsInRoleAsync(user, "admin") || user.Id == task.CreatorId)
            {
                var priorities = Enum.GetValues(typeof(Priorities)).Cast<Priorities>()
                    .Where(p => p != Priorities.All).ToList();
                var states = Enum.GetValues(typeof(TaskState)).Cast<TaskState>()
                    .Where(p => p != TaskState.All).ToList();
                List<User> users = _userManager.Users.ToList();
                EditTaskViewModel model = new EditTaskViewModel(task, priorities, states, users);
                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTaskViewModel editTaskViewModel)
        {
            TaskModel task = editTaskViewModel.TaskModel;
            if (task is null)
                return NotFound();
            if (ModelState.IsValid)
            {
                task.DateOfCreation ??= DateTime.Now;
                _db.Tasks.Update(task);
                await _db.SaveChangesAsync();
                return RedirectToAction("Details", new {id = task.Id});
            }

            var priorities = Enum.GetValues(typeof(Priorities)).Cast<Priorities>()
                .Where(p => p != Priorities.All).ToList();
            var states = Enum.GetValues(typeof(TaskState)).Cast<TaskState>()
                .Where(p => p != TaskState.All).ToList();
            List<User> users = _userManager.Users.ToList();
            EditTaskViewModel model = new EditTaskViewModel(task, priorities, states, users);
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            TaskModel task = await _db.Tasks.Include(t => t.Creator).Include(t => t.Worker)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (task is null)
                return NotFound();
            return View(task);
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            TaskModel task = await _taskCacheService.GetTask(id);
            if (task is null)
                return NotFound();
            if (task.TaskState == TaskState.Opened)
            {
                ViewBag.Message = "Открытую задачу нельзя удалить";
                return View(task);
            }

            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id.HasValue)
            {
                var task = new TaskModel {Id = id.Value};
                _db.Entry(task).State = EntityState.Deleted;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> EditState(int id)
        {
            TaskModel task = await _taskCacheService.GetTask(id);
            if (task is null)
                return NotFound();
            var user = await _userManager.GetUserAsync(User);
            if (task.TaskState == TaskState.New)
            {
                task.TaskState = TaskState.Opened;
                task.Worker = user;
                task.WorkerId = user.Id;
                task.DateOfOpen = DateTime.Now;
                _db.Tasks.Update(task);
                await _db.SaveChangesAsync();
                return RedirectToAction("Details", new {task.Id});
            }

            if (task.TaskState == TaskState.Opened)
            {
                if (task.WorkerId == user.Id || await _userManager.IsInRoleAsync(user, "admin"))
                {
                    task.TaskState = TaskState.Closed;
                    task.DateOfClose = DateTime.Now;
                    _db.Tasks.Update(task);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Details", new {task.Id});
                }

                return RedirectToAction("Details", id);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> TakeTask(int id)
        {
            TaskModel task = await _taskCacheService.GetTask(id);
            if (task is null)
                return NotFound();
            var user = await _userManager.GetUserAsync(User);
            if (task.TaskState == TaskState.Opened)
            {
                if (task.WorkerId != user.Id)
                {
                    task.Worker = user;
                    task.WorkerId = user.Id;
                    _db.Tasks.Update(task);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Details", new {id = task.Id});
                }

                ViewBag.Message = "Вы уже взяли эту работу";
                return View("Details", task);
            }

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}