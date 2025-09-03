using FluentValidation;
using System.Text.RegularExpressions;

public class VeiculoValidator : AbstractValidator<VeiculoDto>
{
    public VeiculoValidator()
    {
        RuleFor(x => x.Placa)
            .NotEmpty().WithMessage("Placa é obrigatória.")
            .Must(BeValidPlaca).WithMessage("Placa deve seguir o formato brasileiro (AAA-9999) ou Mercosul (AAA9A99).");

        RuleFor(x => x.Marca)
            .NotEmpty().WithMessage("Marca é obrigatória.")
            .MaximumLength(50);

        RuleFor(x => x.Modelo)
            .NotEmpty().WithMessage("Modelo é obrigatório.")
            .MaximumLength(50);

        RuleFor(x => x.Ano)
            .GreaterThan(1900).WithMessage("Ano inválido.")
            .LessThanOrEqualTo(DateTime.Now.Year);
    }

    private bool BeValidPlaca(string placa)
    {
        if (string.IsNullOrEmpty(placa))
            return false;

        // Remove espaços e hífens para normalizar
        var normalizedPlaca = placa.Replace(" ", "").Replace("-", "").ToUpper();

        // Formato antigo brasileiro: AAA9999 (3 letras + 4 números)
        var formatoAntigo = Regex.IsMatch(normalizedPlaca, @"^[A-Z]{3}[0-9]{4}$");
        
        // Formato Mercosul: AAA9A99 (3 letras + 1 número + 1 letra + 2 números)
        var formatoMercosul = Regex.IsMatch(normalizedPlaca, @"^[A-Z]{3}[0-9][A-Z][0-9]{2}$");

        return formatoAntigo || formatoMercosul;
    }
}