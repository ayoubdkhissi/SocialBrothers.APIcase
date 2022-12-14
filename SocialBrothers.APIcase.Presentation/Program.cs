using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SocialBrothers.APIcase.Domain.Interfaces;
using SocialBrothers.APIcase.Infrastructure;
using SocialBrothers.APIcase.Infrastructure.Services;
using SocialBrothers.APIcase.Presentation.DTOs;
using SocialBrothers.APIcase.Presentation.Validators;

var builder = WebApplication.CreateBuilder(args);

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


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
