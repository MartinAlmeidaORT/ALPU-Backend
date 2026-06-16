using System.Linq;
using Domain.Models;
using Domain.Models.Campaign;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Humanizer;
using System.Globalization;

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
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                page.Content().Column(column =>
                {
                    // Encabezado
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text(t =>
                        {
                            t.Span("CTO N°.").Bold();
                            t.Span(Model.ContractId.ToString() ?? "______");
                        });
                        row.RelativeItem().Text(t =>
                        {
                            t.Span("OC.N° ").Bold();
                            t.Span("_____________");
                        });
                        row.RelativeItem().AlignRight().Text(t =>
                        {
                            t.Span("Fecha ").Bold();
                            t.Span($"{Model.Date:dd/MM/yy}");
                        });
                    });

                    column.Item().PaddingVertical(10).LineHorizontal(1);

                    // Comparecencia (Partes)
                    column.Item().PaddingBottom(10).Text(text =>
                    {
                        text.Span("POR UNA PARTE: La firma (Agencia de Publicidad/Cliente): ").Bold();
                        text.Span($"{Model.Client?.Agency?.Name ?? "____________________"} ");
                        text.Span("RUT ").Bold();
                        text.Span($"{Model.Client?.RUT ?? "____________________"} ");
                        text.Span("con domicilio en ").Bold();
                        text.Span($"{Model.Client?.Address?.ToString() ?? "____________________"} ");
                        text.Span("representada por: ").Bold();
                        text.Span($"{Model.Client?.FullName ?? "____________________"} ");
                        text.Span("en adelante llamado el ").Bold();
                        text.Span("Contratante ");
                        text.Span("y POR OTRA PARTE: ").Bold();
                        text.Span($"{Model.Broadcaster?.FullName ?? "____________________"} ");
                        text.Span("con domicilio en ").Bold();
                        text.Span($"{Model.Broadcaster?.Address?.ToString() ?? "____________________"}, ");
                        text.Span("en adelante el ").Bold();
                        text.Span("Contratado, han acordado lo siguiente:").Bold();
                    });

                    // Extraer piezas y medios de forma segura
                    var piezas = Model.Campaigns?
                        .SelectMany(c => c.Services ?? Enumerable.Empty<BaseCampaignService>())
                        .SelectMany(s => s.Pieces ?? Enumerable.Empty<Piece>())
                        .Select(p => p.Name)
                        .ToList();

                    var medios = Model.Campaigns?
                        .SelectMany(c => c.Services ?? Enumerable.Empty<BaseCampaignService>())
                        .Select(s => s.Service?.Name)
                        .ToList();

                    string piezasStr = piezas?.Any() == true ? string.Join(", ", piezas) : "____________________";
                    string mediosStr = medios?.Any() == true ? string.Join(", ", medios) : "____________________";

                    // Cláusulas 1 a 4
                    column.Item().PaddingBottom(5).Text(text =>
                    {
                        text.Span("1. El Contratante arrenda los servicios profesionales del Contratado en la(s) piezas (nombre de la/s pieza/s): ").Bold();
                        text.Span($"{piezasStr} ");
                        text.Span("para su utilización de la siguiente forma (medios): ").Bold();
                        text.Span($"{mediosStr} .- ");
                        text.Span("Plazo: ").Bold();
                        text.Span($"{Model.DueDate:dd/MM/yy}");
                    });

                    column.Item().PaddingBottom(5).Text(text =>
                    {
                        text.Span("2. El precio acordado es de $U/U$S ").Bold();
                        text.Span($"{Model.TotalPrice}, {((int)Model.TotalPrice).ToWords(new CultureInfo("es")).ToUpper()} ");
                        text.Span("(Pesos Uruguayos/ Dólares) más IVA, debiendo ser abonado en el plazo de 30 (treinta) días como máximo.");
                    });

                    column.Item().PaddingBottom(5).Text(text =>
                    {
                        text.Span("3. El producto del servicio realizado solo podrá ser utilizado en: ").Bold();
                        text.Span(Model.Country?.Name ?? "____________________");
                    });

                    column.Item().PaddingBottom(10).Text(text =>
                    {
                        text.Span("4. El plazo del presente es a partir de la firma del mismo hasta el día ").Bold();
                        text.Span($"{Model.DueDate:dd/MM/yy} ");
                        text.Span("inclusive.").Bold();
                    });

                    // Textos legales fijos (Cláusulas 5 a 14)
                    string[] clausulasLegales = new[]
                    {
                        "5. En caso que el Contratante decida utilizar el producto en otro territorio que no esté incluido en la cláusula tercera de este acuerdo, deberá abonar el 50% (cincuenta por ciento) sobre el precio acordado en la cláusula segunda, por cada país en que se emitan la/s pieza/s por el término del presente.",
                        "6. El Contratante deberá cumplir estrictamente con lo estipulado en la cláusula primera y por el período establecido en la cláusula cuarta. Constatada de hecho la utilización del producto mencionado, una vez vencido el plazo señalado en la cláusula cuarta, el presente contrato se entenderá renovado automáticamente de pleno derecho, sin necesidad de gestión de tipo alguno, tomándose siempre como fecha de comienzo del nuevo período del día siguiente de la finalización del presente, por igual plazo y de acuerdo a los precios vigentes de ALPU al momento de la renovación.",
                        "7. Sin perjuicio de lo establecido en la cláusula sexta, cuando el contratante desee utilizar la/s pieza/s por tercera vez se deberá recabar por escrito el consentimiento del Contratado. En caso de incumplimiento se deberá abonar al Contratado una multa equivalente a tres veces el precio vigente del ALPU, sin perjuicio de lo que se deba por la reutilización según la cláusula sexta.",
                        "8. El Contratante, en caso de prorrogar el plazo estipulado y/o utilizar la/s pieza/s en otro medio y/o territorio que no esté incluido en este acuerdo, deberá comunicarlo fehacientemente al Contratado en un plazo de 10 (diez) días a partir de la prórroga y/o emisión en otro medio y/o territorio. Abonará entonces la suma correspondiente, de acuerdo a los precios vigentes de ALPU, en el plazo de 30 (treinta) días a partir de la comunicación.",
                        "9. La contravención a lo estipulado en la cláusula anterior habilitará al Contratado a reclamar al Contratante los daños y perjuicios que puedan corresponder, sin perjuicio de lo estipulado en la cláusula décimo segunda.",
                        "10. Si la Agencia y/o Cliente, por cualquier razón, deciden no emitir el trabajo realizado por el Contratado, deberán comunicárselo en el plazo de 15 (quince) días a partir de la fecha del presente. En ese caso el trabajo efectuado se abonará como Prueba de acuerdo a los precios vigentes de ALPU. Si en el contrato figura más de una pieza, se abonará como prueba cada una de ellas. Vencido ese plazo se deberá abonar el 50% (cincuenta por ciento) del precio que se estipuló en la cláusula segunda, sin que ello otorgue derecho de utilización alguno sobre las piezas afectadas.",
                        "11. Los servicios profesionales establecidos en la cláusula primera son prioridad exclusiva del Contratado a perpetuidad. El pago efectuado sólo otorga derecho a su utilización por el período y en los términos que surgen del mismo.",
                        "12. El pago se efectuará en las Oficinas de ALPU o al cobrador autorizado por ésta. No siendo pagado el precio acordado dentro del plazo correspondiente, el Contratante caerá en mora de pleno derecho, sin necesidad de interpelación judicial ni gestión extrajudicial alguna y regirá el interés según la tasa máxima legal mensual, que se consolidará automáticamente con la deuda. En caso de que sea necesaria la acción judicial para obtener el cobro, se deberá notificar la deuda al Contratante, encontrándose el presente contrato comprendido dentro de los títulos que habilitan el proceso ejecutivo, en los términos del articulo 353.3 del Código General del Proceso, considerándose la suma adeudada como cantidad líquida y exigible.",
                        "13. El Contratado autoriza a ALPU a todos los efectos legales, la que podrá realizar a su nombre y representación todas las gestiones privadas, judiciales y administrativas necesarias para el cumplimiento del contrato y para el cobro del precio acordado.",
                        "14. Para todos los efectos legales a que diere lugar este Contrato, las partes fijan como domicilios especiales los denunciados en la comparecencia"
                    };

                    foreach (var clausula in clausulasLegales)
                    {
                        column.Item().PaddingBottom(5).Text(clausula).FontSize(9);
                    }

                    column.Item().PaddingVertical(30);

                    // Firmas
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().LineHorizontal(1);
                            col.Item().PaddingTop(5).Text("Firma digital CLIENTE / AGENCIA / PROD.").FontSize(9).AlignCenter();
                        });

                        row.ConstantItem(60); // Espacio entre firmas

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().LineHorizontal(1);
                            col.Item().PaddingTop(5).Text("Firma digital LOCUTOR/A.").FontSize(9).AlignCenter();
                        });
                    });
                });
            });
    }
}