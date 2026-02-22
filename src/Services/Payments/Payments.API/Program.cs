
var builder = WebApplication.CreateBuilder(args);
//builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//          .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
//          .AddEnvironmentVariables();
// Add services to the container.
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment)
                 .AddApplicationServices(builder.Configuration)
                 .AddPresentation(builder.Configuration);
builder.Services.AddConfigurations(builder.Configuration);
builder.Host.ConfigureSerilog();
builder.Services.AddOpenApi();
var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseDatabaseMigration();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors();
}
else
{
    app.UseHsts();
    //production cors policy
    //app.UseCors();
}

app.UseCorrelationIdMiddleware();
app.UsePerformanceLoggingMiddleware();
app.UseAdvancedRequestLogging();
app.UseHttpsRedirection();
app.UseCustomExceptionHandler();
app.UseStatusCodePagesWithReExecute("/errors/{0}");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
