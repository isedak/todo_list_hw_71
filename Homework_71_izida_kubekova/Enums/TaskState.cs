using System.ComponentModel.DataAnnotations;
using Homework_71_izida_kubekova.Models.Data;

namespace Homework_71_izida_kubekova.Models
{
    public enum TaskState
    {
        [Display (Name = "Все")]
        All = 0,
        
        [Display (Name = "Новая")]
        New = 1,

        [Display (Name = "Открыта")]
        Opened = 2,

        [Display (Name = "Закрыта")]
        Closed = 3
    }
}