using System.ComponentModel;
using DeepDive.SK.Domain.Interfaces;

namespace DeepDive.SK.Infrastructure.SemanticKernel.Plugins
{
    public interface IReviewPlugin : IKernelPlugin
    {
        Task<int> PostReviewCommentsAsync([Description("The guid of the repository")] Guid repositoryId, [Description("The id (integer) of the specific pull request")] int pullRequestId);
    }
}