using System.Net.Http.Headers;

namespace Ims.DemoPlatform.Tasks.API.Services
{
    public class ProjectService : IProjectService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProjectService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _projectsApiBaseUrl;

        public ProjectService(
            HttpClient httpClient,
            ILogger<ProjectService> logger,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _projectsApiBaseUrl = configuration["ProjectsApi:BaseUrl"] ?? "";
        }

        public async Task<bool> ProjectExistsAsync(Guid projectId)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers.Authorization
                    .ToString().Replace("Bearer ", "");

                if (!string.IsNullOrEmpty(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{_projectsApiBaseUrl}/projects/{projectId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if project exists: {ProjectId}", projectId);
                return false;
            }
        }
    }
}

