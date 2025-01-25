namespace DeepDive.SK.Domain.Entities;

public sealed record PullRequestEntity(
    int Id,
    string SourceBranch,
    string TargetBranch,
    string Title,
    string Description)
{
    public string SourceBranchName => SourceBranch.Replace("refs/heads/", string.Empty);
    public string TargetBranchName => TargetBranch.Replace("refs/heads/", string.Empty);
}
