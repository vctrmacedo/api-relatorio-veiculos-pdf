using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Injeção de dependência
builder.Services.AddScoped<RelatorioService>();
builder.Services.AddValidatorsFromAssemblyContaining<RelatorioRequestValidator>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Adicionar middleware global de tratamento de erros
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();