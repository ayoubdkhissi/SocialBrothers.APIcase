using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SocialBrothers.APIcase.Domain.Interfaces;
using SocialBrothers.APIcase.Infrastructure;
using SocialBrothers.APIcase.Infrastructure.Services;
using SocialBrothers.APIcase.Presentation.Controllers;
using SocialBrothers.APIcase.Presentation.DTOs;
using SocialBrothers.APIcase.Presentation.Validators;
using System.Reflection;

// Configuring logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/addressAPI.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    string connection_string = builder.Configuration.GetConnectionString("SQLiteDb");
    options.UseSqlite(connection_string);  
});

builder.Services.AddScoped<IAddressRepository, AddressRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IValidator<AddressDto>, AddressDtoValidator>();

builder.Services.AddHttpClient<AddressesController>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
