using System.Diagnostics.CodeAnalysis;
using Domain.Models;

namespace Domain.Common.Payloads;

public record ContractUrlPayload
{
    public ContractUrlPayload() { }

    [SetsRequiredMembers]
    public ContractUrlPayload(string pdfAmazonS3Url, Contract contract)
    {
        PdfAmazonS3Url = pdfAmazonS3Url;
        Contract = contract;
    }

    public required string PdfAmazonS3Url { get; init; }
    public required Contract Contract { get; init; }
}
