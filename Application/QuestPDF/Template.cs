using Domain.Models;
using Domain.Models.Campaign;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Application.QuestPDF;

public class ContractDocument(Contract model) : IDocument
{
    public Contract Model { get; init; } = model;

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Margin(40);

                page.Content().Column(column =>
                {
                    // Header with contract details
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().AlignLeft().Text("CTO N°").FontSize(10).Bold();
                        row.RelativeItem().Text(Model.ContractId.ToString() ?? "______").FontSize(10);
                        row.RelativeItem().AlignCenter().Text("OC N°").FontSize(10).Bold();
                        row.RelativeItem().Text("_____________").FontSize(10);
                        row.RelativeItem().AlignRight().Text($"Fecha {Model.Date:dd/MM/yyyy}").FontSize(10);
                    });

                    column.Item().PaddingVertical(15).LineHorizontal(1);

                    // Section 1: POR UNA PARTE
                    column.Item().Row(row =>
                    {
                        row.RelativeItem(3).Text("POR UNA PARTE: La firma (Agencia de Publicidad/Cliente)").FontSize(10).Bold();
                        row.RelativeItem(2).AlignRight().Text(Model.Client.Agency.Name ?? "____________________").FontSize(10);
                    });

                    column.Item().PaddingTop(5).Row(row =>
                    {
                        row.RelativeItem().Text($"con domicilio en {Model.Client.Address.ToString() ?? "___________________________"}, representada por {Model.Client.FullName ?? "___________________________"} en adelante llamado el \"Contratante\"; y");
                    });

                    // Section 2: CONTRATANTE
                    column.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem(3).Text($"POR OTRA PARTE: Siendo el contratado {Model.Broadcaster.FullName}").FontSize(10).Bold();
                        row.RelativeItem(2).AlignRight().Text("_____________").FontSize(10);
                    });

                    column.Item().PaddingTop(5).Row(row =>
                    {
                        row.RelativeItem().Text($"con domicilio en {Model.Broadcaster.Address.ToString() ?? "___________________________"}, debidamente autorizado, en adelante llamado \"Contratista\"; han acordado lo siguiente:");
                    });

                    column.Item().PaddingVertical(15).LineHorizontal(1);

                    // Contract terms
                    column.Item().PaddingTop(10).Text("1. El Contratante autoriza los servicios profesionales del Contratado en la(s) pieza(s):").FontSize(10);

                    column.Item().PaddingLeft(20).Column(innerColumn =>
                    {
                        if (Model.Campaigns.Any(c => c.Services.Any(s => s.Pieces != null)))
                        {
                            foreach (Piece piece in Model.Campaigns.SelectMany(c => c.Services.SelectMany(s => s.Pieces)))
                            {
                                innerColumn.Item().Text($"• {piece.Name}").FontSize(9);
                            }
                        }
                    });

                    column.Item().PaddingTop(10).Text("2. Medios a utilizar por el cliente frente al/los vigentes locales/regionales/nacionales:").FontSize(10);

                    column.Item().PaddingLeft(20).Column(innerColumn =>
                    {
                        if (Model.Campaigns.Any(c => c.Services != null))
                        {
                            foreach (BaseCampaignService service in Model.Campaigns.SelectMany(c => c.Services))
                            {
                                innerColumn.Item().Text($"• {service.Service.Name}").FontSize(9);
                            }
                        }
                    });

                    column.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem().Text("3. El producto del servicio realizado solo podrá ser utilizado en:").FontSize(10);
                        row.RelativeItem().AlignRight().Text(Model.Country.Name ?? "_____________").FontSize(10);
                    });

                    column.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem().Text("4. El plazo del presente contrato a partir de la firma del mismo hasta el:").FontSize(10);
                        row.RelativeItem().AlignRight().Text(Model.DueDate.ToShortDateString()).FontSize(10);
                    });

                    column.Item().PaddingTop(15).Text("5. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.").FontSize(9);

                    column.Item().PaddingTop(15).Row(row =>
                    {
                        row.RelativeItem().Text("Precio:").FontSize(10).Bold();
                        row.RelativeItem().AlignRight().Text($"${Model.TotalPrice}").FontSize(10);
                    });

                    column.Item().PaddingVertical(30);

                    // Signatures
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Height(40).Border(1).BorderColor(Colors.Grey.Medium);
                            col.Item().Text("Firma Contratante").FontSize(9).AlignCenter();
                        });

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Height(40).Border(1).BorderColor(Colors.Grey.Medium);
                            col.Item().Text("Firma Contratista").FontSize(9).AlignCenter();
                        });
                    });
                });
            });
    }
}
