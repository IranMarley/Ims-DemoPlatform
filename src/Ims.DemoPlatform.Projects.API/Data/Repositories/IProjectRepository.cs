using Ims.DemoPlatform.Projects.API.Models.Entities;

namespace Ims.DemoPlatform.Projects.API.Data.Repositories;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id);
    Task<IEnumerable<Project>> GetAllAsync();
    Task<IEnumerable<Project>> GetByOwnerIdAsync(Guid ownerId);
    Task<Project> CreateAsync(Project project);
    Task<bool> UpdateAsync(Project project);
    Task<bool> DeleteAsync(Guid id);
}

