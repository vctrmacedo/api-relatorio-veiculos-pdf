using Microsoft.AspNetCore.Mvc;
using FluentValidation;

[ApiController]
[Route("api/[controller]")] // Fica api/Relatorio 
public class RelatorioController : ControllerBase
{
    private readonly RelatorioService _relatorioService;
    private readonly IValidator<RelatorioRequestDto> _validator;
    private readonly ILogger<RelatorioController> _logger;

    public RelatorioController(
        RelatorioService relatorioService,
        IValidator<RelatorioRequestDto> validator,
        ILogger<RelatorioController> logger)
    {
        _relatorioService = relatorioService;
        _validator = validator;
        _logger = logger;
    }

    [HttpPost("gerar-relatorio")]
    public IActionResult GerarRelatorio([FromBody] RelatorioRequestDto request)
    {
        try
        {
            _logger.LogInformation("Recebida requisição para gerar relatório");

            // Validação de request nulo
            if (request == null)
            {
                _logger.LogWarning("Requisição recebida com payload nulo");
                return BadRequest(new { Error = "Payload da requisição não pode ser nulo" });
            }

            // Validação com FluentValidation - valida os dados do request. Se houver erros, retornamos um erro 400 com os detalhes.
            var validation = _validator.Validate(request);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Falha de validação: {Erros}", validation.Errors);
                return BadRequest(new 
                { 
                    Error = "Dados inválidos", 
                    Details = validation.Errors.Select(e => new 
                    { 
                        Field = e.PropertyName, 
                        Message = e.ErrorMessage 
                    }) 
                });
            }

            // Validações adicionais de segurança
            if (request.HistoricoManutencao == null || !request.HistoricoManutencao.Any())
            {
                _logger.LogWarning("Tentativa de gerar relatório sem histórico de manutenção para placa {Placa}", request.Veiculo?.Placa);
                return BadRequest(new { Error = "Histórico de manutenção é obrigatório e deve conter pelo menos um registro" });
            }

            var pdfBytes = _relatorioService.GerarRelatorio(request);
            
            // Gerar nome do arquivo com placa e data
            var placaLimpa = request.Veiculo.Placa.Replace("-", "_").Replace(" ", "_");
            var nomeArquivo = $"relatorio_{placaLimpa}_{DateTime.Now:dd_MM_yyyy}.pdf";
            
            _logger.LogInformation("Relatório gerado com sucesso para placa {Placa}", request.Veiculo.Placa);
            return File(pdfBytes, "application/pdf", nomeArquivo);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Erro de argumento ao gerar relatório");
            return BadRequest(new { Error = "Dados fornecidos são inválidos", Details = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Erro de operação inválida ao gerar relatório");
            return BadRequest(new { Error = "Operação inválida", Details = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno do servidor ao gerar relatório");
            return StatusCode(500, new { Error = "Erro interno do servidor", Message = "Ocorreu um erro inesperado ao gerar o relatório" });
        }
    }
}