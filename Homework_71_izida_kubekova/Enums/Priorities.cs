using System.ComponentModel.DataAnnotations;
using Homework_71_izida_kubekova.Models.Data;

namespace Homework_71_izida_kubekova.Models
{
    public enum Priorities
    {
        [Display (Name = "Все")]
        All = 0,
        [Display (Name = "Низкий")]
        Low = 1,
        [Display (Name = "Средний")]
        Middle = 2,
        [Display (Name = "Высокий")]
        High = 3
    }
}