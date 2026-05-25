using QuestPDF.Helpers;

public static class ContractDocumentDataSource
{

    public static ContractPdfModel GetContractDetails()
    {
        var random = new Random();
        return new ContractPdfModel
        {
            Id = Placeholders.Label(),
            ContractDate = DateTime.Now,
            ClientAgencyName = Placeholders.Name(),
            ClientRut = Placeholders.Label(),
            ClientAddress = Placeholders.Label(),
            ClientName = Placeholders.Name(),
            BroadcasterName = Placeholders.Name(),
            BroadcasterAddress = Placeholders.Label(),
            PiecesNames = Enumerable.Range(1, 5).Select(i => Placeholders.Label()).ToArray(),
            Media = Enumerable.Range(1, 5).Select(i => Placeholders.Label()).ToArray(),
            Deadlines = Enumerable.Range(1, 5).Select(i => DateTime.Now.AddDays(random.Next(1, 30)).ToShortDateString()).ToArray(),
            ContractPriceNumber = 10000,
            ContractPriceString = Placeholders.Label(),
            Country = Placeholders.Label(),
            EndOfContract = DateTime.Now.AddMonths(random.Next(1, 12))
        };
    }
}