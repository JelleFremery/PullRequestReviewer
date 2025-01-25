
using DeepDive.SK.Domain.Entities;

namespace DeepDive.SK.Application;

public interface IReviewerUseCase
{
    Task<List<RepositoryEntity>> SearchRepositories(string? searchTerm);
    Task ReviewPullRequestAsync(Guid repositoryId, int pullRequestId);
}