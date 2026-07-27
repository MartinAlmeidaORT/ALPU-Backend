namespace Domain.Common.Inputs;

public record UploadDemoInput()
{
    public string Key { get; init; } = null!;

    public int LanguageId { get; init; }

    public string Title { get; init; } = null!;
}
