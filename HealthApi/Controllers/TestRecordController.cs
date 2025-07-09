//using HealthApi.DTO;
//using HealthApi.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Net.Http;
//using System.Security.Claims;
//using System.Text.Json;

//namespace HealthApi.Controllers
//{
//    public class TestRecordController : Controller
//    {
//        private readonly AppDbContext context;
//        private readonly HttpClient _httpClient;

//        public TestRecordController(AppDbContext context, HttpClient httpClient)
//        {
//            this.context = context;
//            _httpClient = httpClient;
//        }


//        [HttpGet("api/TestRecords/UserRecords")]
//        public IActionResult GetUserTestRecords()
//        {
//            //var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier); --------------------------------- change later 
//            var userIdString = "5";

//            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
//            {
//                return Unauthorized("User not authenticated.");
//            }

//            var testRecordsDetailsList = new List<TestRecordDetailsDTO>();
//            var testRecords = context.TestRecords.Where(t => t.UserId == userId).ToList();

//            foreach (var testRecord in testRecords)
//            {
//                var testRecordDetails = new TestRecordDetailsDTO();

//                testRecordDetails.Result = testRecord.Result;
//                testRecordDetails.TestDetails = testRecord.Details;
//                testRecordDetails.TestId = testRecord.RecordId;
//                testRecordDetails.LabName = context.Labs.FirstOrDefault(l => l.LabId == testRecord.LabId).LabName;
//                testRecordDetails.TestTypeName = context.TestTypes.FirstOrDefault(t => t.TestId == testRecord.TestTypeId).TestName;
//                testRecordDetails.DateAdministered = testRecord.DateAdministered;
//                testRecordDetails.ContactInfo = context.Labs.FirstOrDefault(l => l.LabId == testRecord.LabId).ContactInfo;

//                testRecordsDetailsList.Add(testRecordDetails);
//            }



//            return Ok(testRecordsDetailsList);
//        }

//        [HttpGet("api/TestRecords/AllRecords")]
//        public IActionResult GetAllTestRecords([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
//        {
//            var totalCount = context.TestRecords.Count();
//            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

//            var testRecords = context.TestRecords
//                .Include(t => t.User)
//                .Include(t => t.TestType)
//                .Include(t => t.Lab)
//                .OrderByDescending(t => t.DateAdministered)
//                .Skip((pageNumber - 1) * pageSize)
//                .Take(pageSize)
//                .ToList();

//            var testRecordsDetailsList = testRecords.Select(testRecord => new AllTestRecordsDTO
//            {
//                TestId = testRecord.RecordId,
//                PatientName = testRecord.User.FirstName + " " + testRecord.User.LastName,
//                TestTypeName = testRecord.TestType.TestName,
//                LabName = testRecord.Lab.LabName,
//                PatientGender = testRecord.User.Sex,
//                PatientEmail = testRecord.User.Email,
//                PatientPhone = testRecord.User.PhoneNumber,
//                AreaName = "Location", // or real one if needed
//                Result = testRecord.Result,
//                TestDate = testRecord.DateAdministered,
//                LabContactInfo = testRecord.Lab.ContactInfo,
//                TestDetails = testRecord.Details
//            }).ToList();

//            return Ok(new
//            {
//                records = testRecordsDetailsList,
//                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
//            });
//        }




//    }
//}


using HealthApi.DTO;
using HealthApi.Models;
using HealthApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class TestRecordsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LocationAnalysisService _locationAnalysisService;

        public TestRecordsController(AppDbContext context, LocationAnalysisService locationAnalysisService)
        {
            _context = context;
            _locationAnalysisService = locationAnalysisService;
        }

        // ✅ Get test records for authenticated user
        [HttpGet("UserRecords")]
        public IActionResult GetUserTestRecords()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                return Unauthorized("User not authenticated.");

            var testRecords = _context.TestRecords
                .Where(t => t.UserId == userId)
                .Include(t => t.Lab)
                .Include(t => t.TestType)
                .ToList();

            var testRecordsDetailsList = testRecords
                .Select(tr => new TestRecordDetailsDTO
                {
                    TestId = tr.RecordId,
                    Result = tr.Result,
                    TestDetails = tr.Details,
                    LabName = tr.Lab?.LabName ?? "Unknown",
                    TestTypeName = tr.TestType?.TestName ?? "Unknown",
                    DateAdministered = tr.DateAdministered,
                    ContactInfo = tr.Lab?.ContactInfo ?? "N/A",
                    LabId = tr.Lab.LabId
                })
                .OrderByDescending(tr => tr.DateAdministered) // ✅ ascending by date
                .ToList();


            return Ok(testRecordsDetailsList);
        }

        // ✅ Get all test records (for admin view)
        [HttpGet("AllRecords")]
        //[Authorize(Roles = "Admin")] // Optional: protect this endpoint for admin only
        public IActionResult GetAllTestRecords([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var totalCount = _context.TestRecords.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var testRecords = _context.TestRecords
                .Include(t => t.User)
                .Include(t => t.TestType)
                .Include(t => t.Lab)
                .OrderByDescending(t => t.DateAdministered)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var testRecordsDetailsList = testRecords
                .Select(tr => new AllTestRecordsDTO
                {
                    TestId = tr.RecordId,
                    PatientName = $"{tr.User?.FirstName} {tr.User?.LastName}",
                    TestTypeName = tr.TestType?.TestName ?? "Unknown",
                    LabName = tr.Lab?.LabName ?? "Unknown",
                    PatientGender = tr.User?.Sex ?? "N/A",
                    PatientEmail = tr.User?.Email ?? "N/A",
                    PatientPhone = tr.User?.PhoneNumber ?? "N/A",
                    AreaName = "Location", // You can replace this with actual area logic
                    Result = tr.Result,
                    TestDate = tr.DateAdministered,
                    LabContactInfo = tr.Lab?.ContactInfo ?? "N/A",
                    TestDetails = tr.Details,
                    userId = tr.UserId,
                })
                .OrderByDescending(tr => tr.TestDate)
                .ToList();


            return Ok(new
            {
                records = testRecordsDetailsList,
                totalPages
            });
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignTest([FromBody] TestAssignmentRequest request)
        {
            if (request.UserIds == null || !request.UserIds.Any())
                return BadRequest("No user IDs provided.");

            foreach (var userId in request.UserIds)
            {
                var lab = _locationAnalysisService.GetClosestLab(userId);
                var assignment = new TestRecord
                {
                    UserId = userId,
                    TestTypeId = request.TestTypeId,
                    DateAdministered = request.AssignmentDate,
                    Result = "Pending", 
                    Details = "No Details",
                    LabId = lab.LabId,
                    
                };
                _context.TestRecords.Add(assignment);

                var newNotification = new Notification();
                newNotification.TatgetUserId = userId;
                newNotification.Title = "A new test has been assigned to you";
                newNotification.Message = $"Please consider going to the assigned Lab '{lab.LabName}' to do your test as fast as possible.";

                _context.Notifications.Add(newNotification);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Test assigned successfully." });
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetTestTypes()
        {
            var types = await _context.TestTypes
                .Select(t => new
                {
                    t.TestId,
                    t.TestName
                })
                .ToListAsync();

            return Ok(types);
        }

        [HttpGet("GetLabDirections")]
        [Authorize]
        public IActionResult GetLabDirections(int labId)
        {
            // 1. Get user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Invalid token: user ID missing.");

            int userId = int.Parse(userIdClaim.Value);

            // 2. Get user and their location
            var user = _context.Users
                .Include(u => u.UserLocation)
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null || user.UserLocation == null)
                return NotFound("User or user location not found.");

            double userLat = user.UserLocation.Latitude;
            double userLon = user.UserLocation.Longitude;

            // 3. Get lab locations
            var allLocations = _context.Labs.FirstOrDefault(l => l.LabId == labId);
            if (allLocations == null)
                return NotFound("No quarantine locations available.");

            // 5. Return the closest location
            return Ok(new
            {
                allLocations.X,
                allLocations.Y, 
                userLat,
                userLon,
            });
        }

        public class TestAssignmentRequest
        {
            public List<int> UserIds { get; set; }
            public int TestTypeId { get; set; }
            public DateTime AssignmentDate { get; set; } = DateTime.Now;
        }
        public class TestType
        {
            public int TestTypeId { get; set; }
            public string Name { get; set; }
        }
    }
}
