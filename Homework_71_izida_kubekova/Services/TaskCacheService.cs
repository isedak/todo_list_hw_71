using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Homework_71_izida_kubekova.Models;
using Homework_71_izida_kubekova.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Homework_71_izida_kubekova.Services
{
    public class TaskCacheService
    {
        private readonly TaskContext _db;
        private readonly IMemoryCache _memoryCache;

        public TaskCacheService(TaskContext db, IMemoryCache memoryCache)
        {
            _db = db;
            _memoryCache = memoryCache;
        }

        public async Task<IEnumerable<TaskModel>> GetList()
        {
            if (!_memoryCache.TryGetValue("tasks_list", out IEnumerable<TaskModel> tasks))
            {
                tasks = await _db.Tasks.Include(t => t.Creator).Include(t => t.Worker).ToListAsync();
                if (tasks.Any())
                {
                    _memoryCache.Set("tasks_list", tasks,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(3)));
                }
            }

            return tasks;
        }

        public async Task<TaskModel> GetTask(int id)
        {
            if (!_memoryCache.TryGetValue(id, out TaskModel task))
            {
                task = await _db.Tasks.Include(t => t.Creator)
                    .Include(t => t.Worker)
                    .FirstOrDefaultAsync(t => t.Id == id);
                if (task != null)
                {
                    _memoryCache.Set(task.Id, task,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(3)));
                }
            }

            return task;
        }
    }
}