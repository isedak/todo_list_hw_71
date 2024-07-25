using System.Collections.Generic;

namespace Homework_71_izida_kubekova.Models.ViewModels
{
    public class EditTaskViewModel
    {
        public EditTaskViewModel(TaskModel taskModel, List<Priorities> priorities, List<TaskState> states, List<User> users)
        {
            TaskModel = taskModel;
            Priorities = priorities;
            States = states;
            Users = users;
        }

        public EditTaskViewModel()
        {
        }

        public TaskModel TaskModel { get; set; }
        public List<Priorities> Priorities { get; set; }
        public List<TaskState> States { get; set; }
        public List<User> Users { get; set; }
    }
}