using StackExchange.Redis;
using System.Text.Json;
using TaskBoardApi.Models;

namespace TaskBoardApi.Services
{
    public class TaskService
    {
        private readonly IDatabase _db;
        private const string RedisKey = "tasks";
        private const string TaskIdKey = "task:id"; // Contador global para IDs

        public TaskService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            var value = await _db.StringGetAsync(RedisKey);
            if (value.IsNullOrEmpty)
                return new List<TaskItem>();

            return JsonSerializer.Deserialize<List<TaskItem>>(value) ?? new List<TaskItem>();
        }

        public async Task SaveAllTasksAsync(List<TaskItem> tasks)
        {
            var json = JsonSerializer.Serialize(tasks);
            await _db.StringSetAsync(RedisKey, json);
        }

        public async Task AddTaskAsync(TaskItem task)
        {
            var newId = (int)await _db.StringIncrementAsync(TaskIdKey);
            task.Id = newId;

            var tasks = await GetAllTasksAsync();
            tasks.Add(task);
            await SaveAllTasksAsync(tasks);
        }

        public async Task UpdateTaskAsync(TaskItem task)
        {
            var tasks = await GetAllTasksAsync();
            var index = tasks.FindIndex(t => t.Id == task.Id);
            if (index >= 0)
            {
                tasks[index] = task;
                await SaveAllTasksAsync(tasks);
            }
        }

        public async Task DeleteTaskAsync(int id)
        {
            var tasks = await GetAllTasksAsync();
            tasks = tasks.Where(t => t.Id != id).ToList();
            await SaveAllTasksAsync(tasks);
        }
    }
}
