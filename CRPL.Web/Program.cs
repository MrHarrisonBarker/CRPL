using System.Text;
using CRPL.Data;
using CRPL.Web;
using CRPL.Web.Core;
using CRPL.Web.StartUp;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);
var appSettings = appSettingsSection.Get<AppSettings>();

builder.Host.UseSerilog((context, services, configuration) => configuration
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails()
        .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.Seq("http://www.seq-ingest.harrisonbarker.co.uk", apiKey: appSettings.SeqKey)
);

// Add services to the container.

builder.Services.AddAutoMapper(expression => expression.AddProfile(typeof(AutoMapping)));

builder.Services.AddChainConnections();

builder.Services.AddDbPipeline(appSettings);
builder.Services.AddServicePipeline(appSettings);

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile("./crpl-c5132-firebase-adminsdk-pkggb-4bf090805b.json"),
});

var key = Encoding.ASCII.GetBytes(appSettings.EncryptionKey);
builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            LifetimeValidator = Utils.LifetimeValidator
        };
    });

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        // options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
    }
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.UseSeeding();

app.WakeServices();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseUsageProxy();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "CRPL"));

app.Run();