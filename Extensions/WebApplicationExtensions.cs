using Microsoft.AspNetCore.Identity;
using OncoTrack.Data;
using OncoTrack.Models;

namespace OncoTrack.Extensions
{
    public static class WebApplicationExtensions
    {
        /// <summary>
        /// Configures the OncoTracker middleware pipeline
        /// </summary>
        public static WebApplication ConfigureOncoTrackerPipeline(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            return app;
        }

        /// <summary>
        /// Seeds initial data for OncoTracker (roles, admin users, etc.)
        /// </summary>
        public static async Task<WebApplication> SeedOncoTrackerDataAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.EnsureCreated();
                    
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    
                    // Create roles if they don't exist
                    string[] roles = { "Doctor", "Patient", "Admin" };
                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }
                    }
                    
                    // Add any additional seeding logic here
                    // For example, creating default admin user, sample data, etc.
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            return app;
        }
    }
}
