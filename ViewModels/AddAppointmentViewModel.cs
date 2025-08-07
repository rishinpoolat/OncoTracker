using System.ComponentModel.DataAnnotations;

namespace OncoTrack.ViewModels
{
    public class AddAppointmentViewModel
    {
        [Required]
        public string PatientId { get; set; }
        
        [Required]
        [Display(Name = "Appointment Date")]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }
        
        [Required]
        [Display(Name = "Appointment Type")]
        public string AppointmentType { get; set; }
        
        [Display(Name = "Notes")]
        public string? Notes { get; set; }
        
        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Scheduled";
    }
}