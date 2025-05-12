using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ZA_PLACE.Models;
using ZA_PLACE.Repository;
using ZA_PLACE.Repository.Base;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Stripe;
using ZA_PLACE.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure DbContext with connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("myConnection")));

// Configure Identity services and use UserExtra as the application user model
builder.Services.AddIdentity<UserExtra, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI(); // Enables Razor UI for Identity (Login, Register, etc.)

// Add custom repository services
builder.Services.AddTransient<IEmailSender>(provider =>
    new EmailSender(
        builder.Configuration["EmailSettings:SmtpServer"],
        int.Parse(builder.Configuration["EmailSettings:Port"]),
        builder.Configuration["EmailSettings:Username"],
        builder.Configuration["EmailSettings:Password"]
    ));
builder.Services.AddTransient(typeof(IRepository<>), typeof(MainRepository<>));

// Configure Identity cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

// Add Razor Pages (needed for Identity UI)
builder.Services.AddRazorPages();

// Add logging (including console logging)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Configure Stripe before building the app
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Build the app
var app = builder.Build();

// Ensure database is created and migrations are applied at runtime
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();  // Apply pending migrations
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
app.UseStaticFiles();