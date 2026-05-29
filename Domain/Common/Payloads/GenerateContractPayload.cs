using Domain.Models;

namespace Domain.Common.Payloads;

public class GenerateContractPayload
{
    public required Contract Contract { get; init; }

    public required string PdfAmazonS3Url { get; init; }
}
