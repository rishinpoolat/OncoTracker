namespace OncoTrack.ViewModels
{
    public class PatientDashboardViewModel
    {
        public string PatientName { get; set; }
        public string CancerType { get; set; }
        public string Stage { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public string? DoctorName { get; set; }
        public List<OncoTrack.Models.TreatmentUpdate> RecentUpdates { get; set; }
        public List<OncoTrack.Models.Medication> ActiveMedications { get; set; }
        public List<OncoTrack.Models.Appointment> UpcomingAppointments { get; set; }
    }
}