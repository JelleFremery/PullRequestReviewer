namespace DeepDive.SK.Domain.Entities;

public sealed record AdjustedFilesEntity(Guid RepositoryId, string CommitId, string[] FilePaths);