using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OncoTrack.Models
{
    public class Medication
    {
        public int MedicationId { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SideEffects { get; set; }
        public string PrescribedBy { get; set; }
        public bool IsActive { get; set; }
    }
}