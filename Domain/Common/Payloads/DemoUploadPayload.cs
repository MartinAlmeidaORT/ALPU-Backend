namespace Domain.Common.Payloads;

public record FormField
{
    public required string Name { get; init; }
    public required string Value { get; init; }
}

public record DemoUploadPayload
{
    public required string Key { get; init; }

    public required string UploadUrl { get; init; }

    public required IReadOnlyList<FormField> Fields { get; init; }
}
