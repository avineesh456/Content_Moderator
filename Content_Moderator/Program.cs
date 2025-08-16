
using Content_Moderator;
using Microsoft.Extensions.ML;
using Microsoft.ML.Data;
using System.Data;
using static Content_Moderator.MLModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPredictionEnginePool<ModelInput, ModelOutput>()
    .FromFile(filePath: "Custom_model.zip", watchForChanges: true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseCors(); // Use the CORS policy
app.UseAuthorization();

app.MapControllers();

app.Run();
