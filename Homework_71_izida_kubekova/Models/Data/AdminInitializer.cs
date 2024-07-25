using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Homework_71_izida_kubekova.Models.Data
{
    public class AdminInitializer
    {
        public static async Task SeedAdmin(RoleManager<IdentityRole> _roleManager, UserManager<User> _userManager)
        {
            string adminEmail = "isedak81@yandex.ru";
            string adminPassword = "Super123!";
            string adminUserName = "Admin";
            string[] roles = {"admin", "user"};
            foreach (var role in roles)
            {
                if (await _roleManager.FindByNameAsync(role) is null)
                    await _roleManager.CreateAsync(new IdentityRole(role));
            }

            if (await _userManager.FindByEmailAsync(adminEmail) is null)
            {
                User admin = new User()
                {
                    Email = adminEmail,
                    UserName = adminUserName
                };
                var result = await _userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                    await _userManager.AddToRoleAsync(admin, "admin");
            }
        }
    }
}