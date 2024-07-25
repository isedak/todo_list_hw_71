using Microsoft.AspNetCore.Identity;

namespace Homework_71_izida_kubekova.Services
{
    public class RussianIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError() => new IdentityError
        {
            Code = nameof(DefaultError), 
            Description =
                "Возникла ошибка"
        };

        public override IdentityError PasswordMismatch() => new IdentityError
        {
            Code = nameof(PasswordMismatch), 
            Description =
                "Указан некорректный пароль"
        };

        public override IdentityError LoginAlreadyAssociated() => new IdentityError
        {
            Code = nameof(LoginAlreadyAssociated), 
            Description =
                "Пользователь с таким логином уже вошёл в приложение"
        };

        public override IdentityError InvalidUserName(string userName) => new IdentityError
        {
            Code = nameof(InvalidUserName), 
            Description =
                $"Имя пользователя {userName} неккоректно, вы можете использовать только буквы и цифры"
        };

        public override IdentityError InvalidEmail(string email) => new IdentityError
        {
            Code = nameof(InvalidEmail), 
            Description =
                $"Некорректная почта {email}"
        };

        public override IdentityError DuplicateUserName(string userName) => new IdentityError
        {
            Code = nameof(DuplicateUserName), 
            Description =
                $"Имя пользователя {userName} уже используется"
        };

        public override IdentityError DuplicateEmail(string email) => new IdentityError
        {
            Code = nameof(DuplicateEmail), 
            Description =
                $"Данная почта {email} уже используется"
        };

        public override IdentityError PasswordTooShort(int length) => new IdentityError
        {
            Code = nameof(PasswordTooShort), 
            Description =
                $"Минимальная длина пароля составляет {length} символов"
        };

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) => new IdentityError
        {
            Code = nameof(PasswordRequiresUniqueChars), 
            Description =
                $"Пароль должен содержать {uniqueChars} символы"
        };

        public override IdentityError PasswordRequiresNonAlphanumeric() => new IdentityError
        {
            Code = nameof(PasswordRequiresNonAlphanumeric), 
            Description =
                "Пароль не содержит буквенно-цифровых символов"
        };

        public override IdentityError PasswordRequiresDigit() => new IdentityError
        {
            Code = nameof(PasswordRequiresDigit), 
            Description =
                "Пароль должен содержать хотя бы одну цифру"
        };

        public override IdentityError PasswordRequiresLower() => new IdentityError
        {
            Code = nameof(PasswordRequiresLower), 
            Description =
                "Пароль должен содержать одну букву в нижнем регистре"
        };

        public override IdentityError PasswordRequiresUpper() => new IdentityError
        {
            Code = nameof(PasswordRequiresUpper), 
            Description =
                "Пароль должен содержать одну букву в верхнем регистре"
        };
    }
}