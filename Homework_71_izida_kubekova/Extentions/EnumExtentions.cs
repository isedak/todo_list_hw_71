using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Homework_71_izida_kubekova.Extentions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            string displayName;

            displayName = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault()
                .GetCustomAttribute<DisplayAttribute>()?.GetName();

            if (String.IsNullOrEmpty(displayName))
            {
                displayName = enumValue.ToString();
            }

            return displayName;
        }
    }
}