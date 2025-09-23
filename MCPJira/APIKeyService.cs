using Microsoft.AspNetCore.Http;

public interface IAPIKeyService
{
    string getAPIKey();
}

public class EnvironmentAPIKeyService : IAPIKeyService
{
    public string getAPIKey() => Environment.GetEnvironmentVariable("X-API-KEY");
}

public class WebSessionAPIKeyService : IAPIKeyService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WebSessionAPIKeyService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string getAPIKey()
    {
        return _httpContextAccessor.HttpContext.Request.Headers["X-API-KEY"];
    }
}
