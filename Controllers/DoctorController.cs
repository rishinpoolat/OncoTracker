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
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.UserType != "Doctor")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var doctor = await _context.Doctors
                .Include(d => d.Patients)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(d => d.UserId == currentUser.Id);

            if (doctor == null)
            {
                return NotFound();
            }

            // Get patient IDs for this doctor
            var patientIds = doctor.Patients.Select(p => p.PatientId).ToList();

            var viewModel = new DoctorDashboardViewModel
            {
                DoctorName = $"Dr. {currentUser.FirstName} {currentUser.LastName}",
                Specialization = doctor.Specialization,
                Patients = doctor.Patients.ToList(),
                TotalPatients = doctor.Patients.Count,
                AppointmentsToday = await _context.Appointments
                    .CountAsync(a => a.DoctorId == doctor.DoctorId 
                        && a.AppointmentDate.Date == DateTime.Today 
                        && a.Status == "Approved"),
                PendingRequests = await _context.Appointments
                    .CountAsync(a => a.DoctorId == doctor.DoctorId && a.Status == "Pending")
            };

            return View(viewModel);
        }

        public async Task<IActionResult> PatientList()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var doctor = await _context.Doctors
                .Include(d => d.Patients)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(d => d.UserId == currentUser.Id);

            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor.Patients.ToList());
        }

        public async Task<IActionResult> PatientDetails(string id)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .Include(p => p.TreatmentUpdates)
                .Include(p => p.Medications)
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        [HttpGet]
        public IActionResult AddTreatmentUpdate(string patientId)
        {
            var model = new AddTreatmentUpdateViewModel
            {
                PatientId = patientId
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTreatmentUpdate(AddTreatmentUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var update = new TreatmentUpdate
                {
                    TreatmentUpdateId = Guid.NewGuid().ToString(),
                    PatientId = model.PatientId,
                    UpdateType = model.UpdateType,
                    Description = model.Description,
                    Notes = model.Notes,
                    UpdateDate = DateTime.Now,
                    CreatedBy = $"Dr. {currentUser.FirstName} {currentUser.LastName}"
                };

                _context.TreatmentUpdates.Add(update);
                await _context.SaveChangesAsync();

                return RedirectToAction("PatientDetails", new { id = model.PatientId });
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult AddMedication(string patientId)
        {
            var model = new AddMedicationViewModel
            {
                PatientId = patientId,
                StartDate = DateTime.Today
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMedication(AddMedicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var medication = new Medication
                {
                    MedicationId = Guid.NewGuid().ToString(),
                    PatientId = model.PatientId,
                    MedicationName = model.MedicationName,
                    Dosage = model.Dosage,
                    Frequency = model.Frequency,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    SideEffects = model.SideEffects,
                    PrescribedBy = $"Dr. {currentUser.FirstName} {currentUser.LastName}",
                    IsActive = true
                };

                _context.Medications.Add(medication);
                await _context.SaveChangesAsync();

                return RedirectToAction("PatientDetails", new { id = model.PatientId });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AssignPatient()
        {
            // Get all patients without an assigned doctor
            var unassignedPatients = await _context.Patients
                .Include(p => p.User)
                .Where(p => p.AssignedDoctorId == null)
                .ToListAsync();

            return View(unassignedPatients);
        }

        [HttpPost]
        public async Task<IActionResult> AssignPatientToMe(string patientId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == currentUser.Id);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == patientId);

            if (doctor != null && patient != null)
            {
                patient.AssignedDoctorId = doctor.DoctorId;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("PatientList");
        }

        [HttpGet]
        public async Task<IActionResult> AddAppointment(string patientId)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);

            if (patient == null)
            {
                return NotFound();
            }

            var model = new AddAppointmentViewModel
            {
                PatientId = patientId,
                AppointmentDate = DateTime.Today.AddDays(1).AddHours(9) // Default to tomorrow at 9 AM
            };

            ViewBag.PatientName = $"{patient.User.FirstName} {patient.User.LastName}";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAppointment(AddAppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.UserId == currentUser.Id);
                    
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.PatientId == model.PatientId);

                if (doctor != null && patient != null)
                {
                    var appointment = new Appointment
                    {
                        AppointmentId = Guid.NewGuid().ToString(),
                        PatientId = model.PatientId,
                        Patient = patient,
                        DoctorId = doctor.DoctorId,
                        Doctor = doctor,
                        AppointmentDate = model.AppointmentDate,
                        AppointmentType = model.AppointmentType,
                        Notes = model.Notes,
                        Status = "Approved", // Doctor-created appointments are auto-approved
                        DoctorName = $"Dr. {currentUser.FirstName} {currentUser.LastName}"
                    };

                    _context.Appointments.Add(appointment);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("PatientDetails", new { id = model.PatientId });
            }

            // If we got this far, something failed, redisplay form
            var patientForView = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PatientId == model.PatientId);

            if (patientForView != null)
            {
                ViewBag.PatientName = $"{patientForView.User.FirstName} {patientForView.User.LastName}";
            }

            return View(model);
        }

        public async Task<IActionResult> AppointmentRequests()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == currentUser.Id);

            if (doctor == null)
            {
                return NotFound();
            }

            var pendingAppointments = await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Where(a => a.DoctorId == doctor.DoctorId && a.Status == "Pending")
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            return View(pendingAppointments);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveAppointment(string appointmentId)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment != null)
            {
                appointment.Status = "Approved";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Appointment approved successfully.";
            }

            return RedirectToAction("AppointmentRequests");
        }

        [HttpPost]
        public async Task<IActionResult> RejectAppointment(string appointmentId)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment != null)
            {
                appointment.Status = "Rejected";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Appointment rejected.";
            }

            return RedirectToAction("AppointmentRequests");
        }

        public async Task<IActionResult> MyAppointments()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == currentUser.Id);

            if (doctor == null)
            {
                return NotFound();
            }

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Where(a => a.DoctorId == doctor.DoctorId && a.Status == "Approved")
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            return View(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> CancelAppointment(string appointmentId)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment != null)
            {
                appointment.Status = "Cancelled";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Appointment cancelled successfully.";
            }

            return RedirectToAction("MyAppointments");
        }

        [HttpGet]
        public async Task<IActionResult> EditAppointment(string id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            var model = new AddAppointmentViewModel
            {
                PatientId = appointment.PatientId,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentType = appointment.AppointmentType,
                Notes = appointment.Notes,
                Status = appointment.Status
            };

            ViewBag.PatientName = $"{appointment.Patient.User.FirstName} {appointment.Patient.User.LastName}";
            ViewBag.AppointmentId = appointment.AppointmentId;
            ViewBag.IsEdit = true;
            return View("AddAppointment", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAppointment(string appointmentId, AddAppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var appointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

                if (appointment != null)
                {
                    appointment.AppointmentDate = model.AppointmentDate;
                    appointment.AppointmentType = model.AppointmentType;
                    appointment.Notes = model.Notes;
                    appointment.Status = model.Status;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Appointment updated successfully.";
                    return RedirectToAction("MyAppointments");
                }
            }

            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PatientId == model.PatientId);

            if (patient != null)
            {
                ViewBag.PatientName = $"{patient.User.FirstName} {patient.User.LastName}";
            }

            ViewBag.AppointmentId = appointmentId;
            ViewBag.IsEdit = true;
            return View("AddAppointment", model);
        }

        [HttpGet]
        public async Task<IActionResult> EditMedication(string id)
        {
            var medication = await _context.Medications
                .Include(m => m.Patient)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(m => m.MedicationId == id);

            if (medication == null)
            {
                return NotFound();
            }

            var model = new AddMedicationViewModel
            {
                PatientId = medication.PatientId,
                MedicationName = medication.MedicationName,
                Dosage = medication.Dosage,
                Frequency = medication.Frequency,
                StartDate = medication.StartDate,
                EndDate = medication.EndDate,
                SideEffects = medication.SideEffects
            };

            ViewBag.PatientName = $"{medication.Patient.User.FirstName} {medication.Patient.User.LastName}";
            ViewBag.MedicationId = medication.MedicationId;
            ViewBag.IsEdit = true;
            return View("AddMedication", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMedication(string medicationId, AddMedicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var medication = await _context.Medications
                    .FirstOrDefaultAsync(m => m.MedicationId == medicationId);

                if (medication != null)
                {
                    medication.MedicationName = model.MedicationName;
                    medication.Dosage = model.Dosage;
                    medication.Frequency = model.Frequency;
                    medication.StartDate = model.StartDate;
                    medication.EndDate = model.EndDate;
                    medication.SideEffects = model.SideEffects;
                    medication.IsActive = model.EndDate == null || model.EndDate > DateTime.Now;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Medication updated successfully.";
                    return RedirectToAction("PatientDetails", new { id = model.PatientId });
                }
            }

            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PatientId == model.PatientId);

            if (patient != null)
            {
                ViewBag.PatientName = $"{patient.User.FirstName} {patient.User.LastName}";
            }

            ViewBag.MedicationId = medicationId;
            ViewBag.IsEdit = true;
            return View("AddMedication", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateMedication(string medicationId)
        {
            var medication = await _context.Medications
                .FirstOrDefaultAsync(m => m.MedicationId == medicationId);

            if (medication != null)
            {
                medication.IsActive = false;
                medication.EndDate = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Medication deactivated successfully.";
                return RedirectToAction("PatientDetails", new { id = medication.PatientId });
            }

            return RedirectToAction("PatientList");
        }
    }
}