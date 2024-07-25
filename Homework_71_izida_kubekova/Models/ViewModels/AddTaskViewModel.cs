using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Homework_71_izida_kubekova.Models.ViewModels
{
    public class AddTaskViewModel
    {
        public AddTaskViewModel(TaskModel taskModel, List<Priorities> priorities)
        {
            TaskModel = taskModel;
            Priorities = priorities;
        }

        public AddTaskViewModel()
        {
        }

        public TaskModel TaskModel { get; set; }
        public List<Priorities> Priorities { get; set; }
    }
}