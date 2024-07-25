using System;

namespace Homework_71_izida_kubekova.Models
{
    public class PageViewModel
    {
        public int Page { get; set; }
        public int Total { get; set; }

        public PageViewModel(int count, int page, int pageSize)
        {
            Page = page;
            Total = (int)Math.Ceiling(count / (double) pageSize);
        }

        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < Total;
    }
}