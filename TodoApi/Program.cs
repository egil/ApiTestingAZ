using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using TodoApi;
using TodoApi.Data;
using TodoApi.Todos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.AddTodoDatabase();

// Change formatting of JSON
builder.ConfigureApiSerialization();

// Add services to the container.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();

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
app.UseCustomErrorPayloads();

app.MapTodosEndpoints();

app.Run();

// Exposed to allow test project to see Program
public partial class Program { }