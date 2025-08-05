using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Saba;
using Saba.Domain.Models;
using Saba.Repository;
using NLog.Web;
using NLog;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");
try
{
    logger.Info("Starting application...");

    // Create a builder for the application
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("AppSettings:SmtpSetting"));
    builder.Services.AddDbContext<SabaContext>(options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));
    builder.Services.AddControllers();
    builder.Services.AddRepositories();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddHttpClient();
    builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(builder.Environment.ContentRootPath));

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAllOrigins",
            builder => builder.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader());
    });

    builder.Services.AddSwaggerGen(opt =>
    {
        opt.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter the Bearer Authorization string as following: `Generated-JWT-Token`",
            In = ParameterLocation.Header,
            //Al establecer SecuritySchemeType.Http y el esquema, no es necesario anteponer el prefijo bearer en la autorizacion del swagger
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer",
        });
        opt.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
        {
           new OpenApiSecurityScheme
             {
                 Reference = new OpenApiReference
                 {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                 }
             },
             new string[] {}
        }
        });
    });

    builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(opt =>
    {
        opt.RequireHttpsMetadata = false;
        opt.SaveToken = true;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            //  ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:SecretToken"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    //if (app.Environment.IsDevelopment())
    //{
    app.UseSwagger();
    app.UseSwaggerUI();
    //}

    app.UseHttpsRedirection();
    app.UseCors("AllowAllOrigins");
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseStaticFiles();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
