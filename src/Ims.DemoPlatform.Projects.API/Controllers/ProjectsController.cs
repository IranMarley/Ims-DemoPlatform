using System.Security.Claims;
using Ims.DemoPlatform.Core.Enums;
using Ims.DemoPlatform.Projects.API.Models.DTOs;
using Ims.DemoPlatform.Projects.API.Services;
using Ims.DemoPlatform.WebApi.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ims.DemoPlatform.Projects.API.Controllers;

[ApiController]
[Route("projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly ILogger<ProjectsController> _logger;
    private readonly IProjectService _projectService;

    public ProjectsController(ILogger<ProjectsController> logger, IProjectService projectService)
    {
        _logger = logger;
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var user = User.Identity?.Name ?? "Unknown";
        _logger.LogInformation("User {User} requested all projects", user);

        IEnumerable<Models.DTOs.ProjectDto> projects;

        if (User.IsInRole(nameof(DefaultRoles.Admin)))
        {
            projects = await _projectService.GetAllAsync();
        }
        else
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized(this.Fail("Invalid user token."));
            projects = await _projectService.GetByOwnerIdAsync(userId.Value);
        }

        var list = projects.ToList();
        _logger.LogInformation("Retrieved {Count} projects for user {User}", list.Count, user);
        return Ok(this.Success(list));
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> GetLookup()
    {
        IEnumerable<ProjectDto> projects;

        if (User.IsInRole(nameof(DefaultRoles.Admin)))
        {
            projects = await _projectService.GetAllAsync();
        }
        else
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized(this.Fail("Invalid user token."));
            projects = await _projectService.GetByOwnerIdAsync(userId.Value);
        }

        var lookup = projects.Select(p => new { p.Id, p.Name });
        return Ok(this.Success(lookup));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = User.Identity?.Name ?? "Unknown";
        _logger.LogInformation("User {User} requested project {ProjectId}", user, id);

        var project = await _projectService.GetByIdAsync(id);

        if (project == null)
        {
            _logger.LogWarning("Project {ProjectId} not found - requested by user {User}", id, user);
            return NotFound(this.Fail("Project not found."));
        }

        if (!IsOwnerOrAdmin(project.OwnerId))
            return Forbid();

        _logger.LogInformation("Project {ProjectId} retrieved by user {User}", id, user);
        return Ok(this.Success(project));
    }

    [HttpGet("owner/{ownerId:guid}")]
    public async Task<IActionResult> GetByOwner(Guid ownerId)
    {
        if (!IsOwnerOrAdmin(ownerId))
            return Forbid();

        var user = User.Identity?.Name ?? "Unknown";
        _logger.LogInformation("User {User} requested projects for owner {OwnerId}", user, ownerId);

        var projects = (await _projectService.GetByOwnerIdAsync(ownerId)).ToList();

        _logger.LogInformation("Retrieved {Count} projects for owner {OwnerId}", projects.Count, ownerId);
        return Ok(this.Success(projects));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
    {
        var user = User.Identity?.Name ?? "Unknown";
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var ownerId))
            return Unauthorized(this.Fail("Invalid user token."));

        _logger.LogInformation("User {User} attempting to create project '{ProjectName}' from IP: {IpAddress}",
            user, dto.Name, HttpContext.Connection.RemoteIpAddress);

        var project = await _projectService.CreateAsync(dto, ownerId);

        _logger.LogInformation("Project '{ProjectName}' (ID: {ProjectId}) created by user {User}",
            project.Name, project.Id, user);

        return CreatedAtAction(nameof(GetById), new { id = project.Id }, this.Success(project));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectDto dto)
    {
        var user = User.Identity?.Name ?? "Unknown";
        _logger.LogInformation("User {User} attempting to update project {ProjectId} from IP: {IpAddress}",
            user, id, HttpContext.Connection.RemoteIpAddress);

        var project = await _projectService.GetByIdAsync(id);
        if (project == null)
        {
            _logger.LogWarning("Project update failed - {ProjectId} not found by user {User}", id, user);
            return NotFound(this.Fail("Project not found."));
        }

        if (!IsOwnerOrAdmin(project.OwnerId))
            return Forbid();

        var success = await _projectService.UpdateAsync(id, dto);
        if (!success)
            return NotFound(this.Fail("Project not found."));

        _logger.LogInformation("Project {ProjectId} updated by user {User}", id, user);
        return Ok(this.Success());
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = nameof(DefaultRoles.Admin))]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = User.Identity?.Name ?? "Unknown";
        _logger.LogWarning("User {User} attempting to DELETE project {ProjectId} from IP: {IpAddress}",
            user, id, HttpContext.Connection.RemoteIpAddress);

        var success = await _projectService.DeleteAsync(id);

        if (!success)
        {
            _logger.LogWarning("Project deletion failed - {ProjectId} not found by user {User}", id, user);
            return NotFound(this.Fail("Project not found."));
        }

        _logger.LogWarning("Project {ProjectId} DELETED by user {User}", id, user);
        return Ok(this.Success());
    }

    private Guid? GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var id) ? id : null;
    }

    private bool IsOwnerOrAdmin(Guid ownerId)
    {
        if (User.IsInRole(nameof(DefaultRoles.Admin))) return true;
        var currentUserId = GetCurrentUserId();
        return currentUserId.HasValue && currentUserId.Value == ownerId;
    }
}

