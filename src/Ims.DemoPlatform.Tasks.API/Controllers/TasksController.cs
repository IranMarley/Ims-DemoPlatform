using Ims.DemoPlatform.Core.Enums;
using Ims.DemoPlatform.Tasks.API.Models.DTOs;
using Ims.DemoPlatform.Tasks.API.Services;
using Ims.DemoPlatform.WebApi.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ims.DemoPlatform.Tasks.API.Controllers;

[ApiController]
[Route("tasks")]
[Authorize(Roles = $"{nameof(DefaultRoles.Admin)},{nameof(DefaultRoles.User)}")]
public class TasksController : ControllerBase
{
    private readonly ILogger<TasksController> _logger;
    private readonly ITaskService _taskService;

    public TasksController(ILogger<TasksController> logger, ITaskService taskService)
    {
        _logger = logger;
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var user = User.Identity?.Name ?? "Unknown";
        _logger.LogInformation("User {User} requested all tasks", user);

        var tasks = (await _taskService.GetAllAsync()).ToList();

        _logger.LogInformation("Retrieved {Count} tasks for user {User}", tasks.Count, user);
        return Ok(this.Success(tasks));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = User.Identity?.Name ?? "Unknown";
        _logger.LogInformation("User {User} requested task {TaskId}", user, id);

        var task = await _taskService.GetByIdAsync(id);

        if (task == null)
        {
            _logger.LogWarning("Task {TaskId} not found - requested by user {User}", id, user);
            return NotFound(this.Fail("Task not found."));
        }

        _logger.LogInformation("Task {TaskId} retrieved by user {User}", id, user);
        return Ok(this.Success(task));
    }

    [HttpGet("project/{projectId:guid}")]
    public async Task<IActionResult> GetByProject(Guid projectId)
    {
        var user = User.Identity?.Name ?? "Unknown";
        _logger.LogInformation("User {User} requested tasks for project {ProjectId}", user, projectId);

        var tasks = (await _taskService.GetByProjectIdAsync(projectId)).ToList();

        _logger.LogInformation("Retrieved {Count} tasks for project {ProjectId}", tasks.Count, projectId);
        return Ok(this.Success(tasks));
    }

    [HttpGet("assigned/{userId:guid}")]
    public async Task<IActionResult> GetByAssignedUser(Guid userId)
    {
        var user = User.Identity?.Name ?? "Unknown";
        _logger.LogInformation("User {User} requested tasks assigned to user {AssignedUserId}", user, userId);

        var tasks = (await _taskService.GetByAssignedUserIdAsync(userId)).ToList();

        _logger.LogInformation("Retrieved {Count} tasks assigned to user {AssignedUserId}", tasks.Count, userId);
        return Ok(this.Success(tasks));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        var user = User.Identity?.Name ?? "Unknown";
        _logger.LogInformation("User {User} attempting to create task '{TaskTitle}' from IP: {IpAddress}",
            user, dto.Title, HttpContext.Connection.RemoteIpAddress);

        var task = await _taskService.CreateAsync(dto);

        _logger.LogInformation("Task '{TaskTitle}' (ID: {TaskId}) created by user {User}",
            task.Title, task.Id, user);

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, this.Success(task));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto)
    {
        var user = User.Identity?.Name ?? "Unknown";
        _logger.LogInformation("User {User} attempting to update task {TaskId} from IP: {IpAddress}",
            user, id, HttpContext.Connection.RemoteIpAddress);

        var success = await _taskService.UpdateAsync(id, dto);

        if (!success)
        {
            _logger.LogWarning("Task update failed - {TaskId} not found by user {User}", id, user);
            return NotFound(this.Fail("Task not found."));
        }

        _logger.LogInformation("Task {TaskId} updated by user {User}", id, user);
        return Ok(this.Success());
    }
}