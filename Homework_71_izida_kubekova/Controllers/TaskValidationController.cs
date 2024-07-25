#nullable enable
using System.Linq;
using Homework_71_izida_kubekova.Models.Data;
using Homework_71_izida_kubekova.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Homework_71_izida_kubekova.Controllers
{
    public class TaskValidationController : Controller
    {
        private readonly TaskContext _db;

        public TaskValidationController(TaskContext db)
        {
            _db = db;
        }

        public bool CheckInput(AddTaskViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.TaskModel.Description))
                return false;
            return true;
        }

        public bool CheckName(AddTaskViewModel model)
        {
            var tasks = _db.Tasks.AsQueryable();
            if (model.TaskModel.Id is > 0)
                tasks = tasks.Where(p => p.Id != model.TaskModel.Id);
            if (string.IsNullOrWhiteSpace(model.TaskModel.Name))
                return false;
            return !tasks.Any(b => b.Name.ToLower() == model.TaskModel.Name.ToLower());
        }
    }
}