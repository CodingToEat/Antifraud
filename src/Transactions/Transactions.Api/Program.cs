using Microsoft.EntityFrameworkCore;

using Transactions.Api.Features;
using Transactions.Api.Models;
using Transactions.Api.Persistence;
using Transactions.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TransactionDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
builder.Services.AddScoped<CreateTransactionHandler>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TransactionDbContext>();
    db.Database.Migrate();
}

app.MapPost("/transactions", async (CreateTransactionRequest request, CreateTransactionHandler handler) =>
    await handler.HandleAsync(request));


app.MapPut("/transactions/{externalId}/status", async (Guid externalId, UpdateTransactionStatusRequest request, ITransactionRepository repository) =>
{
    await repository.UpdateStatusAsync(externalId, request.Status);
    return Results.NoContent();
});


app.MapGet("/transactions/{externalId}", async (
    Guid externalId,
    ITransactionRepository repository) =>
{
    var tx = await repository.GetByExternalIdAsync(externalId);
    return tx is null ? Results.NotFound() : Results.Ok(tx);
});

app.Run();
