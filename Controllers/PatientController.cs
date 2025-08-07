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
                    .Where(a => a.AppointmentDate >= DateTime.Now && a.Status == "Approved")
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

        [HttpGet]
        public async Task<IActionResult> BookAppointment()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var patient = await _context.Patients
                .Include(p => p.AssignedDoctor)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(p => p.UserId == currentUser.Id);

            if (patient?.AssignedDoctor == null)
            {
                TempData["Error"] = "No doctor assigned to your account. Please contact support.";
                return RedirectToAction("Index");
            }

            var viewModel = new BookAppointmentViewModel
            {
                DoctorId = patient.AssignedDoctor.DoctorId,
                DoctorName = $"Dr. {patient.AssignedDoctor.User.FirstName} {patient.AssignedDoctor.User.LastName}",
                AppointmentDate = DateTime.Today.AddDays(1),
                AvailableTimeSlots = GetAvailableTimeSlots()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> BookAppointment(BookAppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableTimeSlots = GetAvailableTimeSlots();
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var patient = await _context.Patients
                .Include(p => p.AssignedDoctor)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(p => p.UserId == currentUser.Id);

            if (patient?.AssignedDoctor == null)
            {
                TempData["Error"] = "No doctor assigned to your account.";
                return RedirectToAction("Index");
            }

            var appointmentDateTime = model.AppointmentDate.Date + model.AppointmentTime;

            if (appointmentDateTime <= DateTime.Now)
            {
                ModelState.AddModelError("", "Appointment date and time must be in the future.");
                model.AvailableTimeSlots = GetAvailableTimeSlots();
                return View(model);
            }

            if (!IsTimeSlotAvailable(appointmentDateTime, model.DoctorId))
            {
                ModelState.AddModelError("", "Selected time slot is not available. Please choose another time.");
                model.AvailableTimeSlots = GetAvailableTimeSlots();
                return View(model);
            }

            var appointment = new Appointment
            {
                AppointmentId = Guid.NewGuid().ToString(),
                PatientId = patient.PatientId,
                Patient = patient,
                DoctorId = model.DoctorId,
                Doctor = patient.AssignedDoctor,
                AppointmentDate = appointmentDateTime,
                AppointmentType = model.AppointmentType,
                Notes = model.Notes,
                Status = "Pending",
                DoctorName = model.DoctorName
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Appointment request submitted successfully. Your doctor will review and respond soon.";
            return RedirectToAction("Appointments");
        }

        private List<TimeSpan> GetAvailableTimeSlots()
        {
            var slots = new List<TimeSpan>();
            for (int hour = 10; hour < 18; hour++)
            {
                slots.Add(new TimeSpan(hour, 0, 0));
                slots.Add(new TimeSpan(hour, 30, 0));
            }
            return slots;
        }

        private bool IsTimeSlotAvailable(DateTime appointmentDateTime, string doctorId)
        {
            if (appointmentDateTime.Hour < 10 || appointmentDateTime.Hour >= 18)
                return false;

            var existingAppointment = _context.Appointments
                .Any(a => a.DoctorId == doctorId && 
                         a.AppointmentDate == appointmentDateTime && 
                         (a.Status == "Approved" || a.Status == "Pending"));

            return !existingAppointment;
        }
    }
}