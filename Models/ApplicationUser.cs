using Microsoft.AspNetCore.Identity;

namespace OncoTrack.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserType { get; set; } // "Doctor" or "Patient"
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}