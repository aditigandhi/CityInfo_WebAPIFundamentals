using CityInfo.API;
using CityInfo.API.Services;
using Microsoft.AspNetCore.StaticFiles;
using Serilog;

// utilizing serilog -- logging to file and console
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
/* this is for reg logging not using serilog */
//builder.Logging.ClearProviders(); // nothing is logged
//builder.Logging.AddConsole(); // bring logs to console log

builder.Host.UseSerilog();


// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true; // when consumer asks for option that we do not have configured
}).AddNewtonsoftJson()
  .AddXmlDataContractSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

# if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();

#else
builder.Services.AddTransient<IMailService, CloudMailService>();

#endif

builder.Services.AddSingleton<CitiesDataStore>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) // only adds swagger middleware when running in dev env
{ //Middleware -- swagger generalizaion used in swagger ui 
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

///////////////////////////////////////// NOTES ///////////////////////////////////////////////////

/* Model View Controller
 * Loose coupling, separation of pattern
 * Model: app logic
 * View: displaying data
 * Controller: handling interaction between model and view
 */