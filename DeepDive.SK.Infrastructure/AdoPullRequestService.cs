using DeepDive.SK.Domain.Entities;
using DeepDive.SK.Domain.Interfaces;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace DeepDive.SK.Infrastructure.ADO;

internal class AdoPullRequestService : IPullRequestRepository, IPullRequestCommenter
{
    private readonly string _projectName;

    public AdoPullRequestService()
    {
        _connection = new VssConnection(new Uri(AdoConfig.OrganizationUrl), new VssBasicCredential(string.Empty, AdoConfig.PatToken));
        _projectName = AdoConfig.ProjectName;
    }

    private readonly VssConnection _connection;

    private GitHttpClient? _gitClient;

    public async Task<List<RepositoryEntity>> GetRepositories()
    {
        _gitClient = await GetGitClientCached();
        var repositories = await _gitClient.GetRepositoriesAsync(_projectName);
        return repositories.Select(r => new RepositoryEntity(r.Id, r.Name)).ToList();
    }

    public async Task<List<PullRequestEntity>> GetPullRequests(Guid repositoryId)
    {
        _gitClient = await GetGitClientCached();
        var searchCriteria = new GitPullRequestSearchCriteria
        {
            Status = PullRequestStatus.Active
        };
        List<GitPullRequest> pullRequests = await _gitClient.GetPullRequestsAsync(
            repositoryId,
            searchCriteria);
        if (pullRequests == null)
        {
            return [];
        }
        return pullRequests.Select(pr =>
                new PullRequestEntity(pr.PullRequestId, pr.SourceRefName, pr.TargetRefName, pr.Title, pr.Description)
            ).ToList();
    }

    public async Task<AdjustedFilesEntity> GetPathsOfAdjustedCSharpFiles(Guid repositoryId, PullRequestEntity pr)
    {
        _gitClient = await GetGitClientCached();
        var targetBranch = await _gitClient.GetBranchAsync(repositoryId, pr.TargetBranchName);
        var productionBranchCommitId = targetBranch.Commit.CommitId;

        var sourceBranch = await _gitClient.GetBranchAsync(repositoryId, pr.SourceBranchName);
        var pullRequestBranchCommitId = sourceBranch.Commit.CommitId;

        var response = await _gitClient.GetCommitDiffsAsync(
            repositoryId,
            diffCommonCommit: true,
            baseVersionDescriptor: new GitBaseVersionDescriptor() { Version = productionBranchCommitId, VersionType = GitVersionType.Commit },
            targetVersionDescriptor: new GitTargetVersionDescriptor { Version = pullRequestBranchCommitId, VersionType = GitVersionType.Commit });

        var filePaths = response.Changes.Where(c =>
                c.ChangeType == VersionControlChangeType.Add || c.ChangeType == VersionControlChangeType.Edit)
            .Select(c => c.Item.Path)
            .Where(c => c.EndsWith(".cs")).ToArray();

        return new AdjustedFilesEntity(repositoryId, pullRequestBranchCommitId, filePaths);
    }

    public async Task<string> GetContentOfFile(Guid repositoryId, string commitId, string filePath)
    {
        _gitClient = await GetGitClientCached();
        var item = await _gitClient.GetItemAsync(repositoryId, filePath, includeContent: true, versionDescriptor: new GitVersionDescriptor { VersionType = GitVersionType.Commit, Version = commitId });
        return item.Content;
    }

    public async Task PostReviewCommentsAsync(Guid repositoryId, int pullRequestId, string filePath, string[] comments)
    {
        var commentThread = new GitPullRequestCommentThread
        {
            Status = CommentThreadStatus.Active, // Keeps the comment open
            ThreadContext = new CommentThreadContext
            {
                FilePath = filePath, // Attach comment to the file
                RightFileStart = new CommentPosition { Line = 10, Offset = 1 }, // Line number (optional)
                RightFileEnd = new CommentPosition { Line = 10, Offset = 5 }
            },
            Comments = comments.Select(c => new Comment
            {
                Content = c,
                CommentType = CommentType.Text
            }).ToList()
        };

        _gitClient = await GetGitClientCached();
        await _gitClient.CreateThreadAsync(commentThread, repositoryId, pullRequestId);
    }

    private async Task<GitHttpClient> GetGitClientCached()
    {
        _gitClient ??= await _connection.GetClientAsync<GitHttpClient>();
        if (_gitClient == null)
        {
            throw new InvalidOperationException("Failed to get Git client");
        }
        return _gitClient;
    }
}
