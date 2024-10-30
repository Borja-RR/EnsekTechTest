using EnsekTechTest.Repository;
using EnsekTechTest.Repository.Repository;
using EnsekTechTest.Repository.Repository.Interfaces;
using EnsekTechTest.Services.CsvHelper;
using EnsekTechTest.Services.CsvHelper.Interfaces;
using EnsekTechTest.Services.Meter;
using EnsekTechTest.Services.Meter.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with dependency injection and configure the connection string
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("WebApiDatabase")));

// Add services to the container
builder.Services.AddScoped<ICsvService, CsvService>();
builder.Services.AddScoped<IMeterReadingService, MeterReadingService>();
builder.Services.AddScoped<IMeterReadingValidationService, MeterReadingValidationService>();
builder.Services.AddScoped<IMeterUploaderService, MeterUploaderService>();
builder.Services.AddScoped<IMetersRepository, MetersRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Create database if it does not exist yet
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    context.Database.EnsureCreated();
}

app.Run();
