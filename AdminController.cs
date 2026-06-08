using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using real_time_chat.Services;
using real_time_chat.Models;

namespace real_time_chat.Controllers
{
    public class AdminController : Controller
    {
        private readonly MongoDbService _context;

        public AdminController(MongoDbService context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "admin") return RedirectToAction("Login", "Auth");

            var users = await _context.Users.Find(_ => true).ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> BlockUser(string id)
        {
            var update = Builders<User>.Update.Set(u => u.IsBlocked, true);
            await _context.Users.UpdateOneAsync(u => u.Id == id, update);
            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> UnblockUser(string id)
        {
            var update = Builders<User>.Update.Set(u => u.IsBlocked, false);
            await _context.Users.UpdateOneAsync(u => u.Id == id, update);
            return RedirectToAction("Dashboard");
        }
    }
}