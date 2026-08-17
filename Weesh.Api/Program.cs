using Scalar.AspNetCore;
using Weesh.Infra.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders().AddConsole();
builder.Services.AddLogging(options => options.SetMinimumLevel(LogLevel.Trace).AddConsole());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();
// TODO: Global Exception Handler

builder.Services.AddWeeshDbContext(builder.Configuration);
builder.Services.AddWeeshIdentity(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsPolicies(builder.Configuration);

// DI 


var app = builder.Build();

await app.InitializeWeeshDbAsync();

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