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

        if (!fileKey.StartsWith($"demos/{broadcasterId}/", StringComparison.Ordinal))
        {
            return Result.Fail(DemoErrors.InvalidFileKey());
        }

        Demo newDemo = new()
        {
            BroadcasterId = broadcasterId,
            FileName = fileKey,
            Language = language,
            Title = title
        };
        Result errors = newDemo.ValidateDemo();
        return errors.IsFailed ? errors : newDemo;
    }

    public Result ValidateDemo()
    {
        return Result.Merge(
            ValidateTitle(),
            ValidateLanguage()
        );
    }

    public Result ValidateTitle()
    {
        if (Title == null) return Result.Fail(DemoErrors.TitleIsRequired());

        if (Title.Length < 5)
        {
            return Result.Fail(DemoErrors.TitleMinLength());
        }

        if (Title.Length > 200)
        {
            return Result.Fail(DemoErrors.TitleMaxLength());
        }

        return Result.Ok();
    }

    public Result ValidateLanguage()
    {
        if (Language == null) return Result.Fail(DemoErrors.LanguageIsRequired());

        return Result.Ok();
    }

    public int BroadcasterId { get; set; }

    public string FileName { get; set; } = null!;

    public int LanguageId { get; set; }

    public Language Language { get; set; } = null!;

    public string Title { get; set; } = null!;

    public virtual Broadcaster Broadcaster { get; set; } = null!;
}

public static class DemoErrors
{
    public class FileKeyIsRequiredError(string msg) : BadRequestError(msg);

    public class DemoNotFoundError(string msg) : NotFoundError(msg);

    public class InvalidFileKeyError(string msg) : BadRequestError(msg);

    public class MaxDemosReachedError(string msg) : BadRequestError(msg);

    public class TitleIsRequiredError(string msg) : BadRequestError(msg);

    public class TitleMinLengthError(string msg) : BadRequestError(msg);

    public class TitleMaxLengthError(string msg) : BadRequestError(msg);

    public class LanguageIsRequiredError(string msg) : BadRequestError(msg);

    public class FileTooLargeError(string msg) : BadRequestError(msg);

    public class FileNotUploadedError(string msg) : BadRequestError(msg);

    public static FileKeyIsRequiredError FileKeyIsRequired() => new("Se debe especificar la clave del archivo de audio.");

    public static DemoNotFoundError DemoNotFound(string fileKey) => new($"No se encontro una demo con la clave {fileKey}.");

    public static InvalidFileKeyError InvalidFileKey() => new("La clave del archivo no corresponde a una subida solicitada por este locutor.");

    public static MaxDemosReachedError MaxDemosReached(int max) => new($"Ya alcanzaste el maximo de {max} demos. Elimina alguna antes de subir una nueva.");

    public static TitleIsRequiredError TitleIsRequired() => new($"Necesita ingresar un titulo para la demo.");

    public static TitleMinLengthError TitleMinLength() => new($"El titulo de la demo necesita por lo menos 5 characteres.");

    public static TitleMaxLengthError TitleMaxLength() => new($"El titulo de la demo puede tener hasta 200 characteres.");

    public static LanguageIsRequiredError LanguageIsRequired() => new($"Necesita seleccionar un lenguaje para la demo.");

    public static FileTooLargeError FileTooLarge(long actualBytes, long maxBytes) => new(
        $"El archivo pesa {actualBytes / 1024.0 / 1024.0:F1}MB, el maximo permitido es {maxBytes / 1024.0 / 1024.0:F0}MB.");

    public static FileNotUploadedError FileNotUploaded(string fileKey) => new(
        $"No se encontro el archivo subido con la clave {fileKey}. Verifica que la subida a S3 haya finalizado antes de confirmar.");
}
