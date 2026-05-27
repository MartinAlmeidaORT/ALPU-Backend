using System.Diagnostics.CodeAnalysis;

namespace Domain.Common.Payloads;

public record ContractUrlPayload
{
    public ContractUrlPayload() { }

    [SetsRequiredMembers]
    public ContractUrlPayload(string pdfAmazonS3Url)
    {
        PdfAmazonS3Url = pdfAmazonS3Url;
    }

    public required string PdfAmazonS3Url { get; init; }
}
