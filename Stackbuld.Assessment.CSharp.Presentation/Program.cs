using LoanApplication.Presentation.Middleware;
using Microsoft.AspNetCore.HttpOverrides;
using Scalar.AspNetCore;
using Stackbuld.Assessment.CSharp.Application.Extensions;
using Stackbuld.Assessment.CSharp.Infrastructure.Extensions;
using Stackbuld.Assessment.CSharp.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddPresentation()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options.WithTitle("Stackbuld API")
        .WithTheme(ScalarTheme.BluePlanet)
        .EnableDarkMode()
        .SortTagsAlphabetically()
        .SortOperationsByMethod()
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
        .AddPreferredSecuritySchemes("Bearer");
});

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
});

app.UseHsts();
app.UseHttpsRedirection();
app.UseMiddleware<TimingMiddleware>();
app.UseExceptionHandler();
app.MapEndpoints();

app.Run();