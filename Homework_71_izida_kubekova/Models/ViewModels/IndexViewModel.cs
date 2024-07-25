using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Homework_71_izida_kubekova.Models.ViewModels
{
    public class IndexViewModel
    {
        public PageViewModel PageViewModel { get; set; }
        public IEnumerable<TaskModel> Tasks { get; set; }
        public FilterTaskViewModel FilterTaskViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
    }
}