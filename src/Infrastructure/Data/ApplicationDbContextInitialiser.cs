using MeterReadingUploadAPI.Domain.Constants;
using MeterReadingUploadAPI.Domain.Entities;
using MeterReadingUploadAPI.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MeterReadingUploadAPI.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            // See https://jasontaylor.dev/ef-core-database-initialisation-strategies
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole(Roles.Administrator);

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // Default users
        var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
            }
        }

        // Default data
        // Seed, if necessary
        if (!_context.TodoLists.Any())
        {
            _context.TodoLists.Add(new TodoList
            {
                Title = "Todo List",
                Items =
                {
                    new TodoItem { Title = "Make a todo list 📃" },
                    new TodoItem { Title = "Check off the first item ✅" },
                    new TodoItem { Title = "Realise you've already done two things on the list! 🤯"},
                    new TodoItem { Title = "Reward yourself with a nice, long nap 🏆" },
                }
            });

            await _context.SaveChangesAsync();
        }

        // Default data
        if (!_context.Accounts.Any())
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Accounts] ON");


                _context.Accounts.AddRange(
                    new Account { Id = 1234, FirstName = "Freya", LastName = "Test" },
                    new Account { Id = 1239, FirstName = "Noddy", LastName = "Test" },
                    new Account { Id = 1240, FirstName = "Archie", LastName = "Test" },
                    new Account { Id = 1241, FirstName = "Lara", LastName = "Test" },
                    new Account { Id = 1242, FirstName = "Tim", LastName = "Test" },
                    new Account { Id = 1243, FirstName = "Graham", LastName = "Test" },
                    new Account { Id = 1244, FirstName = "Tony", LastName = "Test" },
                    new Account { Id = 1245, FirstName = "Neville", LastName = "Test" },
                    new Account { Id = 1246, FirstName = "Jo", LastName = "Test" },
                    new Account { Id = 1247, FirstName = "Jim", LastName = "Test" },
                    new Account { Id = 1248, FirstName = "Pam", LastName = "Test" },
                    new Account { Id = 2233, FirstName = "Barry", LastName = "Test" },
                    new Account { Id = 2344, FirstName = "Tommy", LastName = "Test" },
                    new Account { Id = 2345, FirstName = "Jerry", LastName = "Test" },
                    new Account { Id = 2346, FirstName = "Ollie", LastName = "Test" },
                    new Account { Id = 2347, FirstName = "Tara", LastName = "Test" },
                    new Account { Id = 2348, FirstName = "Tammy", LastName = "Test" },
                    new Account { Id = 2349, FirstName = "Simon", LastName = "Test" },
                    new Account { Id = 2350, FirstName = "Colin", LastName = "Test" },
                    new Account { Id = 2351, FirstName = "Gladys", LastName = "Test" },
                    new Account { Id = 2352, FirstName = "Greg", LastName = "Test" },
                    new Account { Id = 2353, FirstName = "Tony", LastName = "Test" },
                    new Account { Id = 2355, FirstName = "Arthur", LastName = "Test" },
                    new Account { Id = 2356, FirstName = "Craig", LastName = "Test" },
                    new Account { Id = 4534, FirstName = "JOSH", LastName = "TEST" },
                    new Account { Id = 6776, FirstName = "Laura", LastName = "Test" },
                    new Account { Id = 8766, FirstName = "Sally", LastName = "Test" }
                );

                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Accounts] OFF");


                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
