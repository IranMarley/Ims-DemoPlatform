namespace Ims.DemoPlatform.Tasks.API.Models.DTOs;

public class CreateTaskDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required Guid ProjectId { get; set; }
}

