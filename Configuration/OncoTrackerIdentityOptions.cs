namespace OncoTrack.Configuration
{
    /// <summary>
    /// Configuration options for OncoTracker Identity settings
    /// </summary>
    public class OncoTrackerIdentityOptions
    {
        public const string SectionName = "OncoTrackerIdentity";

        /// <summary>
        /// Password requirements configuration
        /// </summary>
        public PasswordOptions Password { get; set; } = new();

        /// <summary>
        /// User account settings
        /// </summary>
        public UserOptions User { get; set; } = new();

        /// <summary>
        /// Sign-in behavior settings
        /// </summary>
        public SignInOptions SignIn { get; set; } = new();

        /// <summary>
        /// Cookie authentication settings
        /// </summary>
        public CookieOptions Cookie { get; set; } = new();
    }

    public class PasswordOptions
    {
        public bool RequireDigit { get; set; } = true;
        public int RequiredLength { get; set; } = 6;
        public bool RequireNonAlphanumeric { get; set; } = false;
        public bool RequireUppercase { get; set; } = true;
        public bool RequireLowercase { get; set; } = true;
    }

    public class UserOptions
    {
        public bool RequireUniqueEmail { get; set; } = true;
    }

    public class SignInOptions
    {
        public bool RequireConfirmedEmail { get; set; } = false;
    }

    public class CookieOptions
    {
        public string LoginPath { get; set; } = "/Account/Login";
        public string LogoutPath { get; set; } = "/Account/Logout";
        public string AccessDeniedPath { get; set; } = "/Account/AccessDenied";
        public int ExpireTimeSpanMinutes { get; set; } = 60;
        public bool SlidingExpiration { get; set; } = true;
    }
}
