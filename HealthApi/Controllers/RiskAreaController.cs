using HealthApi.Models;
using HealthApi.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HealthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RiskAreaController : Controller
    {
        IRiskAreaRepository RisRepo;

        public RiskAreaController(IRiskAreaRepository _risRepo)
        {
            RisRepo = _risRepo;

        }
        [HttpGet]
        public ActionResult AllAreas()
        {
            List<RiskArea> RiskAreas = RisRepo.GetAll();
            return Ok(RiskAreas);
        }


        [HttpGet("by-id/{id:int}")]
        public IActionResult GetById(int id)
        {

            if (ModelState.IsValid)
            {
                RiskArea RiskArea_fromREq = RisRepo.GetById(id);

                return Ok(RiskArea_fromREq);
            }
            return BadRequest(ModelState);
        }


        [HttpGet("by-level/{level}")]
        public IActionResult GetByRiskLevel(string level)
        {

            if (ModelState.IsValid)
            {
                List<RiskArea> RiskAreas_fromREq = RisRepo.GetByRiskLevel(level);

                return Ok(RiskAreas_fromREq);
            }
            return BadRequest(ModelState);
        }
    }
}
