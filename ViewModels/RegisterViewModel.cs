using System.ComponentModel.DataAnnotations;

namespace OncoTrack.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Required]
        [Display(Name = "User Type")]
        public string UserType { get; set; } // "Doctor" or "Patient"

        // Patient-specific fields
        [Display(Name = "Cancer Type")]
        public string? CancerType { get; set; }

        [Display(Name = "Stage")]
        public string? Stage { get; set; }

        [Display(Name = "Diagnosis Date")]
        [DataType(DataType.Date)]
        public DateTime? DiagnosisDate { get; set; }

        // Doctor-specific fields
        [Display(Name = "Specialization")]
        public string? Specialization { get; set; }

        [Display(Name = "License Number")]
        public string? LicenseNumber { get; set; }

        [Display(Name = "Years of Experience")]
        public int? YearsOfExperience { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}