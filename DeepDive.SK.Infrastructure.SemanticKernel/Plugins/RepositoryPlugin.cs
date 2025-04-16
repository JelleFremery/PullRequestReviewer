using System.ComponentModel;
using DeepDive.SK.Domain.Entities;
using DeepDive.SK.Domain.Interfaces;
using Microsoft.SemanticKernel;

namespace DeepDive.SK.Infrastructure.SemanticKernel.Plugins;

public class RepositoriesPlugin : IRepositoriesPlugin
{
    private readonly IPullRequestRepository _pullRequestRepository;

    public RepositoriesPlugin(IPullRequestRepository pullRequestRepository)
    {
        _pullRequestRepository = pullRequestRepository;
    }

    [KernelFunction("get_repositories")]
    [Description("Gets a list of my repositories")]
    [return: Description("An array of repositories")]
    public async Task<List<RepositoryEntity>> GetRepositories()
    {
        return await _pullRequestRepository.GetRepositories();
    }

    [KernelFunction("get_pull_requests_for_repository")]
    [Description("Gets a list of my repositories")]
    [return: Description("An array of repositories")]
    public async Task<List<PullRequestEntity>> GetPullRequestsForRepository([Description("The guid of the repository")] Guid repositoryId)
    {
        return await _pullRequestRepository.GetPullRequests(repositoryId);
    }
}