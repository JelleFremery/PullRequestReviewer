namespace DeepDive.SK.Domain.Interfaces;

public interface IFileReviewService
{
    Task<string> ReviewFileAsync(string fileContent);
}
