using Application.QuestPDF;
using Domain.Models;
using Domain.Models.Campaign;
using Domain.Models.Campaign.Period;
using Domain.Models.Services;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Tests;

public class PdfGeneratorTest
{
    [Fact]
    public void GenerateContractPdf_ShouldCreateValidPdf()
    {
        // Arrange - Crear datos de prueba
        QuestPDF.Settings.License = LicenseType.Community;
        DateOnly date = new();
        Country country = new()
        {
            CountryCode = "UY",
            Name = "Uruguay"
        };
        var contract = new Contract
        {
            ContractId = 26,
            ContractSerial = "JP04-26",
            Date = date,
            DueDate = date.AddMonths(3),
            TotalPrice = 5000,
            Country = country,
            Client = new()
            {
                FirstName = "Juan",
                LastName = "Pérez",
                RUT = "123456789",
                Address = new()
                {
                    Country = country,
                    Department = new()
                    {
                        Name = "Treinta y Tres"
                    },
                    City = "Treinta y Tres",
                    Street = "Calle Principal 123"
                },
                Agency = new()
                {
                    Name = "Uva Creativos Ltda"
                }
            },
            Broadcaster = new()
            {
                FirstName = "Pedro",
                LastName = "Lamont",
                Address = new()
                {
                    Country = country,
                    Department = new()
                    {
                        Name = "Treinta y Tres"
                    },
                    City = "Treinta y Tres",
                    Street = "Avenida Central 456"
                },
            },
            Campaigns = [
                new Campaign() {
                    Services = [
                        new RadioCampaignService() {
                            Service = new PeriodService() {
                                Name = "Radio"
                            },
                            Pieces = [
                                new() {
                                    Name = "Spot de 30 segundos"
                                }
                            ]
                        },
                        new TvCampaignService() {
                            Service = new PeriodService() {
                                Name = "Television"
                            },
                            Pieces = [
                                new() {
                                    Name = "Jingle"
                                },
                                new () {
                                    Name = "Cuña comercial"
                                }
                            ]
                        }
                    ]
                }
            ]
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
