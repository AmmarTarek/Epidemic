using HealthApi.DTO; 
using HealthApi.Models;
using HealthApi.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SelfAssessmentController : ControllerBase
    {
        ISelfAssessmentRepository selfRepo;
        private readonly AppDbContext context;


        public SelfAssessmentController(ISelfAssessmentRepository _selfRepo, AppDbContext context)
        {

            selfRepo = _selfRepo;
            this.context = context;
        }
        [HttpGet]
        public ActionResult AllAssessments()
        {
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


        // salem edit assessment
        [HttpPost("CheckSymptoms")]
        public async Task<IActionResult> CheckSymptomsAsync([FromBody] SymptomRequestDTO request)
        {

            //var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userIdString = "5";

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var dangerScore = 0;

            if (request.Fever) dangerScore += 2;
            if (request.Cough) dangerScore += 2;
            if (request.BreathDifficulty) dangerScore += 3;
            if (request.LossTasteSmell) dangerScore += 2;
            if (request.Fatigue) dangerScore += 1;
            if (request.MuscleAches) dangerScore += 1;
            if (request.SoreThroat) dangerScore += 1;
            if (request.RunnyNose) dangerScore += 1;
            if (request.Nausea) dangerScore += 1;
            if (request.ContactPositive) dangerScore += 3;

            var isFlagged = dangerScore >= 6;


            if (isFlagged)
            {
                var user = await context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.IsFlagged = true;
                    context.Users.Update(user);

                    var assessment = new SelfAssessment
                    {
                        UserId = userId,
                        Symptoms = $"Fever: {request.Fever}," +
                        $" Cough: {request.Cough}," +
                        $" BreathDifficulty: {request.BreathDifficulty}," +
                        $" LossTasteSmell: {request.LossTasteSmell}," +
                        $" Fatigue: {request.Fatigue}," +
                        $" MuscleAches: {request.MuscleAches}," +
                        $" SoreThroat: {request.SoreThroat}," +
                        $" RunnyNose: {request.RunnyNose}," +
                        $" Nausea: {request.Nausea}," +
                        $" ContactPositive: {request.ContactPositive}",
                        IsFlagged = isFlagged,
                    };

                    context.SelfAssessments.Add(assessment);

                    await context.SaveChangesAsync();
                }
                else
                {
                    return NotFound("User not found.");
                }
            }
            else
            {
                var user = await context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.IsFlagged = false;
                    context.Users.Update(user);

                    var assessment = new SelfAssessment
                    {
                        UserId = userId,
                        Symptoms = $"Fever: {request.Fever}," +
                        $" Cough: {request.Cough}," +
                        $" BreathDifficulty: {request.BreathDifficulty}," +
                        $" LossTasteSmell: {request.LossTasteSmell}," +
                        $" Fatigue: {request.Fatigue}," +
                        $" MuscleAches: {request.MuscleAches}," +
                        $" SoreThroat: {request.SoreThroat}," +
                        $" RunnyNose: {request.RunnyNose}," +
                        $" Nausea: {request.Nausea}," +
                        $" ContactPositive: {request.ContactPositive}",
                        IsFlagged = isFlagged,
                    };

                    context.SelfAssessments.Add(assessment);

                    await context.SaveChangesAsync();
                }
                else
                {
                    return NotFound("User not found.");
                }
            }

            return Ok(new { isFlagged });
        }

        [HttpGet("LastCheck")]
        public IActionResult GetLastCheck()
        {
            //var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userIdString = "5"; // Replace with actual user ID retrieval logic ----------------------------------------------------- this is a placeholder for testing purposes


            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var lastCheck = context.SelfAssessments
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.Date) // replace Timestamp with your actual DateTime column
                .FirstOrDefault();

            if (lastCheck == null)
            {
                return NotFound("No assessments found for this user.");
            }

            var lastCheckTime = lastCheck.Date;
            var timeSince = DateTime.UtcNow - lastCheckTime;
            string readableTime;

            if (timeSince.TotalDays >= 7)
            {
                int weeks = (int)(timeSince.TotalDays / 7);
                readableTime = $"since {weeks} week{(weeks > 1 ? "s" : "")} ago";
            }
            else if (timeSince.TotalDays >= 1)
            {
                int days = (int)timeSince.TotalDays;
                readableTime = $"since {days} day{(days > 1 ? "s" : "")} ago";
            }
            else if (timeSince.TotalHours >= 1)
            {
                int hours = (int)timeSince.TotalHours;
                readableTime = $"since {hours} hour{(hours > 1 ? "s" : "")} ago";
            }
            else if (timeSince.TotalMinutes >= 1)
            {
                int minutes = (int)timeSince.TotalMinutes;
                readableTime = $"since {minutes} minute{(minutes > 1 ? "s" : "")} ago";
            }
            else
            {
                readableTime = "just now";
            }

            var result = new
            {
                lastCheckTime = lastCheckTime.ToString("u"),
                readableTime
            };

            return Ok(result);
        }

    }
}
 
