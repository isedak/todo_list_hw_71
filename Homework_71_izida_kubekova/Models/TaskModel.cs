using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Homework_71_izida_kubekova.Models
{
    public class TaskModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "Введите название от 3 до 40 символов")]
        [Remote("CheckName", "TaskValidation", ErrorMessage = "Такое название уже есть", AdditionalFields = "Id")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Remote("CheckInput", "TaskValidation", ErrorMessage = "Введите описание")]
        public string Description { get; set; }

        public string WorkerId { get; set; }

        public string CreatorId { get; set; }

        public User Worker { get; set; }
        public User Creator { get; set; }

        public DateTime? DateOfCreation { get; set; }
        public DateTime? DateOfOpen { get; set; }
        public DateTime? DateOfClose { get; set; }

        [Required(ErrorMessage = "Вы не выбрали приоритет")]
        public Priorities Priority { get; set; }

        public TaskState TaskState { get; set; }
    }
}