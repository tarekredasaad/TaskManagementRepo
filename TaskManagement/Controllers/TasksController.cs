using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Models;

namespace TaskManagement.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : BaseController
{
    private readonly ITaskService _taskService;

    public TasksController(ControllerParameters controllerParameters, ITaskService taskService) : base(controllerParameters)
    {
        _taskService = taskService;
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskRequest request)
    {
        var task = await _taskService.CreateTaskAsync(CurrentUserId, request);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [Authorize(Policy = "TaskReadPolicy")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskDto>> GetById(Guid id)
    {
        var task = await _taskService.GetTaskByIdAsync(CurrentUserId, id);
        return task is null ? NotFound() : Ok(task);
    }

    [Authorize(Policy = "TaskReadPolicy")]
    [HttpGet]
    public async Task<ActionResult<List<TaskDto>>> GetMine()
    {
        var tasks = await _taskService.GetMyTasksAsync(CurrentUserId);
        return Ok(tasks);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<TaskDto>> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusRequest request)
    {
        var updated = await _taskService.UpdateTaskStatusAsync(CurrentUserId, id, request);
        return updated is null ? NotFound() : Ok(updated);
    }
}
