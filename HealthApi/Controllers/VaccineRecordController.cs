//using HealthApi.DTO;
//using HealthApi.Models;
//using HealthApi.Repository;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace HealthApi.Controllers
//{

//    [Route("api/[controller]")]
//    [ApiController]
//    public class VaccineRecordController : ControllerBase
//    {
//            IVaccineRecordsRepository VacRepo;
//            private readonly AppDbContext context;


//            public VaccineRecordController(IVaccineRecordsRepository _vacRepo , AppDbContext context)
//            {
//            VacRepo = _vacRepo;
//            this.context = context;

//            }
//            [HttpGet]
//            public ActionResult AllRecirds()
//            {
//                List<VaccineRecordsDTO> records = VacRepo.GetAll();
//                return Ok(records);
//            }


//        [HttpGet("by-id/{id:int}")]
//        public IActionResult GetById(int id)
//        {

//            if (ModelState.IsValid)
//            {
//                VaccineRecordsDTO VAC_fromREq = VacRepo.GetById(id);

//                return Ok(VAC_fromREq);
//            }
//            return BadRequest(ModelState);
//        }

//        [HttpGet("by-user/{UserId:int}")]
//        public IActionResult GetByUserId(int UserId)
//        {

//            if (ModelState.IsValid)
//            {
//                VaccineRecordsDTO VAC_fromREq = VacRepo.GetById(UserId);

//                return Ok(VAC_fromREq);
//            }
//            return BadRequest(ModelState);
//        }


//        [HttpGet("by-TestType/{TestTypeId:int}")]
//        public IActionResult GetByTestType(int TestTypeId)
//        {

//            if (ModelState.IsValid)
//            {
//                VaccineRecordsDTO VAC_fromREq = VacRepo.GetByTestType(TestTypeId);

//                return Ok(VAC_fromREq);
//            }
//            return BadRequest(ModelState);
//        }



//        [HttpGet("by-Lab/{LabId:int}")]
//        public IActionResult GetByLabId ( int LabId)
//        {

//            if (ModelState.IsValid)
//            {
//                VaccineRecordsDTO VAC_fromREq = VacRepo.GetByLabId(LabId);

//                return Ok(VAC_fromREq);
//            }
//            return BadRequest(ModelState);
//        }



//        [HttpGet("by-Sex/{sex}")]
//        public IActionResult GetBySex(string sex)
//        {

//            if (ModelState.IsValid)
//            {
//                VaccineRecordsDTO VAC_fromREq = VacRepo.GetBySex(sex);

//                return Ok(VAC_fromREq);
//            }
//            return BadRequest(ModelState);
//        }




//        [HttpPost]
//        public IActionResult Assign_NewRecord(VaccineRecordsDTO newVAC)
//        {

//            if (ModelState.IsValid)
//            {
//                VacRepo.Add(newVAC);
//                VacRepo.Save();
//                return Ok(new { data = newVAC, Message = "Record added successfuly" });
//            }
//            else
//            {
//                return BadRequest(ModelState);
//            }
//        }

//        [HttpPut("{id:int}")]

//        public IActionResult Edit_Status(int id, string newStatus)
//        {

//            if (ModelState.IsValid)
//            {

//              VaccineRecordsDTO record = VacRepo.GetById(id);
//                record.Status = newStatus;
//                VacRepo.Update(record);
//                VacRepo.Save();
//                return Ok(new { data = record, Message = "Record updated successfully" });

//            }
//            return BadRequest(ModelState);
//        }

//        [HttpGet("UserRecords")]
//        public IActionResult GetUserVaccineRecords()
//        {
//            // Use claim later; temporarily hardcoded
//            // var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            var userIdString = "5";

//            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
//            {
//                return Unauthorized("User not authenticated.");
//            }
//            var vaccineRecordsDetailsList = new List<VaccineRecordDTO>();
//            var vaccineRecords = context.VaccineRecords.Where(v => v.UserId == userId).ToList();

//            foreach (var record in vaccineRecords)
//            {
//                var vaccineType = context.VaccineTypes.FirstOrDefault(t => t.VaccineId == record.VaccineTypeId);
//                var lab = context.Labs.FirstOrDefault(l => l.LabId == record.LabId);

//                var recordDetails = new VaccineRecordDTO
//                {
//                    VaccineTypeName = vaccineType?.VaccineName,
//                    Status = record.Status,
//                    DateOfAssignment = record.DateOfAssignment,
//                    DateOfVaccined = record.DateOfVaccined,
//                    LabName = lab?.LabName,
//                    ContactInfo = lab?.ContactInfo,
//                    StatusMessage = record.StatusMessage
//                };

//                vaccineRecordsDetailsList.Add(recordDetails);
//            }

//            return Ok(vaccineRecordsDetailsList);
//        }




//    }
//}

using HealthApi.DTO;
using HealthApi.Models;
using HealthApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VaccineRecordController : ControllerBase
    {
        private readonly IVaccineRecordsRepository _vacRepo;
        private readonly AppDbContext _context;

        public VaccineRecordController(IVaccineRecordsRepository vacRepo, AppDbContext context)
        {
            _vacRepo = vacRepo;
            _context = context;
        }

        [HttpGet]
        public ActionResult GetAll()
        {
            var records = _vacRepo.GetAll();
            return Ok(records);
        }

        [HttpGet("by-id/{id:int}")]
        public IActionResult GetById(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var record = _vacRepo.GetById(id);
            return Ok(record);
        }

        [HttpGet("by-user/{userId:int}")]
        public IActionResult GetByUserId(int userId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var record = _vacRepo.GetById(userId); // ⚠️ double-check: should this call a `GetByUserId`?
            return Ok(record);
        }

        [HttpGet("by-TestType/{testTypeId:int}")]
        public IActionResult GetByTestType(int testTypeId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var record = _vacRepo.GetByTestType(testTypeId);
            return Ok(record);
        }

        [HttpGet("by-Lab/{labId:int}")]
        public IActionResult GetByLabId(int labId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var record = _vacRepo.GetByLabId(labId);
            return Ok(record);
        }

        [HttpGet("by-Sex/{sex}")]
        public IActionResult GetBySex(string sex)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var record = _vacRepo.GetBySex(sex);
            return Ok(record);
        }

        [HttpPost]
        public IActionResult AssignNewRecord([FromBody] VaccineRecordsDTO newVac)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _vacRepo.Add(newVac);
            _vacRepo.Save();

            return Ok(new { data = newVac, Message = "Record added successfully" });
        }

        [HttpPut("{id:int}")]
        public IActionResult EditStatus(int id, [FromQuery] string newStatus)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var record = _vacRepo.GetById(id);
            if (record == null) return NotFound("Record not found");

            record.Status = newStatus;
            _vacRepo.Update(record);
            _vacRepo.Save();

            return Ok(new { data = record, Message = "Record updated successfully" });
        }

        // ✅ Secure endpoint that gets records for the authenticated user
        [HttpGet("UserRecords")]
        public IActionResult GetUserVaccineRecords()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var vaccineRecordsDetailsList = _context.VaccineRecords
                .Where(v => v.UserId == userId)
                .Include(v => v.Lab)
                .Include(v => v.VaccineType)
                .Select(record => new VaccineRecordDTO
                {
                    VaccineTypeName = record.VaccineType.VaccineName,
                    Status = record.Status,
                    DateOfAssignment = record.DateOfAssignment,
                    DateOfVaccined = record.DateOfVaccined,
                    LabName = record.Lab.LabName,
                    ContactInfo = record.Lab.ContactInfo,
                    StatusMessage = record.StatusMessage
                }).ToList();

            return Ok(vaccineRecordsDetailsList);
        }
    }
}
