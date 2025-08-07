namespace OncoTrack.ViewModels
{
    public class DoctorDashboardViewModel
    {
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public List<OncoTrack.Models.Patient> Patients { get; set; }
        public int TotalPatients { get; set; }
        public int AppointmentsToday { get; set; }
        public int PendingRequests { get; set; }
    }
}