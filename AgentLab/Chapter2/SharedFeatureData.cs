using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Course.Chapter2;

// Shared data contracts for the Feature Design Team workflow
// Data Flow: Input Idea → PM → Architect → QA → Feature Card

public sealed class FeatureWorkflowState
{
    [JsonPropertyName("original_idea")]
    [Description("The original feature idea input by the user")]
    public string OriginalIdea { get; set; } = "";

    [JsonPropertyName("current_step")]
    [Description("Current step in the workflow (0=start, 1=PM, 2=Architect, 3=QA, 4=Complete)")]
    public int CurrentStep { get; set; } = 0;

    [JsonPropertyName("pm_output")]
    [Description("Output from the Product Manager agent")]
    public ProductManagerOutput? PMOutput { get; set; }

    [JsonPropertyName("architect_output")]
    [Description("Output from the Architect agent")]
    public ArchitectOutput? ArchitectOutput { get; set; }

    [JsonPropertyName("qa_output")]
    [Description("Output from the QA Engineer agent")]
    public QAEngineerOutput? QAOutput { get; set; }

    [JsonPropertyName("feature_card")]
    [Description("Final aggregated Feature Card")]
    public FeatureCard? FeatureCard { get; set; }

    [JsonPropertyName("iteration_count")]
    [Description("Number of iterations through refinement loops")]
    public int IterationCount { get; set; } = 0;

    [JsonPropertyName("workflow_status")]
    [Description("Current status of the workflow")]
    public WorkflowStatus Status { get; set; } = WorkflowStatus.NotStarted;
}

public enum WorkflowStatus
{
    NotStarted,
    InProgress,
    PendingReview,
    Complete,
    Failed
}

[Description("Product Manager agent output")]
public sealed class ProductManagerOutput
{
    [JsonPropertyName("refined_story")]
    [Description("The refined user story with clear value proposition")]
    public string RefinedStory { get; set; } = "";

    [JsonPropertyName("user_personas")]
    [Description("Identified user personas who benefit from this feature")]
    public List<UserPersona> UserPersonas { get; set; } = new();

    [JsonPropertyName("user_stories")]
    [Description("Broken down user stories in As a... I want... So that... format")]
    public List<UserStory> UserStories { get; set; } = new();

    [JsonPropertyName("success_metrics")]
    [Description("How we measure success of this feature")]
    public List<string> SuccessMetrics { get; set; } = new();

    [JsonPropertyName("priority")]
    [Description("Suggested priority: High, Medium, Low")]
    public string Priority { get; set; } = "Medium";

    [JsonPropertyName("scope_notes")]
    [Description("Notes on scope boundaries and what's in/out")]
    public string ScopeNotes { get; set; } = "";

    [JsonPropertyName("ready_for_architecture")]
    [Description("Whether the story is clear enough for technical design")]
    public bool ReadyForArchitecture { get; set; }
}

public sealed class UserPersona
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("needs")]
    public List<string> Needs { get; set; } = new();
}

public sealed class UserStory
{
    [JsonPropertyName("as_a")]
    [Description("The user role")]
    public string AsA { get; set; } = "";

    [JsonPropertyName("i_want")]
    [Description("The desired action or feature")]
    public string IWant { get; set; } = "";

    [JsonPropertyName("so_that")]
    [Description("The benefit or value")]
    public string SoThat { get; set; } = "";

    [JsonPropertyName("acceptance_criteria")]
    [Description("Testable criteria for this story")]
    public List<string> AcceptanceCriteria { get; set; } = new();
}

[Description("Architect agent output")]
public sealed class ArchitectOutput
{
    [JsonPropertyName("technical_summary")]
    [Description("High-level technical approach summary")]
    public string TechnicalSummary { get; set; } = "";

    [JsonPropertyName("components")]
    [Description("Components/modules needed for this feature")]
    public List<TechnicalComponent> Components { get; set; } = new();

    [JsonPropertyName("data_model")]
    [Description("Data entities and relationships")]
    public List<DataEntity> DataModel { get; set; } = new();

    [JsonPropertyName("api_endpoints")]
    [Description("API endpoints if applicable")]
    public List<ApiEndpoint> ApiEndpoints { get; set; } = new();

    [JsonPropertyName("dependencies")]
    [Description("External dependencies or services required")]
    public List<string> Dependencies { get; set; } = new();

    [JsonPropertyName("technical_risks")]
    [Description("Identified technical risks and mitigations")]
    public List<TechnicalRisk> TechnicalRisks { get; set; } = new();

    [JsonPropertyName("effort_estimate")]
    [Description("T-shirt size effort estimate: XS, S, M, L, XL")]
    public string EffortEstimate { get; set; } = "M";

    [JsonPropertyName("implementation_notes")]
    [Description("Additional notes for implementation")]
    public string ImplementationNotes { get; set; } = "";

    [JsonPropertyName("ready_for_qa")]
    [Description("Whether the design is complete enough for QA planning")]
    public bool ReadyForQA { get; set; }
}

public sealed class TechnicalComponent
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("type")]
    [Description("Component type: Service, UI, Database, API, etc.")]
    public string Type { get; set; } = "";

    [JsonPropertyName("responsibility")]
    public string Responsibility { get; set; } = "";

    [JsonPropertyName("interfaces")]
    public List<string> Interfaces { get; set; } = new();
}

public sealed class DataEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("properties")]
    public List<string> Properties { get; set; } = new();

    [JsonPropertyName("relationships")]
    public List<string> Relationships { get; set; } = new();
}

public sealed class ApiEndpoint
{
    [JsonPropertyName("method")]
    [Description("HTTP method: GET, POST, PUT, DELETE")]
    public string Method { get; set; } = "";

    [JsonPropertyName("path")]
    public string Path { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("request_body")]
    public string? RequestBody { get; set; }

    [JsonPropertyName("response")]
    public string Response { get; set; } = "";
}

public sealed class TechnicalRisk
{
    [JsonPropertyName("risk")]
    public string Risk { get; set; } = "";

    [JsonPropertyName("severity")]
    [Description("Severity: High, Medium, Low")]
    public string Severity { get; set; } = "Medium";

    [JsonPropertyName("mitigation")]
    public string Mitigation { get; set; } = "";
}

[Description("QA Engineer agent output")]
public sealed class QAEngineerOutput
{
    [JsonPropertyName("test_strategy")]
    [Description("Overall testing approach")]
    public string TestStrategy { get; set; } = "";

    [JsonPropertyName("test_cases")]
    [Description("Specific test cases derived from acceptance criteria")]
    public List<TestCase> TestCases { get; set; } = new();

    [JsonPropertyName("edge_cases")]
    [Description("Edge cases and boundary conditions to test")]
    public List<string> EdgeCases { get; set; } = new();

    [JsonPropertyName("non_functional_tests")]
    [Description("Non-functional requirements to verify")]
    public List<NonFunctionalTest> NonFunctionalTests { get; set; } = new();

    [JsonPropertyName("test_data_requirements")]
    [Description("Test data needed for testing")]
    public List<string> TestDataRequirements { get; set; } = new();

    [JsonPropertyName("automation_candidates")]
    [Description("Tests suitable for automation")]
    public List<string> AutomationCandidates { get; set; } = new();

    [JsonPropertyName("qa_sign_off_criteria")]
    [Description("Criteria required for QA sign-off")]
    public List<string> QASignOffCriteria { get; set; } = new();

    [JsonPropertyName("ready_for_feature_card")]
    [Description("Whether QA analysis is complete")]
    public bool ReadyForFeatureCard { get; set; }
}

public sealed class TestCase
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("preconditions")]
    public List<string> Preconditions { get; set; } = new();

    [JsonPropertyName("steps")]
    public List<string> Steps { get; set; } = new();

    [JsonPropertyName("expected_result")]
    public string ExpectedResult { get; set; } = "";

    [JsonPropertyName("priority")]
    [Description("Test priority: P0 (critical), P1 (high), P2 (medium), P3 (low)")]
    public string Priority { get; set; } = "P2";
}

public sealed class NonFunctionalTest
{
    [JsonPropertyName("category")]
    [Description("Category: Performance, Security, Accessibility, Usability")]
    public string Category { get; set; } = "";

    [JsonPropertyName("requirement")]
    public string Requirement { get; set; } = "";

    [JsonPropertyName("measurement")]
    public string Measurement { get; set; } = "";
}

[Description("Complete Feature Card ready for development")]
public sealed class FeatureCard
{
    [JsonPropertyName("feature_id")]
    [Description("Unique feature identifier")]
    public string FeatureId { get; set; } = "";

    [JsonPropertyName("title")]
    [Description("Feature title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("original_idea")]
    [Description("The original input idea")]
    public string OriginalIdea { get; set; } = "";

    [JsonPropertyName("summary")]
    [Description("Executive summary of the feature")]
    public string Summary { get; set; } = "";

    // From PM
    [JsonPropertyName("user_stories")]
    public List<UserStory> UserStories { get; set; } = new();

    [JsonPropertyName("success_metrics")]
    public List<string> SuccessMetrics { get; set; } = new();

    [JsonPropertyName("priority")]
    public string Priority { get; set; } = "Medium";

    // From Architect
    [JsonPropertyName("technical_summary")]
    public string TechnicalSummary { get; set; } = "";

    [JsonPropertyName("components")]
    public List<TechnicalComponent> Components { get; set; } = new();

    [JsonPropertyName("effort_estimate")]
    public string EffortEstimate { get; set; } = "M";

    [JsonPropertyName("technical_risks")]
    public List<TechnicalRisk> TechnicalRisks { get; set; } = new();

    // From QA
    [JsonPropertyName("acceptance_criteria")]
    public List<string> AcceptanceCriteria { get; set; } = new();

    [JsonPropertyName("test_cases")]
    public List<TestCase> TestCases { get; set; } = new();

    [JsonPropertyName("qa_sign_off_criteria")]
    public List<string> QASignOffCriteria { get; set; } = new();

    // Metadata
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("status")]
    [Description("Status: Draft, Ready, Approved")]
    public string Status { get; set; } = "Draft";

    [JsonPropertyName("github_issue_url")]
    [Description("URL to created GitHub issue (if applicable)")]
    public string? GitHubIssueUrl { get; set; }
}

public static class FeatureStateShared
{
    public const string Scope = "FeatureStateScope";
    public const string Key = "singleton";
}
