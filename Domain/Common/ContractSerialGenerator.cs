namespace Domain.Common;

public static class ContractSerialGenerator
{
    public static string Generate(string broadcasterFirstName, string broadcasterLastName, int broadcasterId, int rootContractId, int replacementCount)
    {
        if (replacementCount < 0)
            throw new ArgumentOutOfRangeException(nameof(replacementCount), "El contador de reemplazos no puede ser negativo.");

        if (replacementCount > 25)
            throw new InvalidOperationException("Se alcanzo el limite de reemplazos (A-Z) para este contrato.");

        string initials = GetInitials(broadcasterFirstName, broadcasterLastName);
        char letter = (char)('A' + replacementCount);

        return $"{initials}{broadcasterId}-{rootContractId}{letter}";
    }

    private static string GetInitials(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName)) return "";
        return $"{firstName[0]}{lastName[0]}".ToUpperInvariant();
    }
}
