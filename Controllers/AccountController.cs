using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OncoTrack.Data;
using OncoTrack.Models;
using OncoTrack.ViewModels;
using System.Security.Claims;

namespace OncoTrack.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user.UserType == "Doctor")
                    {
                        return RedirectToAction("Index", "Doctor");
                    }
                    else if (user.UserType == "Patient")
                    {
                        return RedirectToAction("Index", "Patient");
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    Address = model.Address,
                    UserType = model.UserType,
                    CreatedAt = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Add user to role
                    await _userManager.AddToRoleAsync(user, model.UserType);
                    
                    // Add claims
                    await _userManager.AddClaimAsync(user, new Claim("UserType", model.UserType));

                    // Create Patient or Doctor record
                    if (model.UserType == "Patient")
                    {
                        var patient = new Patient
                        {
                            UserId = user.Id,
                            CancerType = model.CancerType ?? "",
                            Stage = model.Stage ?? "",
                            DiagnosisDate = model.DiagnosisDate ?? DateTime.Now
                        };
                        _context.Patients.Add(patient);
                    }
                    else if (model.UserType == "Doctor")
                    {
                        var doctor = new Doctor
                        {
                            UserId = user.Id,
                            Specialization = model.Specialization ?? "",
                            LicenseNumber = model.LicenseNumber ?? "",
                            YearsOfExperience = model.YearsOfExperience ?? 0
                        };
                        _context.Doctors.Add(doctor);
                    }

                    await _context.SaveChangesAsync();
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    if (model.UserType == "Doctor")
                    {
                        return RedirectToAction("Index", "Doctor");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Patient");
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}