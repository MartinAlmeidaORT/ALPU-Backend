using Amazon.S3;
using Amazon.S3.Model;
using Domain.Enums;
using FluentResults;
using Microsoft.Extensions.Configuration;

namespace DataAccess.ExternalServices;

public class AmazonS3Service(IAmazonS3 s3Client, IConfiguration config)
{
    private readonly IAmazonS3 _s3Client = s3Client;
    private readonly string _bucketName = config["AWS:BucketName"]!;

    private static readonly string[] ImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly string[] AudioExtensions = [".mp3"];

    public async Task<Result<(string, string)>> SaveBillProofAsync(string fileName, BillType type)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!ImageExtensions.Contains(extension))
        {
            return Result.Fail("Extensión no permitida");
        }

        var subfolder = type == BillType.Income ? "incomes" : "expenses";
        var key = BuildKey($"bills/{subfolder}", fileName);

        var uploadUrl = GetPreSignedPutUrl(key, GetContentType(extension), TimeSpan.FromMinutes(5));
        return (key, uploadUrl);
    }

    public async Task<string> SaveContractAsync(byte[] pdfBytes, int contractId)
    {
        var key = $"contracts/{contractId}.pdf";

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = new MemoryStream(pdfBytes),
            ContentType = "application/pdf",
            // Prevent public access — only your API retrieves these
            CannedACL = S3CannedACL.Private
        };

        await _s3Client.PutObjectAsync(request);
        return key;
    }

    // Get a pre-signed URL so the user can download without exposing the bucket
    public string GetDownloadUrl(string key, int expiryMinutes = 15)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };

        return _s3Client.GetPreSignedURL(request);
    }

    public async Task MoveContractToCancelledAsync(int contractId)
    {
        var sourceKey = $"contracts/{contractId}.pdf";
        var destinationKey = $"contracts/cancelled/{contractId}.pdf";
        // Copy to cancelled folder
        var copyRequest = new CopyObjectRequest
        {
            SourceBucket = _bucketName,
            SourceKey = sourceKey,
            DestinationBucket = _bucketName,
            DestinationKey = destinationKey,
            CannedACL = S3CannedACL.Private
        };
        await _s3Client.CopyObjectAsync(copyRequest);

        // Delete from active folder
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = sourceKey
        };
        await _s3Client.DeleteObjectAsync(deleteRequest);
    }

    // ---- Voice demos (CU13) ----

    // Returns a pre-signed PUT url so the locutor's client can upload the audio file directly.
    // Video demos are handled as YouTube links per RF18 and never touch S3.
    public Result<(string Key, string UploadUrl)> SaveDemoAsync(string fileName, int locutorId)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AudioExtensions.Contains(extension))
        {
            string message = $"Formato de audio no permitido. Formatos aceptados: {string.Join(", ", AudioExtensions)}";
            return Result.Fail(message);
        }

        var key = BuildKey($"demos/{locutorId}", fileName);
        var uploadUrl = GetPreSignedPutUrl(key, GetContentType(extension), TimeSpan.FromMinutes(5));

        return Result.Ok((key, uploadUrl));
    }

    // Pre-signed GET url for the client <audio> element to play the demo.
    // Slightly longer default expiry than a plain download since it's played inline in the banco de voces.
    public string GetDemoPlaybackUrl(string key, int expiryMinutes = 30)
        => GetDownloadUrl(key, expiryMinutes);

    public async Task DeleteDemoAsync(string key)
    {
        await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        });
    }

    // ---- Profile pictures ----

    // Fixed key per locutor (no timestamp) so a new upload naturally overwrites the previous picture.
    // If the extension changes between uploads, call DeleteProfilePictureAsync with the old key first
    // to avoid an orphaned object.
    public Result<(string Key, string UploadUrl)> SaveProfilePictureAsync(string fileName, int locutorId)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!ImageExtensions.Contains(extension))
        {
            return Result.Fail("Extensión no permitida. Formatos aceptados: .jpg, .jpeg, .png");
        }

        var key = $"profile-pictures/{locutorId}{extension}";
        var uploadUrl = GetPreSignedPutUrl(key, GetContentType(extension), TimeSpan.FromMinutes(5));

        return Result.Ok((key, uploadUrl));
    }

    public string GetProfilePictureUrl(string key, int expiryMinutes = 15)
        => GetDownloadUrl(key, expiryMinutes);

    public async Task DeleteProfilePictureAsync(string key)
    {
        await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        });
    }

    // ---- shared helpers ----

    private static string GetContentType(string extension) => extension switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".webp" => "image/webp",
        ".png" => "image/png",
        ".mp3" => "audio/mpeg",
        ".wav" => "audio/wav",
        ".m4a" => "audio/mp4",
        ".ogg" => "audio/ogg",
        _ => "application/octet-stream"
    };

    private static string BuildKey(string prefix, string fileName)
    {
        var safeName = Path.GetFileName(fileName).Replace(" ", "-").ToLowerInvariant();
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var suffix = Guid.NewGuid().ToString("N")[..4];
        return $"{prefix}/{timestamp}-{suffix}-{safeName}";
    }

    private string GetPreSignedPutUrl(string key, string contentType, TimeSpan expiresIn)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = key,
            Verb = HttpVerb.PUT,
            ContentType = contentType,
            Expires = DateTime.UtcNow.Add(expiresIn)
        };

        return _s3Client.GetPreSignedURL(request);
    }
}
