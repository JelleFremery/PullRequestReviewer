using DeepDive.SK.Domain.Entities;
using DeepDive.SK.Domain.Interfaces;

namespace DeepDive.SK.Application;

public class ReviewerWorkCase(
    IPullRequestRepository pullRequestRepository,
    IFileReviewService fileReviewService,
    IPullRequestCommenter pullRequestCommenter) : IReviewerUseCase
{
    private readonly bool ActuallyPost = false;

    public async Task<List<RepositoryEntity>> SearchRepositories(string? searchTerm)
    {
        var allRepositories = await pullRequestRepository.GetRepositories();
        if (string.IsNullOrEmpty(searchTerm))
        {
            return allRepositories;
        }
        return allRepositories.Where(r => r.Name.Contains(searchTerm)).ToList();
    }

    public async Task ReviewPullRequestAsync(Guid repositoryId, int pullRequestId)
    {
        // Step 1: Retrieve PR details
        var pullRequests = await pullRequestRepository.GetPullRequests(repositoryId);
        var pullRequest = pullRequests.Find(pr => pr.Id == pullRequestId) ?? throw new ArgumentException("Pull request not found");
        var adjustedFiles = await pullRequestRepository.GetPathsOfAdjustedCSharpFiles(
            repositoryId, pullRequest);

        // Step 2: Process each file
        foreach (var filePath in adjustedFiles.FilePaths)
        {
            var content = await pullRequestRepository.GetContentOfFile(repositoryId, adjustedFiles.CommitId, filePath);

            // Review the file using Semantic Kernel
            var reviewComments = await fileReviewService.ReviewFileAsync(content);

            // Post comments to Azure DevOps
            while (ActuallyPost)
            {
                await pullRequestCommenter.PostReviewCommentsAsync(repositoryId, pullRequestId, filePath, [reviewComments]);
            }
        }
    }
}
