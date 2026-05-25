
public class ContractPdfModel
{
    public string Id { get; set; }
    public DateTime ContractDate { get; set; }
    public string ClientAgencyName { get; set; }
    public string ClientRut { get; set; }

    public string ClientAddress { get; set; }

    public string ClientName { get; set; }

    public string BroadcasterName { get; set; }

    public string BroadcasterAddress { get; set; }

    public string[] PiecesNames { get; set; }

    public string[] Media { get; set; }

    public string[] Deadlines { get; set; }

    public int ContractPriceNumber { get; set; }

    public string ContractPriceString { get; set; }

    public string Country { get; set; }

    public DateTime EndOfContract { get; set; }

}
