public record ManutencaoDto(
    DateTime Data,
    string Servico,
    int Quilometragem,
    decimal Custo
);