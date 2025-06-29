using Microsoft.AspNetCore.Mvc;
using HealthApi.Models;

namespace HealthApi.Controllers
{
    public class notificationsController : Controller
    {
        private readonly AppDbContext context;

        public notificationsController(AppDbContext context) 
        { 
            this.context = context;
        }

        [HttpPost("api/SendNotification")]
        public IActionResult SendNotification([FromBody] NotificationRequest request)
        {
            var userIds = request.UserIds; // 🔥 Here you receive [5, 6]

            if (userIds == null || !userIds.Any())
            {
                return BadRequest("User IDs cannot be null or empty.");
            }

            foreach (var userId in userIds)
            {
                var notification = new Notification
                {
                    TatgetUserId = userId,
                    Title = request.Title,
                    Message = request.Message,
                };

                context.Notifications.Add(notification);
            }

            try
            {
                context.SaveChanges(); // 🔥 Save all notifications in one go
                return Ok(new { message = "Notifications sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }



        }

        [HttpGet("api/GetMyNotifications/{userId}")]
        public IActionResult GetMyNotifications(int userId, int pageNumber = 1, int pageSize = 5)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID.");

            var query = context.Notifications
                .Where(n => n.TatgetUserId == userId)
                .OrderByDescending(n => n.CreatedAt);

            var totalCount = query.Count();

            var pagedNotifications = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Notifications = pagedNotifications
            };

            return Ok(result);
        }


        public class NotificationRequest
        {
            public List<int> UserIds { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
        }




    }
}
