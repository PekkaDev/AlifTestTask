using Microsoft.EntityFrameworkCore;
using WalletAPI;
using WalletAPI.Data;
using WalletAPI.Data.DTO;
using WalletAPI.Data.RequestDTO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WalletDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<WalletService>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseMiddleware<WalletAuthenticationMiddleware>();

var walletApi = app.MapGroup("/wallet")
    .WithParameterValidation();

walletApi.MapPost("/check", async (CheckRequestDTO request, WalletService walletService) =>
{
    var wallet = await walletService.FindByUserLogin(request.UserLogin);
    
    return Results.Ok(new
    {
        Exist = wallet != null
    });
});

walletApi.MapPost("/topup", async (TopUpRequestDTO request, WalletService walletService) =>
{
    var wallet = await walletService.FindByUserLogin(request.UserLogin);

    if (wallet == null)
        return Results.Problem("Wallet with the specified id does not exist", statusCode: 404);

    var success = await walletService.TopUpAsync(wallet, request.Amount);

    if (!success)
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            { "amount", ["Exceeds maximum balance"] }
        });

    return Results.Ok();
});

walletApi.MapPost("monthly-summary", async (MonthlySummaryRequestDTO request, WalletService walletService) =>
{
    var wallet = await walletService.FindByUserLogin(request.UserLogin);

    if (wallet == null)
        return Results.Problem("Wallet with the specified id does not exist", statusCode: 404);

    var topUpTransactions = walletService.GetTransactionsForCurrentMonth(wallet);

    return TypedResults.Ok(new MonthlySummaryResponseDTO()
    {
        TransactionCount = topUpTransactions.Count,
        TransactionSum = topUpTransactions.Sum(it => it.TransactionAmount),
        TopUpTransactions = topUpTransactions
            .Select(it => new TransactionDTO
            {
                Amount = it.TransactionAmount,
                TransactionDate = it.TransactionDate
            })
            .ToList()
    });
});

walletApi.MapPost("/balance", async (BalanceRequestDTO request, WalletService walletService) =>
{
    var wallet = await walletService.FindByUserLogin(request.UserLogin);

    if (wallet == null)
        return Results.Problem("Wallet with the specified id does not exist", statusCode: 404);

    var balance = wallet.Transactions.Sum(it => it.TransactionAmount);

    return Results.Ok(new
    {
        Balance = balance
    });
});


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WalletDbContext>();

    context.Database.Migrate();
    WalletDbInitializer.Initialize(context);
}

app.Run();