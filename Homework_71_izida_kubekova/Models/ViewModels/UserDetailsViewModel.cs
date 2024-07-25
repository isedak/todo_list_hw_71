using System.Collections.Generic;

namespace Homework_71_izida_kubekova.Models.ViewModels
{
    public class UserDetailsViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IEnumerable<TaskModel> Tasks { get; set; }
        public int ClosedTasks { get; set; }
        public int CreatedTasks { get; set; }
    }
}