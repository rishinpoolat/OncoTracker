using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OncoTrack.Models
{
    public class TreatmentUpdate
    {
        public int TreatmentUpdateId { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public string UpdateType { get; set; } // "Finding", "Progress", "Side Effect", etc.
        public string Description { get; set; }
        public string? Notes { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreatedBy { get; set; }
    }
}