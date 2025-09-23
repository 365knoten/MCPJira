using System.ComponentModel;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

// Check if we should use the STDIO or HTTP Server Transport
// We do this by checking for the --stdio command line argument or the environment variable "UseStdio".
// IF we don't find either, we default to HTTP transport.
var useSTDIO =
    (args.Length > 0 && args.Contains("--stdio", StringComparer.InvariantCultureIgnoreCase))
    || Environment.GetEnvironmentVariable("UseStdio") == "true";


#region "If we are using the STDIO transport"
if (useSTDIO)
{
    Console.WriteLine("Starting MCP Server with STDIO transport...");

    var builder = Host.CreateEmptyApplicationBuilder(settings: null);

    // in stido mode we take the Jira Server URL and the default Project from the environment variable
    builder.Configuration.AddEnvironmentVariables();

    builder.Services.AddMcpServer().WithStdioServerTransport().WithToolsFromAssembly();
    builder.Services.AddScoped<IAPIKeyService, EnvironmentAPIKeyService>();
    builder.Services.AddScoped<IJiraService, JiraService>();

    var app = builder.Build();
    app.Run();
}
#endregion

#region "If we are using the HTTP transport"
else
{
    Console.WriteLine("Starting MCP Server with HTTP transport...");
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Configuration.AddEnvironmentVariables();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<IAPIKeyService, WebSessionAPIKeyService>();
    builder.Services.AddScoped<IJiraService, JiraService>();

    builder.Services.AddMcpServer().WithHttpTransport().WithToolsFromAssembly();
    var app = builder.Build();

    app.UseMiddleware<ApiKeyMiddleware>();
    app.MapMcp();
    var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";
    Console.WriteLine($"Starting server on port {port}...");
    app.Run();
}
#endregion


