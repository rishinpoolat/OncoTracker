using System.ComponentModel.DataAnnotations;

namespace OncoTrack.ViewModels
{
    public class BookAppointmentViewModel
    {
        [Required]
        [Display(Name = "Appointment Date")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }
        
        [Required]
        [Display(Name = "Appointment Time")]
        [DataType(DataType.Time)]
        public TimeSpan AppointmentTime { get; set; }
        
        [Required]
        [Display(Name = "Appointment Type")]
        public string AppointmentType { get; set; }
        
        [Display(Name = "Notes")]
        [StringLength(500, ErrorMessage = "Notes cannot be longer than 500 characters.")]
        public string? Notes { get; set; }
        
        public string DoctorName { get; set; }
        public string DoctorId { get; set; }
        
        public List<string> AppointmentTypes { get; set; } = new List<string>
        {
            "Consultation",
            "Follow-up",
            "Chemotherapy",
            "Scan",
            "Blood Test",
            "Other"
        };
        
        public List<TimeSpan> AvailableTimeSlots { get; set; } = new List<TimeSpan>();
    }
}