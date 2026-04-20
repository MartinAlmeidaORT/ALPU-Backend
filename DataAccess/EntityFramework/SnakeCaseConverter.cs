using CaseConverter;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataAccess.EntityFramework;

public class SnakeCaseConverter<TEnum> : ValueConverter<TEnum, string>
    where TEnum : Enum
{
    public SnakeCaseConverter() : base(
        v => v.ToString().ToSnakeCase(),
        v => (TEnum)Enum.Parse(typeof(TEnum), v, true))
    {

    }
}
