using Domain.Models;

namespace Domain.Common.Payloads;

public record RegisterBillPayload
{
    public RegisterBillPayload() { }

    public RegisterBillPayload(Bill bill, string amazonS3Url)
    {
        Bill = bill;
        AmazonS3Url = amazonS3Url;
    }

    public Bill Bill { get; init; } = null!;

    public string AmazonS3Url { get; init; } = string.Empty;
}
