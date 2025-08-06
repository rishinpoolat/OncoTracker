using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OncoTrack.Data;
using OncoTrack.Models;
using OncoTrack.ViewModels;

namespace OncoTrack.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.UserType != "Patient")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var patient = await _context.Patients
                .Include(p => p.AssignedDoctor)
                    .ThenInclude(d => d.User)
                .Include(p => p.TreatmentUpdates)
                .Include(p => p.Medications)
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(p => p.UserId == currentUser.Id);

            if (patient == null)
            {
                return NotFound();
            }

            var viewModel = new PatientDashboardViewModel
            {
                PatientName = $"{currentUser.FirstName} {currentUser.LastName}",
                CancerType = patient.CancerType,
                Stage = patient.Stage,
                DiagnosisDate = patient.DiagnosisDate,
                DoctorName = patient.AssignedDoctor?.User != null ? 
                    $"Dr. {patient.AssignedDoctor.User.FirstName} {patient.AssignedDoctor.User.LastName}" : 
                    "Not Assigned",
                RecentUpdates = patient.TreatmentUpdates
                    .OrderByDescending(u => u.UpdateDate)
                    .Take(5)
                    .ToList(),
                ActiveMedications = patient.Medications
                    .Where(m => m.IsActive)
                    .ToList(),
                UpcomingAppointments = patient.Appointments
                    .Where(a => a.AppointmentDate >= DateTime.Now && a.Status == "Scheduled")
                    .OrderBy(a => a.AppointmentDate)
                    .Take(5)
                    .ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> TreatmentHistory()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var patient = await _context.Patients
                .Include(p => p.TreatmentUpdates)
                .FirstOrDefaultAsync(p => p.UserId == currentUser.Id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient.TreatmentUpdates.OrderByDescending(u => u.UpdateDate).ToList());
        }

        public async Task<IActionResult> Medications()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var patient = await _context.Patients
                .Include(p => p.Medications)
                .FirstOrDefaultAsync(p => p.UserId == currentUser.Id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient.Medications.OrderByDescending(m => m.StartDate).ToList());
        }

        public async Task<IActionResult> Appointments()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var patient = await _context.Patients
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(p => p.UserId == currentUser.Id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient.Appointments.OrderBy(a => a.AppointmentDate).ToList());
        }
    }
}