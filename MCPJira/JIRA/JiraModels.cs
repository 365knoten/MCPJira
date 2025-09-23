// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using System.ComponentModel;
using System.Text.Json.Serialization;

public partial class NameOnly
{
    public string? name { get; set; }

    public string toString()
    {
        return this.name ?? "";
    }
}

public class User
{
    [Description("Die UserID dieses Benutzers")]
    public string? name { get; set; }

    [Description("Die E-Mail-Adresse dieses Benutzers")]
    public string? emailAddress { get; set; }

    [Description("Die AnzeigeName dieses Benutzers")]
    public string? displayName { get; set; }
}

public class Fields
{
    public NameOnly? resolution { get; set; }
    public NameOnly? priority { get; set; }
    public User? assignee { get; set; }
    public Status? status { get; set; }
    public List<NameOnly>? components { get; set; }
    public User? creator { get; set; }
    public User? reporter { get; set; }
    public NameOnly? issuetype { get; set; }
    public Project? project { get; set; }
    public string? resolutiondate { get; set; }
    public string? created { get; set; }
    public string? updated { get; set; }
    public string? description { get; set; }
    public string? summary { get; set; }

    [JsonPropertyName("customfield_10400")]
    public NameOnly? estimate { get; set; }
    public string? duedate { get; set; }
}

public class Issue_Raw
{
    public string? id { get; set; }
    public string? key { get; set; }
    public Fields? fields { get; set; }

    public Issue ToIssue()
    {
        var issue = new Issue
        {
            key = this.key,
            resolution = this.fields.resolution?.name,
            priority = this.fields.priority?.name,
            assignee = this.fields.assignee,
            status = this.fields.status,
            components = this.fields.components?.Select(c => c.name).ToList(),
            creator = this.fields.creator,
            reporter = this.fields.reporter,
            issuetype = this.fields.issuetype?.name,
            project = this.fields.project,
            resolutiondate = this.fields.resolutiondate,
            created = this.fields.created,
            updated = this.fields.updated,
            description = this.fields.description,
            Title = this.fields.summary,
            estimate = this.fields.estimate?.name,
            duedate = this.fields.duedate,
        };

        return issue;
    }
}

public class Issue
{
    [Description("Die IssueID dieses Issues")]
    public string key { get; set; }

    public string resolution { get; set; }
    public string priority { get; set; }

    [Description("Die User, dem dieses Issue zugewiesen ist")]
    public User assignee { get; set; }
    public Status status { get; set; }

    [Description("Die Komponenten, die dieses Issue betreffen")]
    public List<string> components { get; set; }

    [Description("Der Ersteller dieses Issues")]
    public User creator { get; set; }

    [Description("Der Benutzer, der dieses Issue gemeldet hat")]
    public User reporter { get; set; }
    public string issuetype { get; set; }

    [Description("Die ID des Projektes")]
    public Project project { get; set; }
    public string resolutiondate { get; set; }
    public string created { get; set; }
    public string updated { get; set; }
    public string description { get; set; }

    [Description("Der Kurztitel dieses Issues")]
    public string Title { get; set; }

    [Description("Die Schätzung in Stunden")]
    public string estimate { get; set; }
    public string duedate { get; set; }
}

public class Project
{
    public string key { get; set; }
    public string name { get; set; }
    public string projectTypeKey { get; set; }
}

public class Root
{
    public List<Issue_Raw> issues { get; set; }

    public List<Issue> ToIssues()
    {
        return issues.Select(i => i.ToIssue()).ToList();
    }
}

public class Status
{
    public string description { get; set; }
    public string name { get; set; }
}

public class CreatedIssueResponse
{
    public string id { get; set; }
    public string key { get; set; }
}
