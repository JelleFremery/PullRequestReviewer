using System.ComponentModel;
using DeepDive.SK.Domain.Entities;
using DeepDive.SK.Domain.Interfaces;

namespace DeepDive.SK.Infrastructure.SemanticKernel.Plugins
{
    public interface IRepositoriesPlugin : IKernelPlugin
    {
        Task<List<PullRequestEntity>> GetPullRequestsForRepository([Description("The guid of the repository")] Guid repositoryId);
        Task<List<RepositoryEntity>> GetRepositories();
    }
}