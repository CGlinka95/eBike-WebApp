using eBikeWebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

#region Additional namespaces
using ServicingSystem;
using RecievingSystem;
using SalesSystem;
using PurchasingSystem;
using AppSecurity.BLL;
using AppSecurity;
#endregion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//Added:
builder.Services.AddServicingDependencies(options =>
 options.UseSqlServer(connectionString));

builder.Services.AddRecievingDependencies(options =>
 options.UseSqlServer(connectionString));

builder.Services.AddSalesDependencies(options =>
 options.UseSqlServer(connectionString));

builder.Services.AddPurchasingDependencies(options =>
 options.UseSqlServer(connectionString));

builder.Services.AppSecurityBackendDependencies(options =>
 options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/PurchasingPages")
        .AllowAnonymousToPage("/PurchasingPages/Index");
    options.Conventions.AuthorizeFolder("/RecievingPages")
        .AllowAnonymousToPage("/RecievingPages/Index");
    options.Conventions.AuthorizeFolder("/SalesPages")
        .AllowAnonymousToPage("/SalesPages/Index");
    options.Conventions.AuthorizeFolder("/ServicingPages")
        .AllowAnonymousToPage("/ServicingPages/Index");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
await ApplicationUserSeeding(app);
app.Run();

static async Task ApplicationUserSeeding(IHost host)
{
    using (var scope = host.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<Program>();
        var env = services.GetRequiredService<IWebHostEnvironment>();
        if (env is not null && env.IsDevelopment())
        {
            try
            {
                var configuration = services.GetRequiredService<IConfiguration>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                if (!userManager.Users.Any())
                {
                    var securityService = services.GetRequiredService<SecurityService>();
                    var users = securityService.ListEmployees();
                    string password = configuration.GetValue<string>("Setup:InitialPassword");
                    foreach (var person in users)
                    {
                        var user = new ApplicationUser
                        {
                            UserName = person.UserName,
                            Email = person.Email,
                            EmployeeId = person.EmployeeId,
                            EmailConfirmed = true
                        };
                        var result = await userManager.CreateAsync(user, password);
                        if (!result.Succeeded)
                        {
                            logger.LogInformation("User was not created");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "An error occurred seeing the website users");
            }
        }
    }
}
