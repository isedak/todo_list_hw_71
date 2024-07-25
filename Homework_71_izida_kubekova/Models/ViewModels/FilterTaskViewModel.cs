using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Homework_71_izida_kubekova.Models.ViewModels
{
    public class FilterTaskViewModel
    {
        public FilterTaskViewModel(string creatorId, string workerId, string name, string description, DateTime? dateFrom,
            DateTime? dateTill,
            SelectList priorities, int? priority, SelectList taskStates, int? taskState, string freeTasksForUser)
        {
            CreatorId = creatorId;
            WorkerId = workerId;
            SelectedName = name;
            SelectedDescription = description;
            DateFrom = dateFrom;
            DateTill = dateTill;
            Priorities = priorities;
            SelectedPriority = priority;
            TaskStates = taskStates;
            SelectedTaskState = taskState;
            SelectedTasksForWorker = freeTasksForUser;
        }

        public string CreatorId { get; set; }
        public string WorkerId { get; set; }
        public string SelectedName { get; set; }
        public string SelectedDescription { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTill { get; set; }
        public SelectList Priorities { get; set; }
        public int? SelectedPriority { get; set; }
        public SelectList TaskStates { get; set; }
        public int? SelectedTaskState { get; set; }
        public string SelectedTasksForWorker { get; set; }
    }
}