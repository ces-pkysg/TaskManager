using Microsoft.EntityFrameworkCore;
using TaskManager.Context;
using TaskManager.Utilities.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);

builder.Services.AddServices(); //Añade todos los servicios que esten en nuestro metodo de extención (va antes del constructor)

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
//EL MIDLLEWARE VA ANTES DEL MAPEO DE LOS CONTROLADORES 
app.UseMiddleware<GlobalErrorHandlerMiddleware>();//REGISTRA EL MIDDLEWARE DE MNEJO GLOBAL DE ERRORES

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
