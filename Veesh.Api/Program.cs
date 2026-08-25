using Scalar.AspNetCore;
using Veesh.Core.DTOs.User;
using Veesh.Core.Interfaces;
using Veesh.Core.Services;
using Veesh.Infra.Extensions;
using Veesh.Infra.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders().AddConsole();
builder.Services.AddLogging(options => options.SetMinimumLevel(LogLevel.Trace).AddConsole());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();
// TODO: Global Exception Handler

builder.Services.AddVeeshDbContext(builder.Configuration);
builder.Services.AddVeeshIdentity(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsPolicies(builder.Configuration);

// DI 
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IWishListRepository, WishListRepository>();
builder.Services.AddScoped<IWishRepository, WishRepository>();
builder.Services.AddScoped<IWishStoreUrlRepository, WishStoreUrlRepository>();
builder.Services.AddScoped<WishListService>();
builder.Services.AddScoped<WishService>();
builder.Services.AddScoped<WishStoreUrlService>();

var app = builder.Build();

await app.InitializeVeeshDbAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    // TODO: add dev cors
}
else
{
    // TODO: add prod cors
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
