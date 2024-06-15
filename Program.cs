using Microsoft.EntityFrameworkCore;
using WalletAPI;
using WalletAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WalletDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddProblemDetails();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler();

app.MapGet("/", () => "Hello World!");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WalletDbContext>();

    context.Database.Migrate();
    WalletDbInitializer.Initialize(context);
}

app.Run();