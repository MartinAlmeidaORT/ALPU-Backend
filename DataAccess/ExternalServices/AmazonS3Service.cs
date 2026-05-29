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

    public async Task<Result<(string, string)>> SaveBillProofAsync(string fileName, BillType type)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return Result.Fail("Extensión no permitida");
        }

        var contentType = extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => string.Empty
        };
        var safeName = Path.GetFileName(fileName).Replace(" ", "-").ToLowerInvariant();
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var suffix = Guid.NewGuid().ToString("N")[..4];
        var subfolder = type == BillType.Income ? "incomes" : "expenses";
        var key = $"bills/{subfolder}/{timestamp}-{suffix}-{safeName}";

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = key,
            Verb = HttpVerb.PUT,
            ContentType = contentType,
            Expires = DateTime.UtcNow.AddMinutes(5)
        };

        var uploadUrl = _s3Client.GetPreSignedURL(request);
        return (key, uploadUrl);
    }

    // Save a contract PDF — key format: "Nombre-Apellido-001"
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

    public async Task MoveContractToCancelledAsync(string contractId)
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
}
