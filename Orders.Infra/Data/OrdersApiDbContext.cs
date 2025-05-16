using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orders.Domain.Entities;
using System.Security.Claims;

namespace Orders.Infra.Data
{
    public class OrdersApiDbContext : IdentityDbContext
    {
        public OrdersApiDbContext(DbContextOptions<OrdersApiDbContext> options)
        : base(options)
        {
        }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .ToTable("Orders")
                .HasIndex(x => x.Id);

        }

        public async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            using var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager);
            await SeedRoleClaimsAsync(roleManager);
        }

        private async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Manager", "User", "Guest" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private async Task SeedUsersAsync(UserManager<IdentityUser> userManager)
        {
            var adminUser = new IdentityUser
            {
                UserName = "admin@pedidos.com",
                Email = "admin@pedidos.com",
                EmailConfirmed = true
            };

            var user = await userManager.FindByEmailAsync(adminUser.Email);
            if (user == null)
            {
                var createPowerUser = await userManager.CreateAsync(adminUser, "Admin@123");
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        private async Task SeedRoleClaimsAsync(RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync("Admin");
            if (adminRole != null)
            {
                var claims = new[]
                {
                new Claim("Pedidos.FullAccess", "true"),
                new Claim("Pedidos.Delete", "true"),
                new Claim("Users.Manage", "true")
            };

                foreach (var claim in claims)
                {
                    var existingClaim = roleManager
                        .GetClaimsAsync(adminRole)
                        .Result
                        .FirstOrDefault(c => c.Type == claim.Type);

                    if (existingClaim == null)
                    {
                        await roleManager.AddClaimAsync(adminRole, claim);
                    }
                }
            }
        }
    }
}
