using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.Authorization;
using Graph = Microsoft.Graph;
using blazor.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var initialScopes = builder.Configuration["GraphApi:Scopes"]?.Split(' ').ToList();
initialScopes = initialScopes.Concat(builder.Configuration["AzureApi:Scopes"]?.Split(' ')).ToList();

builder.Services.AddScoped<GraphService>();
builder.Services.AddScoped<AzureService>();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
        .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
            .AddDownstreamWebApi("GraphApi", builder.Configuration.GetSection("GraphApi"))
            .AddDownstreamWebApi("AzureApi", builder.Configuration.GetSection("AzureApi"))
            .AddInMemoryTokenCaches();
builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
