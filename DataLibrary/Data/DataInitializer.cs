using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataLibrary.Data;

public class DataInitializer(BankAppDataContext dbContext, UserManager<IdentityUser> userManager)
{
    public void SeedData()
    {
        dbContext.Database.Migrate();
        SeedRoles();
        SeedUsers();
    }

    // Här finns möjlighet att uppdatera dina användares loginuppgifter
    private void SeedUsers()
    {
        AddUserIfNotExists("richard.chalk@systementor.se", "Hejsan123#", new string[] { "Admin" });
        AddUserIfNotExists("richard.erdos.chalk@gmail.se", "Hejsan123#", new string[] { "Cashier" });
    }

    // Här finns möjlighet att uppdatera dina användares roller
    private void SeedRoles()
    {
        AddRoleIfNotExisting("Admin");
        AddRoleIfNotExisting("Cashier");
    }

    private void AddRoleIfNotExisting(string roleName)
    {
        var role = dbContext.Roles.FirstOrDefault(r => r.Name == roleName);
        if (role == null)
        {
            dbContext.Roles.Add(new IdentityRole { Name = roleName, NormalizedName = roleName });
            dbContext.SaveChanges();
        }
    }

    private void AddUserIfNotExists(string userName, string password, string[] roles)
    {
        if (userManager.FindByEmailAsync(userName).Result != null) return;

        var user = new IdentityUser
        {
            UserName = userName,
            Email = userName,
            EmailConfirmed = true
        };
        userManager.CreateAsync(user, password).Wait();
        userManager.AddToRolesAsync(user, roles).Wait();
    }
}