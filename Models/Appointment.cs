using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OncoTrack.Models
{
    public class Appointment
    {
        public required string AppointmentId { get; set; }
        public required string PatientId { get; set; }
        public required Patient Patient { get; set; }
        public DateTime AppointmentDate { get; set; }
        public required string AppointmentType { get; set; } // "Consultation", "Chemotherapy", "Scan", etc.
        public string? Notes { get; set; }
        public required string Status { get; set; } // "Pending", "Approved", "Rejected", "Completed", "Cancelled"
        public required string DoctorName { get; set; }
        public required string DoctorId { get; set; }
        public required Doctor Doctor { get; set; }
    }
}