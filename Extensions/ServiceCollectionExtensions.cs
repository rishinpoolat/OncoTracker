using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OncoTrack.Configuration;
using OncoTrack.Data;
using OncoTrack.Models;

namespace OncoTrack.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Entity Framework and Identity services to the service collection
        /// </summary>
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Entity Framework with SQLite
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    configuration.GetConnectionString("DefaultConnection") ?? 
                    "Data Source=oncotrack.db"
                ));

            // Bind configuration options
            var identityOptions = new OncoTrackerIdentityOptions();
            configuration.GetSection(OncoTrackerIdentityOptions.SectionName).Bind(identityOptions);

            // Configure Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = identityOptions.Password.RequireDigit;
                options.Password.RequiredLength = identityOptions.Password.RequiredLength;
                options.Password.RequireNonAlphanumeric = identityOptions.Password.RequireNonAlphanumeric;
                options.Password.RequireUppercase = identityOptions.Password.RequireUppercase;
                options.Password.RequireLowercase = identityOptions.Password.RequireLowercase;
                
                // User settings
                options.User.RequireUniqueEmail = identityOptions.User.RequireUniqueEmail;
                
                // Sign in settings
                options.SignIn.RequireConfirmedEmail = identityOptions.SignIn.RequireConfirmedEmail;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }

        /// <summary>
        /// Configures authentication cookie settings
        /// </summary>
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind configuration options
            var identityOptions = new OncoTrackerIdentityOptions();
            configuration.GetSection(OncoTrackerIdentityOptions.SectionName).Bind(identityOptions);

            // Configure cookie settings
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = identityOptions.Cookie.LoginPath;
                options.LogoutPath = identityOptions.Cookie.LogoutPath;
                options.AccessDeniedPath = identityOptions.Cookie.AccessDeniedPath;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(identityOptions.Cookie.ExpireTimeSpanMinutes);
                options.SlidingExpiration = identityOptions.Cookie.SlidingExpiration;
            });

            return services;
        }

        /// <summary>
        /// Adds custom authorization policies for OncoTracker roles
        /// </summary>
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            // Add authorization policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DoctorOnly", policy => policy.RequireClaim("UserType", "Doctor"));
                options.AddPolicy("PatientOnly", policy => policy.RequireClaim("UserType", "Patient"));
                options.AddPolicy("HealthcareStaff", policy => 
                    policy.RequireAssertion(context =>
                        context.User.HasClaim("UserType", "Doctor") ||
                        context.User.HasClaim("UserType", "Admin")));
            });

            return services;
        }

        /// <summary>
        /// Adds all OncoTracker-specific services in one call
        /// </summary>
        public static IServiceCollection AddOncoTrackerServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure options
            services.Configure<OncoTrackerIdentityOptions>(
                configuration.GetSection(OncoTrackerIdentityOptions.SectionName));

            services.AddIdentityServices(configuration);
            services.AddAuthenticationServices(configuration);
            services.AddAuthorizationPolicies();
            
            return services;
        }
    }
}
