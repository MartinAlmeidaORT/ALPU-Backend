namespace Domain.Common.Inputs;

public record RequestUploadInput
{
    public required string FileName { get; init; }
}

public record ConfirmUploadInput
{
    public required string Key { get; init; }
}
