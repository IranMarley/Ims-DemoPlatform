namespace Ims.DemoPlatform.Tasks.API.Services
{
    public interface IProjectService
    {
        Task<bool> ProjectExistsAsync(Guid projectId);
    }
}

