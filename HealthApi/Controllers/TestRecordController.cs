using HealthApi.DTO;
using HealthApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;

namespace HealthApi.Controllers
{
    public class TestRecordController : Controller
    {
        private readonly AppDbContext context;
        private readonly HttpClient _httpClient;

        public TestRecordController (AppDbContext context , HttpClient httpClient) 
        {
            this.context = context;
            _httpClient = httpClient;
        }


        [HttpGet("api/TestRecords/UserRecords")]
        public IActionResult GetUserTestRecords()
        {
            //var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier); --------------------------------- change later 
            var userIdString = "5";

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var testRecordsDetailsList = new List <TestRecordDetailsDTO>();
            var testRecords = context.TestRecords.Where(t => t.UserId == userId).ToList();

            foreach (var testRecord in testRecords) 
            {
                var testRecordDetails = new TestRecordDetailsDTO();

                testRecordDetails.Result = testRecord.Result;
                testRecordDetails.TestDetails = testRecord.Details;
                testRecordDetails.TestId = testRecord.RecordId;
                testRecordDetails.LabName = context.Labs.FirstOrDefault(l => l.LabId == testRecord.LabId).LabName;
                testRecordDetails.TestTypeName = context.TestTypes.FirstOrDefault(t => t.TestId == testRecord.TestTypeId).TestName;
                testRecordDetails.DateAdministered = testRecord.DateAdministered;
                testRecordDetails.ContactInfo = context.Labs.FirstOrDefault(l => l.LabId == testRecord.LabId).ContactInfo;

                testRecordsDetailsList.Add(testRecordDetails);
            }



            return Ok(testRecordsDetailsList);
        }

        [HttpGet("api/TestRecords/AllRecords")]
        public IActionResult GetAllTestRecords([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var totalCount = context.TestRecords.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var testRecords = context.TestRecords
                .Include(t => t.User)
                .Include(t => t.TestType)
                .Include(t => t.Lab)
                .OrderByDescending(t => t.DateAdministered)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var testRecordsDetailsList = testRecords.Select(testRecord => new AllTestRecordsDTO
            {
                TestId = testRecord.RecordId,
                PatientName = testRecord.User.FirstName + " " + testRecord.User.LastName,
                TestTypeName = testRecord.TestType.TestName,
                LabName = testRecord.Lab.LabName,
                PatientGender = testRecord.User.Sex,
                PatientEmail = testRecord.User.Email,
                PatientPhone = testRecord.User.PhoneNumber,
                AreaName = "Location", // or real one if needed
                Result = testRecord.Result,
                TestDate = testRecord.DateAdministered,
                LabContactInfo = testRecord.Lab.ContactInfo,
                TestDetails = testRecord.Details
            }).ToList();

            return Ok(new
            {
                records = testRecordsDetailsList,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }




    }
}
