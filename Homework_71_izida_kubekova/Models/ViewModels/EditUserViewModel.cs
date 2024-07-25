using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Homework_71_izida_kubekova.Models.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "не указано имя пользователя")]
        [Remote("CheckUserName", "EditValidation", ErrorMessage = "Такое имя пользователя уже существует",
            AdditionalFields = "Id")]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Вы ввели не Эл. адрес")]
        [Required(ErrorMessage = "Не указан Эл. адрес")]
        [Remote("CheckEmail", "EditValidation",
            ErrorMessage = "Пользователь с таким электронным адресом уже существует", AdditionalFields = "Id")]
        public string Email { get; set; }
    }
}