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

//        public TestRecordController (AppDbContext context , HttpClient httpClient) 
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

//            var testRecordsDetailsList = new List <TestRecordDetailsDTO>();
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Ensure all routes require authentication
    public class TestRecordsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestRecordsController(AppDbContext context)
        {
            _context = context;
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

            var testRecordsDetailsList = testRecords.Select(tr => new TestRecordDetailsDTO
            {
                TestId = tr.RecordId,
                Result = tr.Result,
                TestDetails = tr.Details,
                LabName = tr.Lab?.LabName ?? "Unknown",
                TestTypeName = tr.TestType?.TestName ?? "Unknown",
                DateAdministered = tr.DateAdministered,
                ContactInfo = tr.Lab?.ContactInfo ?? "N/A"
            }).ToList();

            return Ok(testRecordsDetailsList);
        }

        // ✅ Get all test records (for admin view)
        [HttpGet("AllRecords")]
        [Authorize(Roles = "Admin")] // Optional: protect this endpoint for admin only
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

            var testRecordsDetailsList = testRecords.Select(tr => new AllTestRecordsDTO
            {
                TestId = tr.RecordId,
                PatientName = $"{tr.User?.FirstName} {tr.User?.LastName}",
                TestTypeName = tr.TestType?.TestName ?? "Unknown",
                LabName = tr.Lab?.LabName ?? "Unknown",
                PatientGender = tr.User?.Sex ?? "N/A",
                PatientEmail = tr.User?.Email ?? "N/A",
                PatientPhone = tr.User?.PhoneNumber ?? "N/A",
                AreaName = "Location", // Placeholder
                Result = tr.Result,
                TestDate = tr.DateAdministered,
                LabContactInfo = tr.Lab?.ContactInfo ?? "N/A",
                TestDetails = tr.Details
            }).ToList();

            return Ok(new
            {
                records = testRecordsDetailsList,
                totalPages
            });
        }
    }
}
