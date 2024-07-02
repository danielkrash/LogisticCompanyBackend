using Carter;
using LogisticCompany.data;
using LogisticCompany.Data.Models.auth;
using LogisticCompany.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

namespace LogisticCompany.config;

public static class AppConfig
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            // options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });
        services.AddCors(options =>
        {
            options.AddPolicy("AllowNextJS", builder =>
            {
                builder.WithOrigins("http://localhost:3000", "https://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        services.AddProblemDetails();
        // var connectionString =
        //     $" Host={Environment.GetEnvironmentVariable("HOST")};" +
        //     $" Database={Environment.GetEnvironmentVariable("DATABASE")};" +
        //     $" Username={Environment.GetEnvironmentVariable("USER")};" +
        //     $" Password={Environment.GetEnvironmentVariable("PASSWORD")};" +
        //     $" SearchPath={Environment.GetEnvironmentVariable("SEARCHPATH")};";
        var connectionString =
            $" Host={Environment.GetEnvironmentVariable("HOST")};" +
            $" Database={Environment.GetEnvironmentVariable("DATABASE")};" +
            $" Username={Environment.GetEnvironmentVariable("USER")};" +
            $" Password={Environment.GetEnvironmentVariable("PASSWORD")};" +
            $" SearchPath={Environment.GetEnvironmentVariable("SEARCHPATH")};";

        services.AddDbContext<LogisticCompanyContext>(options => { options.UseNpgsql(connectionString); }
        );
        services.AddAuthorization();
        services.AddIdentityApiEndpoints<ApplicationUser>()
            .AddRoles<UserRole>()
            .AddEntityFrameworkStores<LogisticCompanyContext>();
        // Configure App Policies;
        ConfigurePolicies(services);
        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddCarter();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public static void ConfigureWebHost(IWebHostBuilder webHost)
    {
        webHost.ConfigureKestrel((context, options) =>
        {
            options.ListenAnyIP(7028, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                // listenOptions.UseHttps();
            });
        });
    }

    public static async Task SeedDatabase(WebApplication webapp)
    {
        //Seed Roles For Users
        using var scope = webapp.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>();
        var roles = new[] { "admin", "manager", "employee", "customer" };
        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new UserRole(role));
        //Seed Admin User
        using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        const string adminEmail = "danikmass121@gmail.com";
        const string password = "546231redQ!";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser
            {
                Email = adminEmail,
                UserName = adminEmail,
                FirstName = "Daniel",
                LastName = "Denisov",
                BirthDate = DateOnly.Parse("2002-05-27"),
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, password);
            await userManager.AddToRoleAsync(adminUser, "admin");
        }
    }

    private static void ConfigurePolicies(IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("admin_only", policy =>
                policy
                    .RequireRole("admin"))
            .AddPolicy("user_only", policy =>
                policy
                    .RequireRole("user"))
            .AddPolicy("admin_manager_employee", policy =>
                policy
                    .RequireRole("manager", "admin" , "employee"))
            .AddPolicy("manager_user", policy =>
                policy
                    .RequireRole("manager", "user"));
    }
}