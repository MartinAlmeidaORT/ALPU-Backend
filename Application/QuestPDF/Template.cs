
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class ContractDocument : IDocument
{
    public ContractPdfModel Model { get; }

    public ContractDocument(ContractPdfModel model)
    {
        Model = model;
    }

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
                        row.RelativeColumn().AlignLeft().Text("CTO N°").FontSize(10).Bold();
                        row.RelativeColumn().Text(Model.Id ?? "______").FontSize(10);
                        row.RelativeColumn().AlignCenter().Text("OC N°").FontSize(10).Bold();
                        row.RelativeColumn().Text("_____________").FontSize(10);
                        row.RelativeColumn().AlignRight().Text($"Fecha {Model.ContractDate:dd/MM/yyyy}").FontSize(10);
                    });

                    column.Item().PaddingVertical(15).LineHorizontal(1);

                    // Section 1: POR UNA PARTE
                    column.Item().Row(row =>
                    {
                        row.RelativeColumn(3).Text("POR UNA PARTE: La firma (Agencia de Publicidad/Cliente)").FontSize(10).Bold();
                        row.RelativeColumn(2).AlignRight().Text(Model.ClientAgencyName ?? "____________________").FontSize(10);
                    });

                    column.Item().PaddingTop(5).Row(row =>
                    {
                        row.RelativeColumn().Text($"con domicilio en {Model.ClientAddress ?? "___________________________"}, representada por {Model.ClientName ?? "___________________________"} en adelante llamado el \"Contratante\"; y");
                    });

                    // Section 2: CONTRATANTE
                    column.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeColumn(3).Text("POR OTRA PARTE: Siendo el").FontSize(10).Bold();
                        row.RelativeColumn(2).AlignRight().Text("_____________").FontSize(10);
                    });

                    column.Item().PaddingTop(5).Row(row =>
                    {
                        row.RelativeColumn().Text($"con domicilio en {Model.BroadcasterAddress ?? "___________________________"}, debidamente autorizado, en adelante llamado \"Contratista\"; han acordado lo siguiente:");
                    });

                    column.Item().PaddingVertical(15).LineHorizontal(1);

                    // Contract terms
                    column.Item().PaddingTop(10).Text("1. El Contratante autoriza los servicios profesionales del Contratado en la(s) pieza(s):").FontSize(10);

                    column.Item().PaddingLeft(20).Column(innerColumn =>
                    {
                        if (Model.PiecesNames != null)
                        {
                            foreach (var piece in Model.PiecesNames)
                            {
                                innerColumn.Item().Text($"• {piece}").FontSize(9);
                            }
                        }
                    });

                    column.Item().PaddingTop(10).Text("2. Medios a utilizar por el cliente frente al/los vigentes locales/regionales/nacionales:").FontSize(10);

                    column.Item().PaddingLeft(20).Column(innerColumn =>
                    {
                        if (Model.Media != null)
                        {
                            foreach (var medium in Model.Media)
                            {
                                innerColumn.Item().Text($"• {medium}").FontSize(9);
                            }
                        }
                    });

                    column.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeColumn().Text("3. El producto del servicio realizado solo podrá ser utilizado en:").FontSize(10);
                        row.RelativeColumn().AlignRight().Text(Model.Country ?? "_____________").FontSize(10);
                    });

                    column.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeColumn().Text("4. El plazo del presente contrato a partir de la firma del mismo hasta el:").FontSize(10);
                        row.RelativeColumn().AlignRight().Text(Model.EndOfContract.ToShortDateString()).FontSize(10);
                    });

                    column.Item().PaddingTop(15).Text("5. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.").FontSize(9);

                    column.Item().PaddingTop(15).Row(row =>
                    {
                        row.RelativeColumn().Text("Precio:").FontSize(10).Bold();
                        row.RelativeColumn().AlignRight().Text($"${Model.ContractPriceNumber} ({Model.ContractPriceString ?? "_______________"})").FontSize(10);
                    });

                    column.Item().PaddingVertical(30);

                    // Signatures
                    column.Item().Row(row =>
                    {
                        row.RelativeColumn().Column(col =>
                        {
                            col.Item().Height(40).Border(1).BorderColor(Colors.Grey.Medium);
                            col.Item().Text("Firma Contratante").FontSize(9).AlignCenter();
                        });

                        row.RelativeColumn().Column(col =>
                        {
                            col.Item().Height(40).Border(1).BorderColor(Colors.Grey.Medium);
                            col.Item().Text("Firma Contratista").FontSize(9).AlignCenter();
                        });
                    });
                });
            });
    }
}