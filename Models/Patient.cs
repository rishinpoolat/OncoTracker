using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OncoTrack.Models
{
    public class Patient
    {
        public string PatientId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string CancerType { get; set; }
        public string Stage { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public string? AssignedDoctorId { get; set; }
        public Doctor? AssignedDoctor { get; set; }
        public List<TreatmentUpdate> TreatmentUpdates { get; set; }
        public List<Medication> Medications { get; set; }
        public List<Appointment> Appointments { get; set; }
    }
}