using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TaskBoardApi.Hubs;
using TaskBoardApi.Models;
using TaskBoardApi.Services;

namespace TaskBoardApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskService _taskService;
        private readonly IHubContext<TaskHub> _hubContext;

        public TasksController(TaskService taskService, IHubContext<TaskHub> hubContext)
        {
            _taskService = taskService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> Post(TaskItem task)
        {
            await _taskService.AddTaskAsync(task);
            await _hubContext.Clients.All.SendAsync("TaskAdded", task);
            return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, TaskItem updatedTask)
        {
            if (id != updatedTask.Id) return BadRequest();

            await _taskService.UpdateTaskAsync(updatedTask);
            await _hubContext.Clients.All.SendAsync("TaskUpdated", updatedTask);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _taskService.DeleteTaskAsync(id);
            await _hubContext.Clients.All.SendAsync("TaskDeleted", id);
            return NoContent();
        }
    }
}
