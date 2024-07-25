using Homework_71_izida_kubekova.Controllers;

namespace Homework_71_izida_kubekova.Models
{
    public class SortViewModel
    {
        public SortOrder NameSort { get; set; }
        public SortOrder PrioritySort { get; set; }
        public SortOrder StateSort { get; set; }
        public SortOrder DateSort { get; set; }
        public SortOrder Current { get; set; }

        public SortViewModel(SortOrder order)
        {
            NameSort = order == SortOrder.NameAsc ? SortOrder.NameDesc : SortOrder.NameAsc;
            PrioritySort = order == SortOrder.PriorityAsc ? SortOrder.PriorityDesc : SortOrder.PriorityAsc;
            StateSort = order == SortOrder.StateAsc ? SortOrder.StateDesc : SortOrder.StateAsc;
            DateSort = order == SortOrder.DateAsc ? SortOrder.DateDesc : SortOrder.DateAsc;
            Current = order;
        }
    }
}