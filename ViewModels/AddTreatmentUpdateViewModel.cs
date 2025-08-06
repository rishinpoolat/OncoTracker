using System.ComponentModel.DataAnnotations;

namespace OncoTrack.ViewModels
{
    public class AddTreatmentUpdateViewModel
    {
        [Required]
        public string PatientId { get; set; }
        
        [Required]
        [Display(Name = "Update Type")]
        public string UpdateType { get; set; }
        
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        
        [Display(Name = "Additional Notes")]
        public string? Notes { get; set; }
    }
}