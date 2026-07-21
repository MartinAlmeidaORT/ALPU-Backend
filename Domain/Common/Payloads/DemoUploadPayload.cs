namespace Domain.Common.Payloads;

public record DemoUploadPayload
{
    public required string Key { get; init; }

    public required string UploadUrl { get; init; }
}
