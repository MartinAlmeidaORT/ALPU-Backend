using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

namespace DataAccess.ExternalServices;

public class AmazonS3Service(IAmazonS3 s3Client, IConfiguration config)
{
    private readonly IAmazonS3 _s3Client = s3Client;
    private readonly string _bucketName = config["AWS:BucketName"]!;

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
