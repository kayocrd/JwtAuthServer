using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;
using JwtAuthServer;
using JwtAuthServer.Data;
using JwtAuthServer.Repositories;
using JwtAuthServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
LoadConfiguration(builder);
ConfigureAuthentication(builder);
ConfigureAuthorization(builder);
ConfigureMvc(builder);
ConfigureServices(builder);

var app = builder.Build(); 


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//create database if not exists
// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     var context = services.GetRequiredService<AppDbContext>();
//     context.Database.EnsureCreated();
// }

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

void LoadConfiguration(WebApplicationBuilder builder)
{
    Settings.Secret = builder.Configuration.GetValue<string>("Secret");
    Settings.Issuer = builder.Configuration.GetValue<string>("Issuer");
    Settings.ApiKey = builder.Configuration.GetValue<string>("ApiKey");
    Settings.ExpirationHours = builder.Configuration.GetValue("ExpirationHours", 1);
    Settings.WhatsappConfig.Endpoint = builder.Configuration.GetValue<string>("WhatsappConfig:Endpoint");
    Settings.WhatsappConfig.InstanceId = builder.Configuration.GetValue<string>("WhatsappConfig:InstanceId");
    Settings.WhatsappConfig.Token = builder.Configuration.GetValue<string>("WhatsappConfig:Token");
}
void ConfigureAuthentication(WebApplicationBuilder builder)
{
    var key = Encoding.ASCII.GetBytes(Settings.Secret);
    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = Settings.Issuer,
            ValidateAudience = false,
        };
    });
}
void ConfigureAuthorization(WebApplicationBuilder builder)
{
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("PendingVerification", policy =>
        {
            policy.RequireClaim("IsVerified", "False");
            policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        });

    });
}
void ConfigureMvc(WebApplicationBuilder builder)
{
    builder.Services.AddResponseCompression(options =>
    {
        // options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
        // options.Providers.Add<CustomCompressionProvider>();
    });
    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Optimal;
    });
    builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        })
        .AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            x.JsonSerializerOptions.WriteIndented = true;
            x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
}
void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection"));
    });
    builder.Services.AddTransient<TokenService>();
    builder.Services.AddTransient<UserRepository>();
    builder.Services.AddTransient<IAccountService, AccountService>();
    builder.Services.AddHttpClient();
    builder.Services.AddSingleton<IWhatsappService, WhatsappService>();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}