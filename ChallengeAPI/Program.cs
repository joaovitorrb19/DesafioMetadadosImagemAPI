using ChallengeAPI.Interfaces;
using ChallengeAPI.Services;
using ChallengeAPI.Repositories;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Configurações do MongoDB
var mongoSettings = builder.Configuration.GetSection("MongoDB");
string? connectionString = mongoSettings.GetValue<string>("ConnectionString");
string? databaseName = mongoSettings.GetValue<string>("DatabaseName");

var client = new MongoClient(connectionString);
var database = client.GetDatabase(databaseName);

builder.Services.AddSingleton(database);
builder.Services.AddSingleton<IMetadadosDeImagemRepository, MetadadosDeImagemRepository>();
builder.Services.AddSingleton<IMetadadosDeImagemService, MetadadosDeImagemService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
