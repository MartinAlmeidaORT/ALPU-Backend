using Domain.Enums;
using FluentResults;

namespace Domain.Interfaces.Private;

public interface IAmazonS3Service
{
    static readonly long MaxDemoFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    Task<Result<(string, string)>> SaveBillProofAsync(string fileName, BillType type);

    Task<string> SaveContractAsync(byte[] pdfBytes, int contractId);

    string GetDownloadUrl(string key, int expiryMinutes = 15);

    Task<string> MoveContractToCancelledAsync(int contractId);

    Result<(string Key, string Url, IReadOnlyDictionary<string, string> Fields)> SaveDemoAsync(string fileName, int locutorId);

    Task<Result> SaveDemoAsync(string key);

    string GetDemoPlaybackUrl(string key, int expiryMinutes = 30);

    Task DeleteDemoAsync(string key);

    Result<(string Key, string UploadUrl)> SaveProfilePictureAsync(string fileName, int locutorId);

    string GetProfilePictureUrl(string key, int expiryMinutes = 15);

    Task DeleteProfilePictureAsync(string key);
}
