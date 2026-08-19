using Scalar.AspNetCore;
using Veesh.Infra.Extensions;

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