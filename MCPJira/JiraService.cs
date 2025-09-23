using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

public interface IJiraService
{
    Task<Issue[]> getStoriesForUser(string username);

    Task<Issue[]> query(string filter);

    Task<string> createStory(
        string summary,
        string asA,
        string iWantTo,
        string because,
        string acceptanceCriterias
    );
}

public class JiraService : IJiraService
{
    #region "Constructor"
    private IAPIKeyService _apiKeyService;
    private IConfiguration _configuration;

    public JiraService(IAPIKeyService apiKeyService, IConfiguration configuration)
    {
        _apiKeyService = apiKeyService;
        _configuration = configuration;
    }

    private HttpClient createHttpClient()
    {
        string key = _apiKeyService.getAPIKey();

        return new HttpClient()
        {
            BaseAddress = new Uri(_configuration["JiraBaseUrl"]),
            DefaultRequestHeaders =
            {
                Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    key
                ),
            },
        };
    }
    #endregion


    #region "getStoriesForUser"
    public async Task<Issue[]> getStoriesForUser(string username)
    {
        Console.WriteLine($"Querying for all Jira issues assigned to: {username}");

        var HTTPClient = createHttpClient();

        return (
            await HTTPClient.GetFromJsonAsync<Root>(
                $"{_configuration["JiraBaseUrl"]}/rest/api/2/search?jql=assignee={Uri.EscapeDataString(username)}"
            )
        )
            .ToIssues()
            .ToArray();

    }
    #endregion


    #region createStory
    public async Task<string> createStory(
        string summary,
        string asA,
        string iWantTo,
        string because,
        string acceptanceCriterias
    )
    {
        #region "create story description"
        // Create the summary using the summary of the story using this template
        string description = @"{panel:title=UserStory||titleBGColor=#90ee90}
{color:#008b00}*Als ein *{color} --ASA--
{color:#008b00}*möchte ich *{color} --IWANTTO--
{color:#008b00}*damit *{color} --BECAUSE--
{panel}

{panel:title=Acceptance Criterias|titleBGColor=#add8e6}
--ACCEPTANCECRITERIAS--
{panel}
".Replace("--ASA--", asA).Replace("--IWANTTO--", iWantTo).Replace("--BECAUSE--", because).Replace(
            "--ACCEPTANCECRITERIAS--",
            acceptanceCriterias
        );

        #endregion


        var HTTPClient = createHttpClient();

        var newIssue = new
        {
            fields = new
            {
                project = new { key = _configuration["JiraProjectKey"] },
                summary = summary,
                description = description,
                issuetype = new { name = "Story" },
            },
        };

        var response = await HTTPClient.PostAsJsonAsync(
            $"{_configuration["JiraBaseUrl"]}/rest/api/2/issue",
            newIssue
        );
        response.EnsureSuccessStatusCode();

        var createdIssue = await response.Content.ReadFromJsonAsync<CreatedIssueResponse>();
        return createdIssue.key;
    }

    #endregion

    #region query
    public async Task<Issue[]> query(string filter)
    {
        Console.WriteLine($"Querying Jira with Filter: {filter}");

        var HTTPClient = createHttpClient();

        return (
            await HTTPClient.GetFromJsonAsync<Root>(
                $"{_configuration["JiraBaseUrl"]}/rest/api/2/search?jql={Uri.EscapeDataString(filter)}"
            )
        )
            .ToIssues()
            .ToArray();

    }
    #endregion
}
