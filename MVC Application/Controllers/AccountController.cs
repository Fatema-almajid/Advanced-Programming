using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Application.Models.ViewModels;
using TrainingCertificationPlatform;
using TrainingCertificationPlatform.Models;

/* 
 * This controller handles user identity and access management using ASP.NET Core Cookie Authentication.
 * It manages the secure registration of trainees (validating unique constraints like Email, Phone, and CPR),
 * performs user login using BCrypt for secure password hashing and verification, establishes user claims 
 * for Role-Based Access Control (RBAC), and provides dynamic navigation routing (RedirectByRole) based on 
 * authorized user roles (Trainee, Instructor, and Training Coordinator).
 */

namespace MVC_Application.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(RedirectByRole));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            TempData["SuccessMessage"] = "Login successful.";
            return RedirectToAction(nameof(RedirectByRole));
        }

        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(RedirectByRole));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var emailExists = await _context.Users.AnyAsync(u => u.Email == model.Email);
            if (emailExists)
            {
                ModelState.AddModelError(nameof(model.Email), "This email address is already registered.");
            }

            var phoneExists = await _context.Users.AnyAsync(u => u.Phone == model.Phone);
            if (phoneExists)
            {
                ModelState.AddModelError(nameof(model.Phone), "This phone number is already registered.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var cprExists = await _context.Users.AnyAsync(u => u.CPR == model.CPR);

            if (cprExists)
            {
                ModelState.AddModelError(nameof(model.CPR),
                    "This CPR is already registered.");
            }

            var user = new User
            {
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                Email = model.Email.Trim(),
                Phone = model.Phone.Trim(),
                CPR = model.CPR.Trim(),
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                RegistrationDate = DateTime.Today,
                Role = UserRole.TRAINEE
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful. Please log in.";
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "You have been logged out.";
            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult RedirectByRole()
        {
            if (User.IsInRole(UserRole.TRAINING_COORDINATOR.ToString()))
            {
                return RedirectToAction("Dashboard", "TrainingCoordinator");
            }

            if (User.IsInRole(UserRole.INSTRUCTOR.ToString()))
            {
                return RedirectToAction("Dashboard", "Instructor");
            }

            if (User.IsInRole(UserRole.TRAINEE.ToString()))
            {
                return RedirectToAction("Dashboard", "Trainee");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}