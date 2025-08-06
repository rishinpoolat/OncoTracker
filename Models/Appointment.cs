using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OncoTrack.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentType { get; set; } // "Consultation", "Chemotherapy", "Scan", etc.
        public string? Notes { get; set; }
        public string Status { get; set; } // "Scheduled", "Completed", "Cancelled"
        public string DoctorName { get; set; }
    }
}