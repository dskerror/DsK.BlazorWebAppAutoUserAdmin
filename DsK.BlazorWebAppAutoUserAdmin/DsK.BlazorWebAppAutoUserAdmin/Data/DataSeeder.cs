using DsK.BlazorWebAppAutoUserAdmin.Data;
using Microsoft.AspNetCore.Identity;

public class DataSeeder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(IServiceProvider serviceProvider, ILogger<DataSeeder> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task SeedDataAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await CreateRoles(roleManager);
        await CreateDefaultUser(userManager);
    }

    private async Task CreateRoles(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "Employee", "Adjuster" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Role '{role}' created successfully.");
                }
                else
                {
                    _logger.LogError($"Error creating role '{role}': {result.Errors}");
                }
            }
        }
    }

    private async Task CreateDefaultUser(UserManager<ApplicationUser> userManager)
    {
        var email = "admin@example.com";
        var password = "Admin@123";
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Employee");
                _logger.LogInformation($"Default employee user '{email}' created successfully.");
            }
            else
            {
                _logger.LogError($"Error creating default admin user '{email}': {result.Errors}");
            }
        }
    }
}