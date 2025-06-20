using Microsoft.AspNetCore.Mvc;
using HealthApi.DTO;
using HealthApi.Repository;
using HealthApi.Models;

namespace HealthApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class VaccineRecordController : ControllerBase
    {
            IVaccineRecordsRepository VacRepo;


            public VaccineRecordController(IVaccineRecordsRepository _vacRepo)
            {
            VacRepo = _vacRepo;
                
            }
            [HttpGet]
            public ActionResult AllRecirds()
            {
                List<VaccineRecordsDTO> records = VacRepo.GetAll();
                return Ok(records);
            }


        [HttpGet("by-id/{id:int}")]
        public IActionResult GetById(int id)
        {

            if (ModelState.IsValid)
            {
                VaccineRecordsDTO VAC_fromREq = VacRepo.GetById(id);

                return Ok(VAC_fromREq);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("by-user/{UserId:int}")]
        public IActionResult GetByUserId(int UserId)
        {

            if (ModelState.IsValid)
            {
                VaccineRecordsDTO VAC_fromREq = VacRepo.GetById(UserId);

                return Ok(VAC_fromREq);
            }
            return BadRequest(ModelState);
        }


        [HttpGet("by-TestType/{TestTypeId:int}")]
        public IActionResult GetByTestType(int TestTypeId)
        {

            if (ModelState.IsValid)
            {
                VaccineRecordsDTO VAC_fromREq = VacRepo.GetByTestType(TestTypeId);

                return Ok(VAC_fromREq);
            }
            return BadRequest(ModelState);
        }



        [HttpGet("by-Lab/{LabId:int}")]
        public IActionResult GetByLabId ( int LabId)
        {

            if (ModelState.IsValid)
            {
                VaccineRecordsDTO VAC_fromREq = VacRepo.GetByLabId(LabId);

                return Ok(VAC_fromREq);
            }
            return BadRequest(ModelState);
        }



        [HttpGet("by-Sex/{sex}")]
        public IActionResult GetBySex(string sex)
        {

            if (ModelState.IsValid)
            {
                VaccineRecordsDTO VAC_fromREq = VacRepo.GetBySex(sex);

                return Ok(VAC_fromREq);
            }
            return BadRequest(ModelState);
        }




        [HttpPost]
        public IActionResult Assign_NewRecord(VaccineRecordsDTO newVAC)
        {

            if (ModelState.IsValid)
            {
                VacRepo.Add(newVAC);
                VacRepo.Save();
                return Ok(new { data = newVAC, Message = "Record added successfuly" });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id:int}")]

        public IActionResult Edit_Status(int id, string newStatus)
        {

            if (ModelState.IsValid)
            {

              VaccineRecordsDTO record = VacRepo.GetById(id);
                record.Status = newStatus;
                VacRepo.Update(record);
                VacRepo.Save();
                return Ok(new { data = record, Message = "Record updated successfully" });

            }
            return BadRequest(ModelState);
        }


    }
}
