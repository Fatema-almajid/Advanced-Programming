using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;

namespace reportingApplication.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public LoginController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public IActionResult Login()
        {
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

            try
            {
                var baseUrl = "https://apitrainingcertification-c7geeadpdvd4gvfb.westeurope-01.azurewebsites.net";
                var authApiUrl = _configuration["AuthApi:Url"] ?? $"{baseUrl}/api/Auth/login";

                var response = await _httpClient.PostAsJsonAsync(authApiUrl, new
                {
                    email = model.Email,
                    password = model.Password
                });

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                    {
                        var root = doc.RootElement;
                        
                        
                        string? token = null;
                        if (root.TryGetProperty("token", out var tokenElement))
                        {
                            token = tokenElement.GetString();
                        }

                        
                        string? userId = null;
                        if (root.TryGetProperty("userId", out var userIdElement))
                        {
                            userId = userIdElement.GetInt32().ToString();
                        }

                        
                        string? role = null;
                        if (root.TryGetProperty("role", out var roleElement))
                        {
                            role = roleElement.GetString();
                        }

                        if (string.IsNullOrEmpty(token))
                        {
                            ModelState.AddModelError(string.Empty, "Failed to retrieve authentication token.");
                            return View(model);
                        }

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, model.Email),
                            new Claim("Token", token)
                        };

                        
                        if (!string.IsNullOrEmpty(userId))
                        {
                            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
                        }

                        
                        if (!string.IsNullOrEmpty(role))
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        var claimsIdentity = new ClaimsIdentity(
                            claims,
                            CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        TempData["SuccessMessage"] = "Login successful.";
                        return RedirectToAction("Index", "Reporting");
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Could not reach the server. Check your connection.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string Email { get; set; } = "";
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = "";
    }
}
