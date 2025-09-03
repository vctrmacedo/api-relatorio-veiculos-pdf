public record RelatorioRequestDto(
    VeiculoDto Veiculo,
    ProprietarioDto Proprietario,
    List<ManutencaoDto> HistoricoManutencao
);