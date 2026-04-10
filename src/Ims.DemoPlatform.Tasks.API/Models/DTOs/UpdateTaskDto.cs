using Ims.DemoPlatform.Tasks.API.Models.Entities;

namespace Ims.DemoPlatform.Tasks.API.Models.DTOs;

public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public ProjectTaskStatus? Status { get; set; }
}

