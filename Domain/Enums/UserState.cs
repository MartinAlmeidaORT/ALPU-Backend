using NpgsqlTypes;

namespace Domain.Enums;

public enum UserState
{
    [PgName("enabled")]
    Enabled,
    [PgName("pending")]
    Pending,
    [PgName("penalized")]
    Penalized
}
