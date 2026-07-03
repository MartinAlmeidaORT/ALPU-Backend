namespace Domain.Common;

public static class ContractSerialGenerator
{
    public static string Generate(int broadcasterId, string broadcasterFirstName, string broadcasterLastName, int contractId)
    {
        string initials = $"{broadcasterFirstName[0]}{broadcasterLastName[0]}".ToUpperInvariant();
        return $"{broadcasterId}{initials}{contractId}";
    }
}
