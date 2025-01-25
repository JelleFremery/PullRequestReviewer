using DeepDive.SK.Domain.Entities;

namespace DeepDive.SK.Domain.Interfaces;

public interface IPullRequestRepository
{
    Task<List<RepositoryEntity>> GetRepositories();
    Task<List<PullRequestEntity>> GetPullRequests(Guid repositoryId);
    Task<AdjustedFilesEntity> GetPathsOfAdjustedCSharpFiles(Guid repositoryId, PullRequestEntity pr);
    Task<string> GetContentOfFile(Guid repositoryId, string commitId, string filePath);
}