using Domain.Common.Abstracts;
using Domain.Common.Errors;

namespace Domain.Models;

public class Language : Entity
{
    internal Language() { }

    public int LanguageId { get; set; }

    public string Name { get; set; } = null!;

}

public static class LanguageErrors
{
    public class LanguageNotFoundError(string msg) : NotFoundError(msg);

    public static LanguageNotFoundError LanguageNotFound(int languageId) => new($"Language with id {languageId} was not found.");
}
