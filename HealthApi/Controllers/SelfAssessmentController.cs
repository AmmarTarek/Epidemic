using Microsoft.AspNetCore.Mvc;
using HealthApi.Repository;
using HealthApi.Models;
using HealthApi.DTO; 

namespace HealthApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SelfAssessmentController : ControllerBase
    {
        ISelfAssessmentRepository selfRepo;

        
        public SelfAssessmentController(ISelfAssessmentRepository _selfRepo)
        {

            selfRepo = _selfRepo;
        }
        [HttpGet]
        public ActionResult AllAssessments() {
            List<SelfAssessmentDTO> assessments = selfRepo.GetAll();
            return Ok(assessments);
        }

        [HttpGet("by-id/{id:int}")]
        public IActionResult GetById(int id)
        {

            if (ModelState.IsValid)
            {
                SelfAssessmentDTO SA_fromREq = selfRepo.GetById(id);

                return Ok(SA_fromREq);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("by-user/{UserId:int}")]
        public IActionResult GetByUserId(int UserId)
        {

            if (ModelState.IsValid)
            {
                SelfAssessmentDTO SA_fromREq = selfRepo.GetByUserId(UserId);

                return Ok(SA_fromREq);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("{IsFlagged:bool}")]
        public IActionResult GetByIsFlagged(bool IsFlagged)
        {

            if (ModelState.IsValid)
            {
                SelfAssessmentDTO SA_fromREq = selfRepo.GetByIsFlagged(IsFlagged);

                return Ok(SA_fromREq);
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        public IActionResult Submit_Assessment(SelfAssessmentDTO newSA)
        {

            if (ModelState.IsValid)
            {
                selfRepo.Add(newSA);
                selfRepo.Save();
                return Ok(new { data = newSA, Message = "Assessment added successfuly" });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

    }
    }

