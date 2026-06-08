using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using real_time_chat.Models;
using real_time_chat.Services;

namespace real_time_chat.Controllers
{
    public class ChatController : Controller
    {
        private readonly MongoDbService _context;
        private readonly IWebHostEnvironment _environment;

        public ChatController(MongoDbService context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile chatFile, string receiverId, string groupId)
        {
            if (chatFile == null || chatFile.Length == 0) return BadRequest("No file selected.");

            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + chatFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await chatFile.CopyToAsync(fileStream);
            }

            string fileUrl = "/uploads/" + uniqueFileName;
            var currentUserId = HttpContext.Session.GetString("UserId") ?? "Unknown";

            var newMessage = new Message
            {
                SenderId = currentUserId,
                ReceiverId = receiverId ?? string.Empty,
                GroupId = groupId ?? string.Empty,
                MessageText = $"Sent a file: {chatFile.FileName}",
                FileUrl = fileUrl,
                Timestamp = DateTime.UtcNow
            };

            await _context.Messages.InsertOneAsync(newMessage);
            return Json(new { success = true, fileUrl = fileUrl });
        }
    }
}