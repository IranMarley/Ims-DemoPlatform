using Ims.DemoPlatform.Tasks.API.Data.Repositories;
using Ims.DemoPlatform.Tasks.API.Models.DTOs;
using Ims.DemoPlatform.Tasks.API.Models.Entities;

namespace Ims.DemoPlatform.Tasks.API.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly ILogger<TaskService> _logger;
    private readonly IProjectService _projectService;

    public TaskService(ITaskRepository repository, ILogger<TaskService> logger, IProjectService projectService)
    {
        _repository = repository;
        _logger = logger;
        _projectService = projectService;
    }

    public async Task<TaskDto?> GetByIdAsync(Guid id)
    {
        var task = await _repository.GetByIdAsync(id);
        return task == null ? null : MapToDto(task);
    }

    public async Task<IEnumerable<TaskDto>> GetAllAsync()
    {
        var tasks = await _repository.GetAllAsync();
        return tasks.Select(t => MapToDto(t));
    }

    public async Task<IEnumerable<TaskDto>> GetByProjectIdAsync(Guid projectId)
    {
        var tasks = await _repository.GetByProjectIdAsync(projectId);
        return tasks.Select(t => MapToDto(t));
    }

    public async Task<IEnumerable<TaskDto>> GetByAssignedUserIdAsync(Guid userId)
    {
        var tasks = await _repository.GetByAssignedUserIdAsync(userId);
        return tasks.Select(t => MapToDto(t));
    }

    public async Task<TaskDto> CreateAsync(CreateTaskDto dto, Guid assignedUserId)
    {
        var projectExists = await _projectService.ProjectExistsAsync(dto.ProjectId);
        if (!projectExists)
        {
            throw new ArgumentException("ProjectId does not exist.");
        }

        var task = new ProjectTask
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Status = ProjectTaskStatus.ToDo,
            ProjectId = dto.ProjectId,
            AssignedUserId = assignedUserId,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(task);
        _logger.LogInformation("Task '{Title}' (ID: {Id}) created in project {ProjectId}", created.Title, created.Id, created.ProjectId);
        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateTaskDto dto)
    {
        var task = await _repository.GetByIdAsync(id);
        if (task == null) return false;

        if (!string.IsNullOrWhiteSpace(dto.Title))
            task.Title = dto.Title;

        if (dto.Description != null)
            task.Description = dto.Description;

        if (dto.Status.HasValue)
            task.Status = dto.Status.Value;

        return await _repository.UpdateAsync(task);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }

    private static TaskDto MapToDto(ProjectTask task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        Status = task.Status,
        ProjectId = task.ProjectId,
        AssignedUserId = task.AssignedUserId,
        CreatedAt = task.CreatedAt
    };
}
