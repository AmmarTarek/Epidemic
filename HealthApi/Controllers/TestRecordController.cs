using HealthApi.DTO;
using HealthApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthApi.Controllers
{
    public class TestRecordController : Controller
    {
        private readonly AppDbContext context;

        public TestRecordController (AppDbContext context) 
        {
            this.context = context;
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
    }
}
