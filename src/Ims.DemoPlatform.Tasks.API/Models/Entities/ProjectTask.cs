namespace Ims.DemoPlatform.Tasks.API.Models.Entities;

public class ProjectTask
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectTaskStatus Status { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? AssignedUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}

