// for public certification lookup

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MVC_Application.Models.ViewModels;

/// Handles public-facing certification verification requests by querying the external training API.

namespace MVC_Application.Controllers
{
    public class PublicCertificationController : Controller
    {
        private readonly HttpClient _httpClient;

        public PublicCertificationController()
        {
            _httpClient = new HttpClient();
        }

        [HttpGet]
        public IActionResult Index(string? referenceNumber)
        {
            return View(new CertificationLookupViewModel
            {
                ReferenceNumber = referenceNumber
            });
        }

        [HttpPost]
        public async Task<IActionResult> Index(CertificationLookupViewModel model)
        {
            var apiUrl =
                $"https://apitrainingcertification-c7geeadpdvd4gvfb.westeurope-01.azurewebsites.net/api/CertificationApi/verify?cpr={model.CPR}&referenceNumber={model.ReferenceNumber}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Certification not found.";
                return View(model);
            }

            var json = await response.Content.ReadAsStringAsync();

            var data = JsonDocument.Parse(json);

            model.TraineeName =
                data.RootElement.GetProperty("traineeName").GetString();

            model.Track =
                data.RootElement.GetProperty("track").GetString();

            model.Status =
                data.RootElement.GetProperty("status").GetString();

            model.CompletedCourses =
                data.RootElement
                    .GetProperty("completedCourses")
                    .EnumerateArray()
                    .Select(x => x.GetString()!)
                    .ToList();

            model.CertificateReference =
    data.RootElement.GetProperty("certificateReference").GetString();

            model.TotalCompletedCourses =
                data.RootElement.GetProperty("totalCompletedCourses").GetInt32();

            model.VerificationDate =
                data.RootElement.GetProperty("verificationDate").GetDateTime();

            return View(model);
        }
    }
}