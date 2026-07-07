namespace Domain.Common;

public static class ContractSerialGenerator
{
    public static string Generate(int broadcasterId, string broadcasterFirstName, string broadcasterLastName, int contractId, string? contractSerial = null)
    {
    string initials = "";
    if (!string.IsNullOrWhiteSpace(broadcasterFirstName) && !string.IsNullOrWhiteSpace(broadcasterLastName))
    {
        initials = $"{broadcasterFirstName[0]}{broadcasterLastName[0]}".ToUpperInvariant();
    }

    if (string.IsNullOrWhiteSpace(contractSerial))
    {
        return $"{broadcasterId}{initials}{contractId}A";
    }

    char lastLetter = contractSerial.Last();
    lastLetter = (char)(lastLetter + 1);

    return $"{broadcasterId}{initials}{contractId}{lastLetter}";
    }
}
