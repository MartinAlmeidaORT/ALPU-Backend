namespace Domain.Common.Payloads;

public record BillUrlPayload
{
    public required string AmazonS3Url { get; set; }
}
