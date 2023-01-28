using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using PetsAPI.Data;
using PetsAPI.Repositories;
using PetsAPI.Repositories.Interfaces;
using PetsAPI.Services;
using PetsAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IDogRepository, DogRepository>();
builder.Services.AddScoped<IDogService, DogService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Automapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Add Fluent Validation
builder.Services.AddFluentValidation(options =>
{
    options.RegisterValidatorsFromAssemblyContaining<Program>();
});

// Add DbContext
builder.Services.AddDbContext<PetsDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection"));
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