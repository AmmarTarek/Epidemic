using HealthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthApi.Controllers
{
    public class EPassController : Controller
    {

        private readonly AppDbContext context;
        public EPassController(AppDbContext context)
        {
            this.context = context;
        }


        //[HttpPost("api/epass/update")]
        //public IActionResult UpdateEPass([FromBody] UserIdsRequest request)
        //{
        //    var userIds = request.UserIds; // 🔥 Here you receive [5, 6]

        //    if (userIds == null || !userIds.Any())
        //    {
        //        return BadRequest("User IDs cannot be null or empty.");
        //    }

        //    foreach (var userId in userIds)
        //    {
        //        var user = context.Users.FirstOrDefault(u => u.UserId == userId);
        //        if (user != null)
        //        {
        //            if (user.EPassStatusId == 2)
        //            {
        //                user.EPassStatusId = 4;          

        //                var notification = new Notification
        //                {
        //                    TatgetUserId = user.UserId,
        //                    Title = "E-Pass Denied",
        //                    Message = "Your E-Pass has been denied.",
        //                    CreatedAt = DateTime.UtcNow
        //                };

        //                context.Notifications.Add(notification); // 🔥 Add notification to the context
        //            }
        //            else if (user.EPassStatusId == 4)
        //            {
        //                user.EPassStatusId = 2;

        //                var notification = new Notification
        //                {
        //                    TatgetUserId = user.UserId,
        //                    Title = "E-Pass Approved",
        //                    Message = "Your E-Pass has been approved.",
        //                    CreatedAt = DateTime.UtcNow
        //                };

        //                context.Notifications.Add(notification); // 🔥 Add notification to the context

        //            }
        //            else
        //            {
        //                return BadRequest($"User with ID {userId} does not have a valid EPass status.");
        //            }

        //            context.Users.Update(user);
        //        }
        //    }

        //    try
        //    {
        //        context.SaveChanges(); // 🔥 Save all notifications in one go
        //        return Ok(new { message = "E-Pass Status Changed successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }

        //}

        [HttpPost("api/epass/update")]
        public IActionResult UpdateEPass([FromBody] UserIdsRequest request)
        {
            var userIds = request.UserIds; // 🔥 Here you receive [5, 6]

            if (userIds == null || !userIds.Any())
            {
                return BadRequest("User IDs cannot be null or empty.");
            }

            foreach (var userId in userIds)
            {
                var user = context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    if (user.EPassStatusId == 2 && request.status == false)
                    {
                        user.EPassStatusId = 4;

                        var notification = new Notification
                        {
                            TatgetUserId = user.UserId,
                            Title = "E-Pass Denied",
                            Message = "Your E-Pass has been denied.",
                            CreatedAt = DateTime.UtcNow
                        };

                        context.Notifications.Add(notification); // 🔥 Add notification to the context
                    }
                    else if (user.EPassStatusId == 4 && request.status == true)
                    {
                        user.EPassStatusId = 2;

                        var notification = new Notification
                        {
                            TatgetUserId = user.UserId,
                            Title = "E-Pass Approved",
                            Message = "Your E-Pass has been approved.",
                            CreatedAt = DateTime.UtcNow
                        };

                        context.Notifications.Add(notification); // 🔥 Add notification to the context

                    }
                    //else
                    //{
                    //    return BadRequest($"User with ID {userId} does not have a valid EPass status.");
                    //}

                    context.Users.Update(user);
                }
            }

            try
            {
                context.SaveChanges(); // 🔥 Save all notifications in one go
                return Ok(new { message = "E-Pass Status Changed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        public class UserIdsRequest
        {
            public List<int> UserIds { get; set; }
            public bool status { get; set; } 
        }


        [HttpGet("api/EPass/status")]
        //[Authorize] // Requires token
        public async Task<IActionResult> GetStatus()
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = 5;

            if (userId == null) return Unauthorized();

            var user = await context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            return Ok((int)user.EPassStatusId);
        }
        
    }
}
