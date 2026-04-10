using Ims.DemoPlatform.Tasks.API.Models.DTOs;

namespace Ims.DemoPlatform.Tasks.API.Services;

public interface ITaskService
{
    Task<TaskDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<TaskDto>> GetAllAsync();
    Task<IEnumerable<TaskDto>> GetByProjectIdAsync(Guid projectId);
    Task<IEnumerable<TaskDto>> GetByAssignedUserIdAsync(Guid userId);
    Task<TaskDto> CreateAsync(CreateTaskDto dto, Guid assignedUserId);
    Task<bool> UpdateAsync(Guid id, UpdateTaskDto dto);
    Task<bool> DeleteAsync(Guid id);
}

