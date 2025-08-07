using Microsoft.AspNetCore.Identity;
using OncoTrack.Data;
using OncoTrack.Extensions;
using OncoTrack.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add all OncoTracker-specific services
builder.Services.AddOncoTrackerServices(builder.Configuration);

var app = builder.Build();

// Configure the OncoTracker pipeline
app.ConfigureOncoTrackerPipeline();

// Seed initial data
await app.SeedOncoTrackerDataAsync();

app.Run();