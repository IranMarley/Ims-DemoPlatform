namespace Ims.DemoPlatform.Projects.API.Models.DTOs;

public class CreateProjectDto
{
    public required string Name { get; set; }
    public required Guid OwnerId { get; set; }
}

