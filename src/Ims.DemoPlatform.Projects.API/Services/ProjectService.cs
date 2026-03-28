using Ims.DemoPlatform.Projects.API.Models.Entities;
using Ims.DemoPlatform.Projects.API.Data.Repositories;
using Ims.DemoPlatform.Projects.API.Models.DTOs;

namespace Ims.DemoPlatform.Projects.API.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repository;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(IProjectRepository repository, ILogger<ProjectService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        var project = await _repository.GetByIdAsync(id);
        return project == null ? null : MapToDto(project);
    }

    public async Task<IEnumerable<ProjectDto>> GetAllAsync()
    {
        var projects = await _repository.GetAllAsync();
        return projects.Select(MapToDto);
    }

    public async Task<IEnumerable<ProjectDto>> GetByOwnerIdAsync(Guid ownerId)
    {
        var projects = await _repository.GetByOwnerIdAsync(ownerId);
        return projects.Select(MapToDto);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            OwnerId = dto.OwnerId,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(project);
        _logger.LogInformation("Project '{Name}' (ID: {Id}) created", created.Name, created.Id);
        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProjectDto dto)
    {
        var project = await _repository.GetByIdAsync(id);
        if (project == null) return false;

        if (!string.IsNullOrWhiteSpace(dto.Name))
            project.Name = dto.Name;

        if (dto.OwnerId.HasValue)
            project.OwnerId = dto.OwnerId.Value;

        return await _repository.UpdateAsync(project);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }

    private static ProjectDto MapToDto(Project project) => new()
    {
        Id = project.Id,
        Name = project.Name,
        OwnerId = project.OwnerId,
        CreatedAt = project.CreatedAt
    };
}

