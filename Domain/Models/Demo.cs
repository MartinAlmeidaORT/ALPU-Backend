using Domain.Common;
using Domain.Common.Errors;
using FluentResults;

namespace Domain.Models;

public class Demo : Entity
{
    internal Demo() { }

    public static Result<Demo> CreateDemo(int broadcasterId, string fileKey, Language language, string title)
    {
        if (string.IsNullOrWhiteSpace(fileKey))
        {
            return Result.Fail(DemoErrors.FileKeyIsRequired());
        }

        return Result.Ok(new Demo
        {
            BroadcasterId = broadcasterId,
            FileName = fileKey,
            Language = language,
            Title = title
        });
    }

    public int BroadcasterId { get; set; }

    public string FileName { get; set; } = null!;

    public string LanguageId { get; set; } = null!;

    public Language Language { get; set; } = null!;

    public string Title { get; set; } = null!;

    public virtual Broadcaster Broadcaster { get; set; } = null!;
}

public static class DemoErrors
{
    public class FileKeyIsRequiredError(string msg) : BadRequestError(msg);
    public class DemoNotFoundError(string msg) : NotFoundError(msg);

    public static FileKeyIsRequiredError FileKeyIsRequired() => new("Se debe especificar la clave del archivo de audio.");
    public static DemoNotFoundError DemoNotFound(string fileKey) => new($"No se encontro una demo con la clave {fileKey}.");
}
