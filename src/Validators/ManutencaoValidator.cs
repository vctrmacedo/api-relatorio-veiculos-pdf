using FluentValidation;

public class ManutencaoValidator : AbstractValidator<ManutencaoDto>
{
    public ManutencaoValidator()
    {
        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("Data é obrigatória.")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Data não pode ser futura.");

        RuleFor(x => x.Servico)
            .NotEmpty().WithMessage("Serviço é obrigatório.")
            .MaximumLength(100);

        RuleFor(x => x.Quilometragem)
            .GreaterThan(0);

        RuleFor(x => x.Custo)
            .GreaterThan(0).WithMessage("Custo deve ser maior que zero.");
    }
}