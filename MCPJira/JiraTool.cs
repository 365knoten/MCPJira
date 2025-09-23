using System.ComponentModel;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ModelContextProtocol.Server;

[McpServerToolType, Description("Werkzeuge zur Interaktion mit Jira.")]
public sealed class JiraTool
{
    #region "Constructor"
    private readonly IJiraService _jiraService;

    public JiraTool(IJiraService jiraService)
    {
        _jiraService = jiraService;
    }
    #endregion


    #region getStoriesForUser
    [McpServerTool, Description("Gibt alle Jira-Issues zurück, die dem angegebenen Nutzer zugewiesen sind")
    ]
    public async Task<Issue[]> getStoriesForUser(
        [Description("Der interne Nutzername des Jira-Benutzers, z.B. 'SIS'")] string username
    )
    {
        return await _jiraService.getStoriesForUser(username);
    }
    #endregion


    #region ListJiraIssues
    [McpServerTool, Description("Gibt eine Liste vom Jira-Issues zurück")]
    public async Task<Issue[]> ListJiraIssues(string Filter = "assignee=currentUser()")
    {
        return await _jiraService.query(Filter);
    }
    #endregion


    #region CreateJiraStory
    [
        McpServerTool,Description("Ersellt eine neue User Story in Jira. Eine User Story besteht aus vier Teilen: Als ein <Rolle> möchte ich <Funktion>, um <Nutzen>. Zusätzlich können Akzeptanzkriterien angegeben werden, die mit einem Bindestrich (-) beginnen.")
    ]
    public async Task<string> CreateJiraStory(
        [Description("Kurze Zusammenfassung der User Story")] string summary,
        [Description("Die Rolle, die die User Story betrifft")] string role,
        [Description("Die Funktion, die die User Story beschreibt")] string funktion,
        [Description("Der Nutzen der User Story")] string nutzen,
        [Description("Die Akzeptanzkriterien der User Story, jeweils mit einem Bindestrich (-) beginnend")] string acceptanceCriterias
    )
    {
        var result = await _jiraService.createStory(
            summary,
            role,
            funktion,
            nutzen,
            acceptanceCriterias
        );

        return "Created issue with key: " + result;
    }

    #endregion
}
