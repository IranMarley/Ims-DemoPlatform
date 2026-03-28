using Ims.DemoPlatform.Tasks.API.Models.Entities;

namespace Ims.DemoPlatform.Tasks.API.Data.Repositories;

public interface ITaskRepository
{
    Task<ProjectTask?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProjectTask>> GetAllAsync();
    Task<IEnumerable<ProjectTask>> GetByProjectIdAsync(Guid projectId);
    Task<IEnumerable<ProjectTask>> GetByAssignedUserIdAsync(Guid userId);
    Task<ProjectTask> CreateAsync(ProjectTask task);
    Task<bool> UpdateAsync(ProjectTask task);
    Task<bool> DeleteAsync(Guid id);
}

