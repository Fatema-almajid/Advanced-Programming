using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace reportingApplication.Controllers
{
    [Authorize(Roles = "TRAINING_COORDINATOR,INSTRUCTOR")]
    public class ReportingController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ReportingController> _logger;
        private const string ApiBaseUrl = "https://localhost:7102";

        public ReportingController(HttpClient httpClient, IConfiguration configuration, ILogger<ReportingController> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var reports = new ReportingViewModel();
                
                var token = User?.FindFirst("Token")?.Value ?? "";
                
                
                var courses = await FetchApiData("/api/Courses", token);
                var sessions = await FetchApiData("/api/Sessions", token);
                var payments = await FetchApiData("/api/Payments", token);
                var certifications = await FetchApiData("/api/TraineeCertifications", token);
                
                
                reports.EnrollmentByCourse = BuildEnrollmentByCourse(sessions, courses);
                reports.InstructorWorkload = BuildInstructorWorkload(sessions);
                reports.CertificationRates = BuildCertificationRates(certifications, sessions);
                reports.RevenueReport = BuildRevenueReport(payments);
                reports.SessionMetrics = BuildSessionMetrics(sessions);
                
                return View(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading reporting data");
                ViewBag.Error = "Could not load reporting data. Please ensure the API is running.";
                return View(new ReportingViewModel());
            }
        }

        private async Task<JsonElement> FetchApiData(string endpoint, string token)
        {
            try
            {
                var url = ApiBaseUrl + endpoint;
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                
                var response = await _httpClient.SendAsync(request);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonDocument.Parse(content).RootElement;
                }
                
                _logger.LogWarning($"Failed to fetch {endpoint}. Status: {response.StatusCode}");
                return JsonDocument.Parse("[]").RootElement;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception fetching {endpoint}");
                return JsonDocument.Parse("[]").RootElement;
            }
        }

        private List<EnrollmentByCourseReport> BuildEnrollmentByCourse(JsonElement sessions, JsonElement courses)
        {
            var result = new List<EnrollmentByCourseReport>();
            
            try
            {
                var sessionList = sessions.EnumerateArray().ToList();
                var courseList = courses.EnumerateArray().ToList();
                
                foreach (var course in courseList)
                {
                    if (course.TryGetProperty("id", out var courseId) && course.TryGetProperty("title", out var title))
                    {
                        int courseIdValue = courseId.GetInt32();
                        var count = sessionList.Count(s => 
                            s.TryGetProperty("courseId", out var sCourseId) && 
                            sCourseId.GetInt32() == courseIdValue);
                        result.Add(new EnrollmentByCourseReport 
                        { 
                            CourseName = title.GetString() ?? "Unknown",
                            EnrollmentCount = count
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building enrollment by course report");
            }

            return result;
        }

        private List<InstructorWorkloadReport> BuildInstructorWorkload(JsonElement sessions)
        {
            var result = new List<InstructorWorkloadReport>();
            
            try
            {
                var sessionList = sessions.EnumerateArray().ToList();
                var instructorSessions = new Dictionary<int, int>();
                
                foreach (var session in sessionList)
                {
                    if (session.TryGetProperty("instructorId", out var instructorId))
                    {
                        int instructorIdValue = instructorId.GetInt32();
                        if (!instructorSessions.ContainsKey(instructorIdValue))
                            instructorSessions[instructorIdValue] = 0;
                        instructorSessions[instructorIdValue]++;
                    }
                }

                foreach (var pair in instructorSessions)
                {
                    result.Add(new InstructorWorkloadReport 
                    { 
                        InstructorId = pair.Key,
                        SessionCount = pair.Value,
                        AssignedStudents = 0
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building instructor workload report");
            }

            return result;
        }

        private List<CertificationRateReport> BuildCertificationRates(JsonElement certifications, JsonElement sessions)
        {
            var result = new List<CertificationRateReport>();
            
            try
            {
                int totalCertifications = certifications.EnumerateArray().Count();
                int totalSessions = sessions.EnumerateArray().Count();
                
                result.Add(new CertificationRateReport
                {
                    TotalCertifications = totalCertifications,
                    TotalEnrollments = totalSessions,
                    CompletionRate = totalSessions > 0 
                        ? Math.Round((double)totalCertifications / totalSessions * 100, 2)
                        : 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building certification rates report");
            }

            return result;
        }

        private List<RevenueReport> BuildRevenueReport(JsonElement payments)
        {
            var result = new List<RevenueReport>();
            
            try
            {
                decimal totalRevenue = 0;
                int completedPayments = 0;
                int pendingPayments = 0;

                foreach (var payment in payments.EnumerateArray())
                {
                    if (payment.TryGetProperty("amount", out var amount))
                    {
                        totalRevenue += amount.GetDecimal();
                    }
                    
                    if (payment.TryGetProperty("status", out var status))
                    {
                        string statusStr = status.GetString() ?? "";
                        if (statusStr.ToLower().Contains("completed") || statusStr.ToLower().Contains("paid"))
                            completedPayments++;
                        else if (statusStr.ToLower().Contains("pending"))
                            pendingPayments++;
                    }
                }

                result.Add(new RevenueReport
                {
                    TotalRevenue = totalRevenue,
                    CompletedPayments = completedPayments,
                    PendingPayments = pendingPayments
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building revenue report");
                result.Add(new RevenueReport
                {
                    TotalRevenue = 0,
                    CompletedPayments = 0,
                    PendingPayments = 0
                });
            }

            return result;
        }

        private List<SessionMetricsReport> BuildSessionMetrics(JsonElement sessions)
        {
            var result = new List<SessionMetricsReport>();
            
            try
            {
                int activeSessions = sessions.EnumerateArray().Count();

                result.Add(new SessionMetricsReport
                {
                    TotalSessions = activeSessions,
                    TotalEnrollments = activeSessions,
                    AverageEnrollmentsPerSession = 1
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building session metrics report");
            }

            return result;
        }
    }

    
    public class ReportingViewModel
    {
        public List<EnrollmentByCourseReport> EnrollmentByCourse { get; set; } = new();
        public List<InstructorWorkloadReport> InstructorWorkload { get; set; } = new();
        public List<CertificationRateReport> CertificationRates { get; set; } = new();
        public List<RevenueReport> RevenueReport { get; set; } = new();
        public List<SessionMetricsReport> SessionMetrics { get; set; } = new();
    }

    public class EnrollmentByCourseReport
    {
        public string CourseName { get; set; } = "";
        public int EnrollmentCount { get; set; }
    }

    public class InstructorWorkloadReport
    {
        public int InstructorId { get; set; }
        public int SessionCount { get; set; }
        public int AssignedStudents { get; set; }
    }

    public class CertificationRateReport
    {
        public int TotalCertifications { get; set; }
        public int TotalEnrollments { get; set; }
        public double CompletionRate { get; set; }
    }

    public class RevenueReport
    {
        public decimal TotalRevenue { get; set; }
        public int CompletedPayments { get; set; }
        public int PendingPayments { get; set; }
    }

    public class SessionMetricsReport
    {
        public int TotalSessions { get; set; }
        public int TotalEnrollments { get; set; }
        public int AverageEnrollmentsPerSession { get; set; }
    }
}
