# Relatório API - Gerador de PDF de Veículos

Este projeto é uma API REST desenvolvida em C# (.NET 9) que gera relatórios em PDF a partir de dados de veículos fornecidos em formato JSON.

## 🚀 Tecnologias Utilizadas

- **.NET 9** - Framework principal
- **ASP.NET Core** - API REST
- **QuestPDF** - Geração de PDFs
- **FluentValidation** - Validação de dados
- **Swagger** - Documentação da API

## 📋 Funcionalidades

- Recebe dados de veículo, proprietário e histórico de manutenção via JSON
- Valida os dados de entrada com regras robustas
- Gera PDF profissional com:
  - Cabeçalho com título, placa e proprietário
  - Seção de dados do proprietário e veículo
  - Tabela de histórico de manutenção
- Tratamento de erros e logging estruturado

## 🔧 Pré-requisitos

- .NET 9 SDK instalado
- IDE/Editor (Visual Studio, VS Code, etc.)

## 📦 Dependências

As seguintes bibliotecas são instaladas automaticamente via NuGet:

- `QuestPDF` - Biblioteca escolhida para geração de PDFs devido à sua:
  - API fluente e intuitiva
  - Excelente performance
  - Suporte completo a layouts responsivos
  - Gratuita para uso comunitário
  - Documentação abrangente
- `FluentValidation` - Validação declarativa e robusta
- `FluentValidation.AspNetCore` - Integração com ASP.NET Core
- `Swashbuckle.AspNetCore` - Geração de documentação Swagger

## 🚀 Como Executar

1. **Clone ou baixe o projeto**

2. **Navegue até o diretório do projeto**
   ```bash
   cd RelatorioApi
   ```

3. **Restaure as dependências**
   ```bash
   dotnet restore
   ```

4. **Execute a aplicação**
   ```bash
   dotnet run
   ```

5. **Acesse a aplicação**
   - API: `http://localhost:5164`
   - Swagger UI: `http://localhost:5164/swagger`

## 📝 Como Usar

### Endpoint Principal
```
POST /api/Relatorio/gerar-relatorio
Content-Type: application/json
```

### Exemplo de JSON de Entrada
```json
{
  "veiculo": {
    "placa": "ABC1D23",
    "marca": "Toyota", 
    "modelo": "Corolla",
    "ano": 2024
  },
  "proprietario": {
    "nome": "Victor Macedo",
    "documento": "123.258.917-07",
    "endereco": "Rua do dev, 30"
  },
  "historicoManutencao": [
    {
      "data": "2023-05-10",
      "servico": "Troca de óleo e filtro",
      "quilometragem": 20500,
      "custo": 350.50
    },
    {
      "data": "2024-01-15",
      "servico": "Alinhamento e balanceamento",
      "quilometragem": 35100,
      "custo": 220.00
    }
  ]
}
```

### Exemplo com cURL
```bash
curl -X 'POST' \
  'http://localhost:5164/api/Relatorio/gerar-relatorio' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "veiculo": {
    "placa": "ABC1D23",
    "marca": "Toyota",
    "modelo": "Corolla",
    "ano": 2024
  },
  "proprietario": {
    "nome": "Victor Macedo",
    "documento": "123.258.917-07",
    "endereco": "Rua do dev, 30"
  },
  "historicoManutencao": [
    {
      "data": "2023-05-10",
      "servico": "Troca de óleo e filtro",
      "quilometragem": 20500,
      "custo": 350.50
    },
    {
      "data": "2024-01-15",
      "servico": "Alinhamento e balanceamento",
      "quilometragem": 35100,
      "custo": 220.00
    }
  ]
}'
```

## ✅ Validações Implementadas

### Veículo
- Placa: obrigatória, **suporta formato brasileiro (ABC-1234) e Mercosul (ABC1D23)**
- Marca: obrigatória, máximo 50 caracteres
- Modelo: obrigatório, máximo 50 caracteres
- Ano: obrigatório, entre 1900 e ano atual

### Proprietário
- Nome: obrigatório, máximo 100 caracteres
- Documento: obrigatório, formato CPF (000.000.000-00)
- Endereço: obrigatório, máximo 200 caracteres

### Histórico de Manutenção
- Data: obrigatória, não pode ser futura, **formato: "YYYY-MM-DD"** (ex: "2023-05-10")
- Serviço: obrigatório, máximo 100 caracteres
- Quilometragem: obrigatória, valor positivo
- Custo: obrigatório, valor positivo

## 🏗️ Estrutura do Projeto

```
src/
├── Controllers/
│   └── RelatorioController.cs      # Endpoint da API
├── Models/
│   ├── VeiculoDto.cs              # DTO do veículo
│   ├── ProprietarioDto.cs         # DTO do proprietário
│   ├── ManutencaoDto.cs           # DTO da manutenção
│   └── RelatorioRequestDto.cs     # DTO da requisição
├── Validators/
│   ├── VeiculoValidator.cs        # Validações do veículo
│   ├── ProprietarioValidator.cs   # Validações do proprietário
│   ├── ManutencaoValidator.cs     # Validações da manutenção
│   └── RelatorioRequestValidator.cs # Validações gerais
├── Services/
│   └── RelatorioService.cs        # Lógica de geração do PDF
└── Program.cs                     # Configuração da aplicação
```

## 🎯 Justificativa da Biblioteca QuestPDF

**QuestPDF** foi escolhida pelos seguintes motivos:

1. **API Fluente**: Sintaxe clara e intuitiva para criar layouts
2. **Performance**: Excelente velocidade de geração de PDFs
3. **Flexibilidade**: Suporte a layouts complexos e responsivos
4. **Gratuita**: Licença Community para projetos não comerciais
5. **Documentação**: Extensa documentação e exemplos
6. **Modernidade**: Biblioteca ativa e bem mantida
7. **Facilidade de Uso**: Menos código boilerplate comparado a outras bibliotecas

## 📄 Estrutura do PDF Gerado

O PDF contém:
- **Cabeçalho**: Título "Relatório de Veículo", placa e nome do proprietário em destaque
- **Dados do Veículo**: Seção organizada com placa, marca, modelo e ano
- **Dados do Proprietário**: Informações do proprietário
- **Histórico de Manutenção**: Tabela com colunas Data, Serviço, Quilometragem e Custo (Se houver mais de um valor, realiza o somatório total)

## 🛠️ Build e Deploy

```bash
# Build do projeto
dotnet build

# Para executar o projeto
dotnet run

## 📞 Suporte

Para dúvidas ou problemas, consulte os logs da aplicação ou verifique a documentação do Swagger em `/swagger`.
