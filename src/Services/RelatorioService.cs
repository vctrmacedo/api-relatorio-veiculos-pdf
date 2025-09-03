using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;

public class RelatorioService
{
    private readonly ILogger<RelatorioService> _logger;

    public RelatorioService(ILogger<RelatorioService> logger)
    {
        _logger = logger;
        
        // Configurar licença Community do QuestPDF
        QuestPDF.Settings.License = LicenseType.Community;
        
        // Habilitar debugging para identificar problemas de layout
        QuestPDF.Settings.EnableDebugging = true;
    }

    public byte[] GerarRelatorio(RelatorioRequestDto request)
    {
        try
        {
            // Validações de entrada
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request não pode ser nulo");

            if (request.Veiculo == null)
                throw new ArgumentException("Dados do veículo são obrigatórios", nameof(request.Veiculo));

            if (request.Proprietario == null)
                throw new ArgumentException("Dados do proprietário são obrigatórios", nameof(request.Proprietario));

            if (request.HistoricoManutencao == null || !request.HistoricoManutencao.Any())
                throw new ArgumentException("Histórico de manutenção é obrigatório", nameof(request.HistoricoManutencao));

            _logger.LogInformation("Iniciando geração do relatório para placa {Placa}", request.Veiculo.Placa);

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    
                    page.Header().Column(col =>
                    {
                        col.Item().Text("RELATÓRIO DE VEÍCULO").FontSize(20).Bold().AlignCenter();
                        col.Item().Text($"Placa: {request.Veiculo.Placa ?? "N/A"}").FontSize(14).Bold().AlignCenter();
                        col.Item().PaddingTop(5).Text($"Proprietário: {request.Proprietario.Nome ?? "N/A"}").FontSize(12).AlignCenter();
                        col.Item().PaddingTop(10).LineHorizontal(1);
                    });

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        // Dados do Veículo
                        col.Item().Text("DADOS DO VEÍCULO").FontSize(14).Bold();
                        col.Item().PaddingLeft(10).Column(vehicleCol =>
                        {
                            vehicleCol.Item().Text($"Placa: {request.Veiculo.Placa ?? "N/A"}");
                            vehicleCol.Item().Text($"Marca: {request.Veiculo.Marca ?? "N/A"}");
                            vehicleCol.Item().Text($"Modelo: {request.Veiculo.Modelo ?? "N/A"}");
                            vehicleCol.Item().Text($"Ano: {request.Veiculo.Ano}");
                        });
                        
                        col.Item().PaddingTop(15).Text("DADOS DO PROPRIETÁRIO").FontSize(14).Bold();
                        col.Item().PaddingLeft(10).Column(ownerCol =>
                        {
                            ownerCol.Item().Text($"Nome: {request.Proprietario.Nome ?? "N/A"}");
                            ownerCol.Item().Text($"Documento: {request.Proprietario.Documento ?? "N/A"}");
                            ownerCol.Item().Text($"Endereço: {request.Proprietario.Endereco ?? "N/A"}");
                        });

                        col.Item().PaddingTop(15).Text("HISTÓRICO DE MANUTENÇÃO").FontSize(14).Bold();
                        
                        col.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(80);
                                cols.RelativeColumn();
                                cols.ConstantColumn(80);
                                cols.ConstantColumn(80);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Data").Bold();
                                header.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Serviço").Bold();
                                header.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("KM").Bold();
                                header.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(5).Text("Custo").Bold();
                            });

                            foreach (var manutencao in request.HistoricoManutencao)
                            {
                                table.Cell().Border(1).Padding(5).Text(manutencao.Data.ToString("dd/MM/yyyy"));
                                table.Cell().Border(1).Padding(5).Text(manutencao.Servico ?? "N/A");
                                table.Cell().Border(1).Padding(5).Text(manutencao.Quilometragem.ToString("N0"));
                                table.Cell().Border(1).Padding(5).Text($"R$ {manutencao.Custo:N2}");
                            }

                            // Total
                            var custoTotal = request.HistoricoManutencao.Sum(m => m.Custo);
                            table.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("");
                            table.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("");
                            table.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("TOTAL:").Bold();
                            table.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text($"R$ {custoTotal:N2}").Bold();
                        });
                    });

                    page.Footer().AlignCenter().Text($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10);
                });
            });

            _logger.LogInformation("Relatório gerado com sucesso para {Placa}", request.Veiculo.Placa);

            var pdfBytes = pdf.GeneratePdf();
            
            if (pdfBytes == null || pdfBytes.Length == 0)
                throw new InvalidOperationException("Falha na geração do PDF - arquivo vazio");

            return pdfBytes;
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Argumento nulo fornecido para geração de relatório");
            throw;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Argumento inválido para geração de relatório");
            throw;
        }
        catch (QuestPDF.Drawing.Exceptions.DocumentLayoutException ex)
        {
            _logger.LogError(ex, "Erro de layout do documento PDF");
            throw new InvalidOperationException("Erro na formatação do documento PDF", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao gerar relatório");
            throw new InvalidOperationException("Erro interno na geração do relatório", ex);
        }
    }
}
