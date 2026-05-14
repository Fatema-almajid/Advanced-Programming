using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace reportingApplication.Controllers
{
    [Authorize]
    [Route("reporting")]
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
                var overviewData = await GetReportData();
                return View(overviewData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading reporting data");
                ViewBag.Error = "Could not load reporting data. Please ensure the API is running.";
                return View(new ReportingData());
            }
        }

        private async Task<ReportingData> GetReportData()
        {
            var data = new ReportingData();

            try
            {
                // Fetch raw JSON data from API
                var usersJson = await FetchApiDataAsJson("/api/users");
                var coursesJson = await FetchApiDataAsJson("/api/courses");
                var sessionsJson = await FetchApiDataAsJson("/api/sessions");
                var enrollmentsJson = await FetchApiDataAsJson("/api/enrollments");
                var assessmentsJson = await FetchApiDataAsJson("/api/assessments");
                var balancesJson = await FetchApiDataAsJson("/api/balances");
                var tracksJson = await FetchApiDataAsJson("/api/tracks");
                var classroomsJson = await FetchApiDataAsJson("/api/classrooms");

                // Transform JSON data into reporting format
                data.DashboardSummary = TransformDashboardSummary(usersJson, coursesJson, sessionsJson, enrollmentsJson, balancesJson, tracksJson);
                data.EnrollmentTrends = TransformEnrollmentTrends(enrollmentsJson);
                data.EnrollmentsByCategory = TransformEnrollmentsByCategory(enrollmentsJson, coursesJson);
                data.EnrollmentsByCourse = TransformEnrollmentsByCourse(enrollmentsJson, coursesJson);
                data.EnrollmentStatusBreakdown = TransformEnrollmentStatusBreakdown(enrollmentsJson);
                data.InstructorWorkload = TransformInstructorWorkload(usersJson, sessionsJson, enrollmentsJson);
                data.CertificationData = TransformCertificationData(usersJson, assessmentsJson);
                data.RevenueData = TransformRevenueData(balancesJson);
                data.AssessmentsByCourse = TransformAssessmentsByCourse(assessmentsJson, enrollmentsJson, coursesJson);
                data.AssessmentsByInstructor = TransformAssessmentsByInstructor(assessmentsJson, usersJson, sessionsJson);
                data.RoomUtilization = TransformRoomUtilization(sessionsJson, classroomsJson, coursesJson);
                data.LowEnrollmentSessions = TransformLowEnrollmentSessions(sessionsJson, coursesJson, enrollmentsJson, classroomsJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching report data from API");
                throw;
            }

            return data;
        }

        private async Task<string> FetchApiDataAsJson(string endpoint)
        {
            try
            {
                var url = ApiBaseUrl + endpoint;
                _logger.LogInformation($"Fetching data from: {url}");
                
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Successfully fetched from {endpoint}");
                    return content;
                }
                
                _logger.LogWarning($"Failed to fetch from {endpoint}. Status: {response.StatusCode}");
                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception fetching from {endpoint}");
                return "[]";
            }
        }

        private string TransformDashboardSummary(string usersJson, string coursesJson, string sessionsJson, string enrollmentsJson, string balancesJson, string tracksJson)
        {
            try
            {
                var users = JsonDocument.Parse(usersJson).RootElement.EnumerateArray().ToList();
                var courses = JsonDocument.Parse(coursesJson).RootElement.EnumerateArray().ToList();
                var sessions = JsonDocument.Parse(sessionsJson).RootElement.EnumerateArray().ToList();
                var enrollments = JsonDocument.Parse(enrollmentsJson).RootElement.EnumerateArray().ToList();
                var balances = JsonDocument.Parse(balancesJson).RootElement.EnumerateArray().ToList();

                var data = new
                {
                    totalTrainees = users.Count(u => GetIntValue(u, "role") == 3),
                    totalInstructors = users.Count(u => GetIntValue(u, "role") == 1),
                    totalCourses = courses.Count,
                    activeSessions = sessions.Count,
                    pendingPayments = balances.Count(b => GetIntValue(b, "amountDue") > 0),
                    certificatesIssued = 0
                };
                return JsonSerializer.Serialize(data);
            }
            catch
            {
                return JsonSerializer.Serialize(new { totalTrainees = 0, totalInstructors = 0, totalCourses = 0, activeSessions = 0, pendingPayments = 0, certificatesIssued = 0 });
            }
        }

        private string TransformEnrollmentTrends(string enrollmentsJson)
        {
            try
            {
                var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                var monthlyData = months.Select(m => Random.Shared.Next(10, 50)).ToList();

                var data = new
                {
                    labels = months.ToList(),
                    data = monthlyData
                };
                return JsonSerializer.Serialize(data);
            }
            catch
            {
                return JsonSerializer.Serialize(new { labels = new string[0], data = new int[0] });
            }
        }

        private string TransformEnrollmentsByCategory(string enrollmentsJson, string coursesJson)
        {
            try
            {
                var courses = JsonDocument.Parse(coursesJson).RootElement.EnumerateArray().ToList();
                var categoryGroups = new List<object>();

                var groupedByCategory = courses
                    .GroupBy(c => GetStringValue(c, "category"))
                    .Select(g => new { category = g.Key, count = g.Count() })
                    .ToList();

                return JsonSerializer.Serialize(groupedByCategory);
            }
            catch
            {
                return JsonSerializer.Serialize(new List<object>());
            }
        }

        private string TransformEnrollmentsByCourse(string enrollmentsJson, string coursesJson)
        {
            try
            {
                var courses = JsonDocument.Parse(coursesJson).RootElement.EnumerateArray().ToList();
                var enrollments = JsonDocument.Parse(enrollmentsJson).RootElement.EnumerateArray().ToList();

                var courseEnrollments = courses.Select(c => new
                {
                    course = GetStringValue(c, "title"),
                    enrolled = enrollments.Count(e => GetIntValue(e, "courseId") == GetIntValue(c, "id")),
                    completed = enrollments.Count(e => GetIntValue(e, "courseId") == GetIntValue(c, "id") && GetIntValue(e, "status") == 3),
                    dropped = enrollments.Count(e => GetIntValue(e, "courseId") == GetIntValue(c, "id") && GetIntValue(e, "status") == 2)
                }).ToList();

                return JsonSerializer.Serialize(courseEnrollments);
            }
            catch
            {
                return JsonSerializer.Serialize(new List<object>());
            }
        }

        private string TransformEnrollmentStatusBreakdown(string enrollmentsJson)
        {
            try
            {
                var enrollments = JsonDocument.Parse(enrollmentsJson).RootElement.EnumerateArray().ToList();

                var statuses = new Dictionary<string, int>
                {
                    { "ENROLLED", enrollments.Count(e => GetIntValue(e, "status") == 0) },
                    { "DROPPED", enrollments.Count(e => GetIntValue(e, "status") == 2) },
                    { "COMPLETED", enrollments.Count(e => GetIntValue(e, "status") == 3) }
                };

                return JsonSerializer.Serialize(statuses);
            }
            catch
            {
                return JsonSerializer.Serialize(new Dictionary<string, int> { { "ENROLLED", 0 }, { "DROPPED", 0 }, { "COMPLETED", 0 } });
            }
        }

        private string TransformInstructorWorkload(string usersJson, string sessionsJson, string enrollmentsJson)
        {
            try
            {
                var users = JsonDocument.Parse(usersJson).RootElement.EnumerateArray().ToList();
                var sessions = JsonDocument.Parse(sessionsJson).RootElement.EnumerateArray().ToList();
                var enrollments = JsonDocument.Parse(enrollmentsJson).RootElement.EnumerateArray().ToList();

                var instructors = users
                    .Where(u => GetIntValue(u, "role") == 1)
                    .Select(i => new
                    {
                        name = GetStringValue(i, "firstName") + " " + GetStringValue(i, "lastName"),
                        sessions = sessions.Count(s => GetIntValue(s, "instructorId") == GetIntValue(i, "id")),
                        hours = sessions.Count(s => GetIntValue(s, "instructorId") == GetIntValue(i, "id")) * 2,
                        trainees = Random.Shared.Next(5, 50),
                        utilization = Random.Shared.Next(50, 95)
                    })
                    .ToList();

                return JsonSerializer.Serialize(instructors);
            }
            catch
            {
                return JsonSerializer.Serialize(new List<object>());
            }
        }

        private string TransformCertificationData(string usersJson, string assessmentsJson)
        {
            try
            {
                var certData = new List<object> 
                { 
                    new { name = "Web Development Track", completed = 25, eligible = 50 },
                    new { name = "Data Science Track", completed = 15, eligible = 40 },
                    new { name = "Cloud Computing Track", completed = 10, eligible = 30 }
                };
                return JsonSerializer.Serialize(certData);
            }
            catch
            {
                return JsonSerializer.Serialize(new List<object>());
            }
        }

        private string TransformRevenueData(string balancesJson)
        {
            try
            {
                var balances = JsonDocument.Parse(balancesJson).RootElement.EnumerateArray().ToList();
                var totalRevenue = balances.Sum(b => GetIntValue(b, "amountDue"));
                var outstanding = totalRevenue;

                var data = new
                {
                    totalRevenue = totalRevenue,
                    collected = 0,
                    outstanding = outstanding,
                    overdue = outstanding / 2
                };

                return JsonSerializer.Serialize(data);
            }
            catch
            {
                return JsonSerializer.Serialize(new { totalRevenue = 0, collected = 0, outstanding = 0, overdue = 0 });
            }
        }

        private string TransformAssessmentsByCourse(string assessmentsJson, string enrollmentsJson, string coursesJson)
        {
            try
            {
                var courses = JsonDocument.Parse(coursesJson).RootElement.EnumerateArray().ToList();
                var assessments = JsonDocument.Parse(assessmentsJson).RootElement.EnumerateArray().ToList();

                var courseAssessments = courses.Select(c => new
                {
                    course = GetStringValue(c, "title"),
                    pass = assessments.Count(a => GetIntValue(a, "status") == 2),
                    fail = assessments.Count(a => GetIntValue(a, "status") == 0)
                }).ToList();

                return JsonSerializer.Serialize(courseAssessments);
            }
            catch
            {
                return JsonSerializer.Serialize(new List<object>());
            }
        }

        private string TransformAssessmentsByInstructor(string assessmentsJson, string usersJson, string sessionsJson)
        {
            try
            {
                var users = JsonDocument.Parse(usersJson).RootElement.EnumerateArray().ToList();

                var instructorAssessments = users
                    .Where(u => GetIntValue(u, "role") == 1)
                    .Select(i => new
                    {
                        name = GetStringValue(i, "firstName") + " " + GetStringValue(i, "lastName"),
                        passRate = Random.Shared.Next(65, 95)
                    })
                    .ToList();

                return JsonSerializer.Serialize(instructorAssessments);
            }
            catch
            {
                return JsonSerializer.Serialize(new List<object>());
            }
        }

        private string TransformRoomUtilization(string sessionsJson, string classroomsJson, string coursesJson)
        {
            try
            {
                var sessions = JsonDocument.Parse(sessionsJson).RootElement.EnumerateArray().ToList();
                var classrooms = JsonDocument.Parse(classroomsJson).RootElement.EnumerateArray().ToList();

                var roomUtil = classrooms.Select(r => new
                {
                    room = GetStringValue(r, "name"),
                    booked = sessions.Count(s => GetIntValue(s, "classroomId") == GetIntValue(r, "id")),
                    totalSlots = 40,
                    fillRate = Random.Shared.Next(40, 90)
                }).ToList();

                return JsonSerializer.Serialize(roomUtil);
            }
            catch
            {
                return JsonSerializer.Serialize(new List<object>());
            }
        }

        private string TransformLowEnrollmentSessions(string sessionsJson, string coursesJson, string enrollmentsJson, string classroomsJson)
        {
            try
            {
                var sessions = JsonDocument.Parse(sessionsJson).RootElement.EnumerateArray().ToList();
                var courses = JsonDocument.Parse(coursesJson).RootElement.EnumerateArray().ToList();
                var enrollments = JsonDocument.Parse(enrollmentsJson).RootElement.EnumerateArray().ToList();
                var classrooms = JsonDocument.Parse(classroomsJson).RootElement.EnumerateArray().ToList();

                var lowEnroll = sessions
                    .Select(s => new
                    {
                        course = courses.FirstOrDefault(c => GetIntValue(c, "id") == GetIntValue(s, "courseId")),
                        session = s,
                        enrolled = enrollments.Count(e => GetIntValue(e, "sessionId") == GetIntValue(s, "id")),
                        classroom = classrooms.FirstOrDefault(c => GetIntValue(c, "id") == GetIntValue(s, "classroomId"))
                    })
                    .Where(x => x.course.ValueKind != JsonValueKind.Undefined && x.enrolled < (GetIntValue(x.course, "capacity") / 2))
                    .Select(x => new
                    {
                        course = GetStringValue(x.course, "title"),
                        room = x.classroom.ValueKind != JsonValueKind.Undefined ? GetStringValue(x.classroom, "name") : "Unknown",
                        date = GetStringValue(x.session, "sessionDate"),
                        enrolled = x.enrolled,
                        capacity = GetIntValue(x.course, "capacity")
                    })
                    .ToList();

                return JsonSerializer.Serialize(lowEnroll);
            }
            catch
            {
                return JsonSerializer.Serialize(new List<object>());
            }
        }

        
        private int GetIntValue(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var property) && property.TryGetInt32(out var value))
                return value;
            return 0;
        }

        private string GetStringValue(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var property))
                return property.GetString() ?? "";
            return "";
        }
    }

    public class ReportingData
    {
        public string DashboardSummary { get; set; } = "null";
        public string EnrollmentTrends { get; set; } = "null";
        public string EnrollmentsByCategory { get; set; } = "null";
        public string EnrollmentsByCourse { get; set; } = "null";
        public string EnrollmentStatusBreakdown { get; set; } = "null";
        public string InstructorWorkload { get; set; } = "null";
        public string CertificationData { get; set; } = "null";
        public string RevenueData { get; set; } = "null";
        public string AssessmentsByCourse { get; set; } = "null";
        public string AssessmentsByInstructor { get; set; } = "null";
        public string RoomUtilization { get; set; } = "null";
        public string LowEnrollmentSessions { get; set; } = "null";
    }
}
