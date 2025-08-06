using System.ComponentModel.DataAnnotations;

namespace OncoTrack.ViewModels
{
    public class AddMedicationViewModel
    {
        [Required]
        public string PatientId { get; set; }
        
        [Required]
        [Display(Name = "Medication Name")]
        public string MedicationName { get; set; }
        
        [Required]
        [Display(Name = "Dosage")]
        public string Dosage { get; set; }
        
        [Required]
        [Display(Name = "Frequency")]
        public string Frequency { get; set; }
        
        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        
        [Display(Name = "Side Effects")]
        public string? SideEffects { get; set; }
    }
}