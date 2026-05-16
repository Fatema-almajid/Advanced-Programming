// for public certification lookup

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MVC_Application.Models.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
            $"https://localhost:7102/api/CertificationApi/verify?traineeId={model.TraineeId}&referenceNumber={model.ReferenceNumber}";
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

            return View(model);
        }
        [HttpPost]
        public IActionResult DownloadPdf(CertificationLookupViewModel model)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);

                    page.Header()
                        .Text("Training Certification")
                        .FontSize(24)
                        .Bold()
                        .AlignCenter();

                    page.Content()
                        .PaddingVertical(20)
                        .Column(col =>
                        {
                            col.Item().Text($"Trainee: {model.TraineeName}");
                            col.Item().Text($"Track: {model.Track}");
                            col.Item().Text($"Status: {model.Status}");
                            col.Item().Text($"Reference #: {model.ReferenceNumber}");

                            col.Item().PaddingTop(20)
                                .Text("Completed Courses:")
                                .Bold();

                            foreach (var course in model.CompletedCourses)
                            {
                                col.Item().Text($"• {course}");
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generated on ");
                            x.Span(DateTime.Now.ToString("dd MMM yyyy"));
                        });
                });
            });

            var pdfBytes = pdf.GeneratePdf();

            return File(
                pdfBytes,
                "application/pdf",
                "certificate.pdf");
        }
    }
}