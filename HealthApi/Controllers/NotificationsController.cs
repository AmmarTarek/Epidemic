//using Microsoft.AspNetCore.Mvc;
//using HealthApi.Models;

//namespace HealthApi.Controllers
//{
//    public class notificationsController : Controller
//    {
//        private readonly AppDbContext context;

//        public notificationsController(AppDbContext context) 
//        { 
//            this.context = context;
//        }

//        [HttpPost("api/SendNotification")]
//        public IActionResult SendNotification([FromBody] NotificationRequest request)
//        {
//            var userIds = request.UserIds; // 🔥 Here you receive [5, 6]

//            if (userIds == null || !userIds.Any())
//            {
//                return BadRequest("User IDs cannot be null or empty.");
//            }

//            foreach (var userId in userIds)
//            {
//                var notification = new Notification
//                {
//                    TatgetUserId = userId,
//                    Title = request.Title,
//                    Message = request.Message,
//                };

//                context.Notifications.Add(notification);
//            }

//            try
//            {
//                context.SaveChanges(); // 🔥 Save all notifications in one go
//                return Ok(new { message = "Notifications sent successfully" });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }



//        }

//        [HttpGet("api/GetMyNotifications/{userId}")]
//        public IActionResult GetMyNotifications(int userId, int pageNumber = 1, int pageSize = 5)
//        {
//            if (userId <= 0)
//                return BadRequest("Invalid user ID.");

//            var query = context.Notifications
//                .Where(n => n.TatgetUserId == userId)
//                .OrderByDescending(n => n.CreatedAt);

//            var totalCount = query.Count();

//            var pagedNotifications = query
//                .Skip((pageNumber - 1) * pageSize)
//                .Take(pageSize)
//                .ToList();

//            var result = new
//            {
//                TotalCount = totalCount,
//                PageNumber = pageNumber,
//                PageSize = pageSize,
//                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
//                Notifications = pagedNotifications
//            };

//            return Ok(result);
//        }


//        public class NotificationRequest
//        {
//            public List<int> UserIds { get; set; }
//            public string Title { get; set; }
//            public string Message { get; set; }
//        }




//    }
//}


using HealthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthApi.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        // Send Notification to Multiple Users
        [HttpPost("api/SendNotification")]
        public IActionResult SendNotification([FromBody] NotificationRequest request)
        {
            if (request.UserIds == null || !request.UserIds.Any())
                return BadRequest("User IDs cannot be null or empty.");

            foreach (var userId in request.UserIds)
            {
                var notification = new Notification
                {
                    TatgetUserId = userId,
                    Title = request.Title,
                    Message = request.Message,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
            }

            try
            {
                _context.SaveChanges();
                return Ok(new { message = "Notifications sent successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error saving notifications: {ex.Message}");
            }
        }

        // Get Notifications for Logged-in User
        [HttpGet("api/GetMyNotifications")]
        public IActionResult GetMyNotifications(int pageNumber = 1, int pageSize = 5)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized("Invalid or missing user ID in token.");

            var query = _context.Notifications
                .Where(n => n.TatgetUserId == userId)
                .OrderByDescending(n => n.CreatedAt);

            var totalCount = query.Count();

            var notifications = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Notifications = notifications
            };

            return Ok(result);
        }

        // DTO class for sending notifications
        public class NotificationRequest
        {
            public List<int> UserIds { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
        }
    }
}
