using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Tests;

public class PdfGeneratorTest
{
    [Fact]
    public void GenerateContractPdf_ShouldCreateValidPdf()
    {
        // Arrange - Crear datos de prueba

        QuestPDF.Settings.License = LicenseType.Community;
        var contract = new ContractPdfModel
        {
            Id = "026",
            ContractDate = DateTime.Now,
            ClientAgencyName = "Uva Creativos Ltda",
            ClientRut = "123456789",
            ClientAddress = "Calle Principal 123",
            ClientName = "Juan Pérez",
            BroadcasterName = "Radio AM 1000",
            BroadcasterAddress = "Avenida Central 456",
            PiecesNames = new[] { "Spot de 30 segundos", "Jingle", "Cuña comercial" },
            Media = new[] { "Radio", "TV", "Digital" },
            Deadlines = new[] { "15/06/2025", "22/06/2025", "30/06/2025" },
            ContractPriceNumber = 5000,
            ContractPriceString = "Cinco mil pesos",
            Country = "Uruguay",
            EndOfContract = DateTime.Now.AddMonths(3)
        };

        // Act - Generar el PDF
        var document = new ContractDocument(contract);
        var filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "Contrato_Prueba.pdf"
        );

        document.GeneratePdf(filePath);

        // Assert
        Assert.True(File.Exists(filePath), $"El archivo PDF no fue creado en: {filePath}");

        var fileInfo = new FileInfo(filePath);
        Assert.True(fileInfo.Length > 0, "El archivo PDF está vacío");
    }
}

public class Contract
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
