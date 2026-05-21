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
        private const string ApiBaseUrl = "https://apitrainingcertification-c7geeadpdvd4gvfb.westeurope-01.azurewebsites.net/";

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

        public async Task<IActionResult> ExportPdf()
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

                var html = GenerateReportHtml(reports);
                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting PDF");
                return BadRequest("Could not generate PDF report.");
            }
        }

        private string GenerateReportHtml(ReportingViewModel reports)
        {
            var html = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Operational Reports</title>
    <script src='https://cdnjs.cloudflare.com/ajax/libs/html2pdf.js/0.10.1/html2pdf.bundle.min.js'></script>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        body {
            font-family: Arial, sans-serif;
            color: #333;
            line-height: 1.6;
            background-color: #f5f5f5;
            padding: 20px;
        }
        .container {
            background: white;
            max-width: 900px;
            margin: 0 auto;
            padding: 40px;
            border-radius: 5px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .header {
            border-bottom: 2px solid #007bff;
            padding-bottom: 20px;
            margin-bottom: 30px;
        }
        h1 {
            font-size: 28px;
            margin-bottom: 5px;
            color: #007bff;
        }
        .generated-date {
            font-size: 12px;
            color: #999;
            margin-top: 10px;
        }
        h2 {
            font-size: 18px;
            margin-top: 30px;
            margin-bottom: 15px;
            color: #007bff;
            border-bottom: 1px solid #ddd;
            padding-bottom: 10px;
        }
        .metrics-row {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 20px;
            margin-bottom: 20px;
        }
        .metric-card {
            background: #f9f9f9;
            padding: 15px;
            border-left: 4px solid #007bff;
        }
        .metric-label {
            font-size: 12px;
            color: #999;
            text-transform: uppercase;
            margin-bottom: 5px;
        }
        .metric-value {
            font-size: 24px;
            font-weight: bold;
            color: #007bff;
        }
        table {
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 20px;
        }
        thead {
            background-color: #007bff;
            color: white;
        }
        th {
            padding: 12px;
            text-align: left;
            font-weight: bold;
        }
        td {
            padding: 10px 12px;
            border-bottom: 1px solid #ddd;
        }
        tbody tr:nth-child(even) {
            background-color: #f9f9f9;
        }
        tbody tr:hover {
            background-color: #f0f0f0;
        }
        .button-group {
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #ddd;
            display: flex;
            gap: 10px;
        }
        button {
            padding: 10px 20px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
            font-weight: bold;
        }
        .btn-pdf {
            background-color: #dc3545;
            color: white;
        }
        .btn-pdf:hover {
            background-color: #c82333;
        }
        .btn-print {
            background-color: #6c757d;
            color: white;
        }
        .btn-print:hover {
            background-color: #5a6268;
        }
        @media print {
            body {
                background: white;
                padding: 0;
            }
            .container {
                box-shadow: none;
                padding: 0;
                max-width: 100%;
            }
            .button-group {
                display: none;
            }
            .metrics-row {
                page-break-inside: avoid;
            }
            table {
                page-break-inside: avoid;
            }
        }
    </style>
</head>
<body>
    <div class='container' id='pdfContent'>
        <div class='header'>
            <h1>Operational Reports</h1>
            <p class='generated-date'>Generated on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"</p>
        </div>";

            // Session Metrics
            if (reports.SessionMetrics.Any())
            {
                var metrics = reports.SessionMetrics.First();
                html += @"
        <h2>Session Metrics</h2>
        <div class='metrics-row'>
            <div class='metric-card'>
                <div class='metric-label'>Total Sessions</div>
                <div class='metric-value'>" + metrics.TotalSessions + @"</div>
            </div>
            <div class='metric-card'>
                <div class='metric-label'>Total Enrollments</div>
                <div class='metric-value'>" + metrics.TotalEnrollments + @"</div>
            </div>
            <div class='metric-card'>
                <div class='metric-label'>Avg Enrollments/Session</div>
                <div class='metric-value'>" + metrics.AverageEnrollmentsPerSession + @"</div>
            </div>
        </div>";
            }

            // Enrollment by Course
            if (reports.EnrollmentByCourse.Any())
            {
                html += @"
        <h2>Enrollment by Course</h2>
        <table>
            <thead>
                <tr>
                    <th>Course Name</th>
                    <th>Enrollment Count</th>
                </tr>
            </thead>
            <tbody>";
                foreach (var enrollment in reports.EnrollmentByCourse)
                {
                    html += $@"
                <tr>
                    <td>{enrollment.CourseName}</td>
                    <td>{enrollment.EnrollmentCount}</td>
                </tr>";
                }
                html += @"
            </tbody>
        </table>";
            }

            // Instructor Workload
            if (reports.InstructorWorkload.Any())
            {
                html += @"
        <h2>Instructor Workload</h2>
        <table>
            <thead>
                <tr>
                    <th>Instructor ID</th>
                    <th>Sessions Assigned</th>
                </tr>
            </thead>
            <tbody>";
                foreach (var instructor in reports.InstructorWorkload)
                {
                    html += $@"
                <tr>
                    <td>{instructor.InstructorId}</td>
                    <td>{instructor.SessionCount}</td>
                </tr>";
                }
                html += @"
            </tbody>
        </table>";
            }

            // Certification Rates
            if (reports.CertificationRates.Any())
            {
                var cert = reports.CertificationRates.First();
                html += @"
        <h2>Certification Completion Rates</h2>
        <div class='metrics-row'>
            <div class='metric-card'>
                <div class='metric-label'>Total Certifications</div>
                <div class='metric-value'>" + cert.TotalCertifications + @"</div>
            </div>
            <div class='metric-card'>
                <div class='metric-label'>Total Enrollments</div>
                <div class='metric-value'>" + cert.TotalEnrollments + @"</div>
            </div>
            <div class='metric-card'>
                <div class='metric-label'>Completion Rate</div>
                <div class='metric-value'>" + cert.CompletionRate + @"%</div>
            </div>
        </div>";
            }

            html += @"
        <div class='button-group'>
            <button class='btn-pdf' onclick='downloadPDF()'>Download as PDF</button>
            <button class='btn-print' onclick='window.print()'>Print</button>
        </div>
    </div>

    <script>
        function downloadPDF() {
            const element = document.getElementById('pdfContent');
            const opt = {
                margin: 10,
                filename: 'Operational_Report_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + @".pdf',
                image: { type: 'jpeg', quality: 0.98 },
                html2canvas: { scale: 2 },
                jsPDF: { orientation: 'portrait', unit: 'mm', format: 'a4' }
            };
            html2pdf().set(opt).from(element).save();
        }
    </script>
</body>
</html>";

            return html;
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
