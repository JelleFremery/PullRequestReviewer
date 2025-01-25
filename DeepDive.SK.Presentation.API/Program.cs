using DeepDive.SK.Application;
using DeepDive.SK.Infrastructure.ADO;
using DeepDive.SK.Infrastructure.SemanticKernel;
using DeepDive.SK.ServiceDefaults;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
// Add services to the container.
builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Custom Dependency Injection
builder.Services.ConfigureApplicationDependencies();
builder.Services.ConfigureAdoDependencies();
builder.Services.ConfigureInfrastructureDependencies();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Servers = [];
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/",
    (HttpContext httpContext) => "Hello world")
    .WithName("GetHelloWorld");

app.MapGet("/repositories",
    async ([FromServices] IReviewerUseCase useCase, [FromQuery] string? searchTerm) =>
    {
        return await useCase.SearchRepositories(searchTerm);
    })
    .WithName("GetRepositories");

app.MapPost("/pr",
    async ([FromServices] IReviewerUseCase prService, [FromBody] Guid repositoryId, int pullRequestId) =>
    {
        await prService.ReviewPullRequestAsync(repositoryId, pullRequestId);
    })
    .WithName("ReviewPullRequest");

app.Run();