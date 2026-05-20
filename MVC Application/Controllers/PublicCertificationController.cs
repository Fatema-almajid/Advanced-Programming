// for public certification lookup

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MVC_Application.Models.ViewModels;

/* 
 * This controller establishes an unauthenticated public portal for real-time third-party certification validation.
 * It serves as an integration gateway between the web application client layer and an isolated internal RESTful microservice API (`CertificationApi`),
 * executing asynchronous out-of-process HTTP communications (`HttpClient`) via parametric query payloads safely decoupled from the core dataset.
 * The endpoint implements transactional boundary defense through automated status-code checks, processes structured API data stream interactions
 * utilizing modern metadata extraction patterns via DOM-based JSON document object models (`JsonDocument`), and binds dynamic, schema-less downstream records
 * to statically-typed domain presentation entities (`CertificationLookupViewModel`) for unified front-end render operations.
 */

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