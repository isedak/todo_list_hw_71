using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Homework_71_izida_kubekova.Models.Data
{
    public class TaskContext : IdentityDbContext<User>
    {
        public DbSet<TaskModel> Tasks { get; set; }

        public TaskContext(DbContextOptions<TaskContext> options) : base(options)
        {
        }
    }
}