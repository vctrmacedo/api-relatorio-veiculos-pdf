using FluentValidation;

public class RelatorioRequestValidator : AbstractValidator<RelatorioRequestDto>
{
    public RelatorioRequestValidator()
    {
        RuleFor(x => x.Veiculo).SetValidator(new VeiculoValidator());
        RuleFor(x => x.Proprietario).SetValidator(new ProprietarioValidator());
        RuleForEach(x => x.HistoricoManutencao).SetValidator(new ManutencaoValidator());
    }
}
