namespace DeepDive.SK.Domain.Entities;

public sealed record RepositoryEntity(Guid RepositoryId, string Name)
{
    public readonly string Id = RepositoryId.ToString();
}
