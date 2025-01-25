namespace DeepDive.SK.Domain.Interfaces;

public interface IPullRequestCommenter
{
    Task PostReviewCommentsAsync(Guid repositoryId, int pullRequestId, string filePath, string[] comments);
}
