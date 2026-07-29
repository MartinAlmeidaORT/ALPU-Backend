namespace Domain.Common.Payloads;

public record ProfilePictureUploadPayload
{
    public required string Key { get; init; }

    public required string UploadUrl { get; init; }
}
