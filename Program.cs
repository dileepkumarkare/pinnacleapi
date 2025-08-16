using Pinnacle.Extensions;
using Pinnacle.Helpers.JWT;
using Microsoft.Extensions.FileProviders;
using Pinnacle.Helpers;
using Serilog;
using DevExpress.XtraCharts;
using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Pinnacle.IServices;
using Pinnacle.Services;
using Pinnacle.Models;
using IAuthenticationService = Pinnacle.IServices.IAuthenticationService;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers(config =>
{
    config.Filters.Add<AuditLogFilter>();
});
builder.Services.AddScoped<IWhatsappService, WhatsappService>();
builder.Services.AddScoped<IItemMasterService, ItemMasterService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<AuditLogFilter>();


builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                    }
                },
                new string[] {}
        }
    });
});

builder.Services.AddJWTTokenServices(builder.Configuration);
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
Log.Information("Pinnacle Rocks here!!");

var app = builder.Build();

//app.UseMiddleware<JwtToken>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(builder => builder
//.WithOrigins(new string[] { "http://localhost:3000", "http://localhost:3001", "http://pinnacle.bluhealthapp.com", "https://pinnacle.bluhealthapp.com", "http://pinnacletest.bluhealthapp.com", "https://pinnacletest.bluhealthapp.com" })
.WithOrigins(new string[] { "http://localhost:3000", "http://localhost:3001", "http://emr.pinnaclehospitals.com", "https://emr.pinnaclehospitals.com" })
.AllowAnyHeader()
.AllowAnyMethod()
.AllowCredentials()
    );
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), @"Uploads")),
    RequestPath = new PathString("/assets")

});
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseAuthorization();
//app.UseMiddleware<JwtToken>();

app.Run();
