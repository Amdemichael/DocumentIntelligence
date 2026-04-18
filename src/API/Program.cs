using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Domain.Interfaces;
using Infrastructure.Repositories;
using MediatR;
using FluentValidation;
using System.Reflection;
using API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
var connectionString = "Data Source=DocumentIntelligence.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Repositories
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

// Add MediatR from Application layer
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Application.Features.Documents.Commands.CreateDocument.CreateDocumentCommand).Assembly);
    // Add pipeline behaviors
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(Application.Common.Behaviors.ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(Application.Common.Behaviors.LoggingBehavior<,>));
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Application.Features.Documents.Commands.CreateDocument.CreateDocumentCommandValidator).Assembly);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Create database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

// Add exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();