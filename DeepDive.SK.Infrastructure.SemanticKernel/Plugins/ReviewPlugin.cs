using System.ComponentModel;
using DeepDive.SK.Application;
using DeepDive.SK.Domain.Interfaces;
using Microsoft.SemanticKernel;

namespace DeepDive.SK.Infrastructure.SemanticKernel.Plugins
{
    public class ReviewPlugin(IPullRequestRepository pullRequestRepository,
        IFileReviewService fileReviewService,
        IPullRequestCommenter pullRequestCommenter) : IReviewPlugin
    {
        [KernelFunction("review_pr")]
        [Description("Posts an array of comments to all changed file in a pull request")]
        [return: Description("Number of comments placed")]
        public async Task<int> PostReviewCommentsAsync(
            [Description("The guid of the repository")] Guid repositoryId,
            [Description("The id (integer) of the specific pull request")] int pullRequestId)
        {
            var pullRequests = await pullRequestRepository.GetPullRequests(repositoryId);
            var pullRequest = pullRequests.Find(pr => pr.Id == pullRequestId) ?? throw new ArgumentException("Pull request not found");
            var adjustedFiles = await pullRequestRepository.GetPathsOfAdjustedCSharpFiles(
                repositoryId, pullRequest);

            int count = 0;
            foreach (var filePath in adjustedFiles.FilePaths)
            {
                var content = await pullRequestRepository.GetContentOfFile(repositoryId, adjustedFiles.CommitId, filePath);

                // Review the file using Semantic Kernel
                var reviewComments = await fileReviewService.ReviewFileAsync(content);

                // Post comments to Azure DevOps
                if (FeatureFlags.ActuallyPostReview)
                {
                    await pullRequestCommenter.PostReviewCommentsAsync(repositoryId, pullRequestId, filePath, [reviewComments]);
                }
                count++;
            }

            return count;
        }
    }
}
