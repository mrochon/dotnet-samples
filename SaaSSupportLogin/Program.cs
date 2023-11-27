using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Events ??= new OpenIdConnectEvents();
    var nextRedirectHandler = options.Events.OnRedirectToIdentityProvider;
    options.Events.OnRedirectToIdentityProvider = async ctx =>
    {
        if (ctx.Properties.Parameters.ContainsKey("login_hint"))
        {
            ctx.ProtocolMessage.LoginHint = ((System.Security.Claims.Claim)ctx.Properties.Parameters["login_hint"]).Value;
        }
        await nextRedirectHandler(ctx);
    };
});

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.Use(async (context, next) =>
{
    await next.Invoke();
    // Wait for the redirect with login_hint and change it to a specific tenant
    if(context.Response.StatusCode == 302)
    {
        var url = context.Response.Headers["Location"][0];
        if(url.StartsWith("https://login.microsoftonline.com/organizations/", StringComparison.OrdinalIgnoreCase)
           &&  url.Contains("&login_hint=")
           && context.Items.ContainsKey("targetDomain"))
        {
            url = url.Replace("organizations", (string) context.Items["targetDomain"]);
            context.Response.Headers["Location"] = url;
        }
    }
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
