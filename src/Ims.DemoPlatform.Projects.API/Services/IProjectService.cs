using Ims.DemoPlatform.Projects.API.Models.DTOs;

namespace Ims.DemoPlatform.Projects.API.Services;

public interface IProjectService
{
    Task<ProjectDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProjectDto>> GetAllAsync();
    Task<IEnumerable<ProjectDto>> GetByOwnerIdAsync(Guid ownerId);
    Task<ProjectDto> CreateAsync(CreateProjectDto dto, Guid ownerId);
    Task<bool> UpdateAsync(Guid id, UpdateProjectDto dto);
    Task<bool> DeleteAsync(Guid id);
}

