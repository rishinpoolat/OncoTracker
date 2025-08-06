using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OncoTrack.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Specialization { get; set; }
        public string LicenseNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public List<Patient> Patients { get; set; }
    }
}