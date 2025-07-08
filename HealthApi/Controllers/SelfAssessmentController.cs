//using HealthApi.DTO; 
//using HealthApi.Models;
//using HealthApi.Repository;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

//namespace HealthApi.Controllers
//{

//    [Route("api/[controller]")]
//    [ApiController]
//    public class SelfAssessmentController : ControllerBase
//    {
//        ISelfAssessmentRepository selfRepo;
//        private readonly AppDbContext context;


//        public SelfAssessmentController(ISelfAssessmentRepository _selfRepo, AppDbContext context)
//        {

//            selfRepo = _selfRepo;
//            this.context = context;
//        }
//        [HttpGet]
//        public ActionResult AllAssessments()
//        {
//            List<SelfAssessmentDTO> assessments = selfRepo.GetAll();
//            return Ok(assessments);
//        }

//        [HttpGet("by-id/{id:int}")]
//        public IActionResult GetById(int id)
//        {

//            if (ModelState.IsValid)
//            {
//                SelfAssessmentDTO SA_fromREq = selfRepo.GetById(id);

//                return Ok(SA_fromREq);
//            }
//            return BadRequest(ModelState);
//        }

//        [HttpGet("by-user/{UserId:int}")]
//        public IActionResult GetByUserId(int UserId)
//        {

//            if (ModelState.IsValid)
//            {
//                SelfAssessmentDTO SA_fromREq = selfRepo.GetByUserId(UserId);

//                return Ok(SA_fromREq);
//            }
//            return BadRequest(ModelState);
//        }

//        [HttpGet("{IsFlagged:bool}")]
//        public IActionResult GetByIsFlagged(bool IsFlagged)
//        {

//            if (ModelState.IsValid)
//            {
//                SelfAssessmentDTO SA_fromREq = selfRepo.GetByIsFlagged(IsFlagged);

//                return Ok(SA_fromREq);
//            }
//            return BadRequest(ModelState);
//        }

//        [HttpPost]
//        public IActionResult Submit_Assessment(SelfAssessmentDTO newSA)
//        {

//            if (ModelState.IsValid)
//            {
//                selfRepo.Add(newSA);
//                selfRepo.Save();
//                return Ok(new { data = newSA, Message = "Assessment added successfuly" });
//            }
//            else
//            {
//                return BadRequest(ModelState);
//            }
//        }


//        // salem edit assessment
//        [HttpPost("CheckSymptoms")]
//        public async Task<IActionResult> CheckSymptomsAsync([FromBody] SymptomRequestDTO request)
//        {

//            //var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);-*-------------------------------------------
//            var userIdString = "5";

//            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
//            {
//                return Unauthorized("User not authenticated.");
//            }

//            var dangerScore = 0;

//            if (request.Fever) dangerScore += 2;
//            if (request.Cough) dangerScore += 2;
//            if (request.BreathDifficulty) dangerScore += 3;
//            if (request.LossTasteSmell) dangerScore += 2;
//            if (request.Fatigue) dangerScore += 1;
//            if (request.MuscleAches) dangerScore += 1;
//            if (request.SoreThroat) dangerScore += 1;
//            if (request.RunnyNose) dangerScore += 1;
//            if (request.Nausea) dangerScore += 1;
//            if (request.ContactPositive) dangerScore += 3;

//            var isFlagged = dangerScore >= 6;


//            if (isFlagged)
//            {
//                var user = await context.Users.FindAsync(userId);
//                if (user != null)
//                {
//                    user.IsFlagged = true;
//                    context.Users.Update(user);

//                    var assessment = new SelfAssessment
//                    {
//                        UserId = userId,
//                        Symptoms = $"Fever: {request.Fever}," +
//                        $" Cough: {request.Cough}," +
//                        $" BreathDifficulty: {request.BreathDifficulty}," +
//                        $" LossTasteSmell: {request.LossTasteSmell}," +
//                        $" Fatigue: {request.Fatigue}," +
//                        $" MuscleAches: {request.MuscleAches}," +
//                        $" SoreThroat: {request.SoreThroat}," +
//                        $" RunnyNose: {request.RunnyNose}," +
//                        $" Nausea: {request.Nausea}," +
//                        $" ContactPositive: {request.ContactPositive}",
//                        IsFlagged = isFlagged,
//                    };

//                    context.SelfAssessments.Add(assessment);

//                    await context.SaveChangesAsync();
//                }
//                else
//                {
//                    return NotFound("User not found.");
//                }
//            }
//            else
//            {
//                var user = await context.Users.FindAsync(userId);
//                if (user != null)
//                {
//                    user.IsFlagged = false;
//                    context.Users.Update(user);

//                    var assessment = new SelfAssessment
//                    {
//                        UserId = userId,
//                        Symptoms = $"Fever: {request.Fever}," +
//                        $" Cough: {request.Cough}," +
//                        $" BreathDifficulty: {request.BreathDifficulty}," +
//                        $" LossTasteSmell: {request.LossTasteSmell}," +
//                        $" Fatigue: {request.Fatigue}," +
//                        $" MuscleAches: {request.MuscleAches}," +
//                        $" SoreThroat: {request.SoreThroat}," +
//                        $" RunnyNose: {request.RunnyNose}," +
//                        $" Nausea: {request.Nausea}," +
//                        $" ContactPositive: {request.ContactPositive}",
//                        IsFlagged = isFlagged,
//                    };

//                    context.SelfAssessments.Add(assessment);

//                    await context.SaveChangesAsync();
//                }
//                else
//                {
//                    return NotFound("User not found.");
//                }
//            }

//            return Ok(new { isFlagged });
//        }

//        [HttpGet("LastCheck")]
//        public IActionResult GetLastCheck()
//        {
//            //var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            var userIdString = "5"; // Replace with actual user ID retrieval logic ----------------------------------------------------- this is a placeholder for testing purposes


//            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
//            {
//                return Unauthorized("User not authenticated.");
//            }

//            var lastCheck = context.SelfAssessments
//                .Where(i => i.UserId == userId)
//                .OrderByDescending(i => i.Date) // replace Timestamp with your actual DateTime column
//                .FirstOrDefault();

//            if (lastCheck == null)
//            {
//                return NotFound("No assessments found for this user.");
//            }

//            var lastCheckTime = lastCheck.Date;
//            var timeSince = DateTime.UtcNow - lastCheckTime;
//            string readableTime;

//            if (timeSince.TotalDays >= 7)
//            {
//                int weeks = (int)(timeSince.TotalDays / 7);
//                readableTime = $"since {weeks} week{(weeks > 1 ? "s" : "")} ago";
//            }
//            else if (timeSince.TotalDays >= 1)
//            {
//                int days = (int)timeSince.TotalDays;
//                readableTime = $"since {days} day{(days > 1 ? "s" : "")} ago";
//            }
//            else if (timeSince.TotalHours >= 1)
//            {
//                int hours = (int)timeSince.TotalHours;
//                readableTime = $"since {hours} hour{(hours > 1 ? "s" : "")} ago";
//            }
//            else if (timeSince.TotalMinutes >= 1)
//            {
//                int minutes = (int)timeSince.TotalMinutes;
//                readableTime = $"since {minutes} minute{(minutes > 1 ? "s" : "")} ago";
//            }
//            else
//            {
//                readableTime = "just now";
//            }

//            var result = new
//            {
//                lastCheckTime = lastCheckTime.ToString("u"),
//                readableTime
//            };

//            return Ok(result);
//        }

//    }
//}

using HealthApi.DTO;
using HealthApi.Models;
using HealthApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SelfAssessmentController : ControllerBase
    {
        private readonly ISelfAssessmentRepository _selfRepo;
        private readonly AppDbContext _context;

        public SelfAssessmentController(ISelfAssessmentRepository selfRepo, AppDbContext context)
        {
            _selfRepo = selfRepo;
            _context = context;
        }

        // GET: api/SelfAssessment
        [HttpGet]
        public IActionResult GetAllAssessments()
        {
            var assessments = _selfRepo.GetAll();
            return Ok(assessments);
        }

        [HttpGet("by-id/{id:int}")]
        public IActionResult GetById(int id)
        {
            var assessment = _selfRepo.GetById(id);
            return assessment != null ? Ok(assessment) : NotFound("Assessment not found.");
        }

        [HttpGet("by-user")]
        public IActionResult GetByUserId()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var assessment = _selfRepo.GetByUserId(userId.Value);
            return assessment != null ? Ok(assessment) : NotFound("Assessment not found.");
        }

        [HttpGet("flagged/{isFlagged:bool}")]
        public IActionResult GetByIsFlagged(bool isFlagged)
        {
            var result = _selfRepo.GetByIsFlagged(isFlagged);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult SubmitAssessment([FromBody] SelfAssessmentDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _selfRepo.Add(dto);
            _selfRepo.Save();
            return Ok(new { data = dto, message = "Assessment submitted successfully." });
        }

        [HttpPost("CheckSymptoms")]
        public async Task<IActionResult> CheckSymptomsAsync([FromBody] SymptomRequestDTO request)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("User not authenticated.");

            int dangerScore =
                (request.Fever ? 2 : 0) +
                (request.Cough ? 2 : 0) +
                (request.BreathDifficulty ? 3 : 0) +
                (request.LossTasteSmell ? 2 : 0) +
                (request.Fatigue ? 1 : 0) +
                (request.MuscleAches ? 1 : 0) +
                (request.SoreThroat ? 1 : 0) +
                (request.RunnyNose ? 1 : 0) +
                (request.Nausea ? 1 : 0) +
                (request.ContactPositive ? 3 : 0);

            bool isFlagged = dangerScore >= 6;

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("User not found.");

            user.IsFlagged = isFlagged;
            _context.Users.Update(user);

            var symptoms = string.Join(", ", new[]
            {
                $"Fever: {request.Fever}",
                $"Cough: {request.Cough}",
                $"BreathDifficulty: {request.BreathDifficulty}",
                $"LossTasteSmell: {request.LossTasteSmell}",
                $"Fatigue: {request.Fatigue}",
                $"MuscleAches: {request.MuscleAches}",
                $"SoreThroat: {request.SoreThroat}",
                $"RunnyNose: {request.RunnyNose}",
                $"Nausea: {request.Nausea}",
                $"ContactPositive: {request.ContactPositive}"
            });

            _context.SelfAssessments.Add(new SelfAssessment
            {
                UserId = userId.Value,
                Symptoms = symptoms,
                IsFlagged = isFlagged,
                Date = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return Ok(new { isFlagged });
        }

        [HttpGet("LastCheck")]
        public IActionResult GetLastCheck()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var lastCheck = _context.SelfAssessments
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Date)
                .FirstOrDefault();

            if (lastCheck == null)
                return NotFound("No assessments found.");

            var timeSince = DateTime.UtcNow - lastCheck.Date;
            string readableTime = FormatTimeSince(timeSince);

            return Ok(new
            {
                lastCheckTime = lastCheck.Date.ToString("u"),
                readableTime
            });
        }

        // --- Helper Methods ---
        private int? GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdString, out int userId) ? userId : null;
        }

        private string FormatTimeSince(TimeSpan time)
        {
            if (time.TotalDays >= 7)
                return $"since {(int)(time.TotalDays / 7)} week(s) ago";
            if (time.TotalDays >= 1)
                return $"since {(int)time.TotalDays} day(s) ago";
            if (time.TotalHours >= 1)
                return $"since {(int)time.TotalHours} hour(s) ago";
            if (time.TotalMinutes >= 1)
                return $"since {(int)time.TotalMinutes} minute(s) ago";
            return "just now";
        }
    }
}
