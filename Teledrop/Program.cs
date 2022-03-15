using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Teledrop.Configurations;
using Teledrop.Models;
using Teledrop.Services;
using X.Extensions.Logging.Telegram;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHangfire(configuration => configuration
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("TeledropHangfireDbConnection")));

builder.Services.AddHangfireServer(x => x.WorkerCount = 1);

builder.Services.AddHttpClient();

builder.Services.AddScoped<TelegramService>();
builder.Services.AddScoped<InstaService>();

builder.Services.Configure<TelegramConfiguration>(builder.Configuration.GetSection("Telegram"));

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddDbContext<TeledropDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("TeledropDbConnection")));

builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddTelegram(builder.Configuration);

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

app.UseHangfireDashboard();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
