using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _requestDelegate;
    private readonly IConfiguration _configuration;

    public ApiKeyMiddleware(RequestDelegate requestDelegate, IConfiguration configuration)
    {
        _requestDelegate = requestDelegate;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        #region "check for api key in header"
        if (!context.Request.Headers.TryGetValue("X-API-KEY", out var apiKeyVal))
        {
            Console.WriteLine("Api Key not found in request headers.");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Api Key not found!");
            return;
        }
        #endregion

        #region "take api key and validate it against jira"
        HttpClient HTTPClient = new HttpClient()
        {
            BaseAddress = new Uri(_configuration["JiraBaseUrl"]),
            DefaultRequestHeaders =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", apiKeyVal),
            },
        };

        try
        {
            Console.WriteLine("Begin querying Jira for authentication...");

            var mySelf = await HTTPClient.GetFromJsonAsync<MySelfResponse>(
                $"${_configuration["JiraBaseUrl"]}/rest/api/2/myself"
            );

            if (mySelf == null || string.IsNullOrEmpty(mySelf.key))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client");
            }

            Console.WriteLine($"User authenticated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized client");
        }

        #endregion
        await _requestDelegate(context);
    }
}

public class MySelfResponse
{
    public string key { get; set; }
    public string displayName { get; set; }
}
