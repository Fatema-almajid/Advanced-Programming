// for public certification lookup

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MVC_Application.Models.ViewModels;


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
        public IActionResult Index()
        {
            return View(new CertificationLookupViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(CertificationLookupViewModel model)
        {
            var apiUrl =
            $"https://localhost:7102/api/CertificationApi/verify?cpr={model.CPR}&referenceNumber={model.ReferenceNumber}"; 
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