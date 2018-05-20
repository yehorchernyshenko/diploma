using System;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationContext _context;

        public AdminController(ApplicationContext context) {
            _context = context;
        }

        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUserMessages() {
            var userMessages = await _context.UserMessages
                .Include(userMessage => userMessage.User)
                .ToListAsync();

            return View("UserMessages", userMessages);
        }

        [HttpGet]
        public async Task<IActionResult> GetUnverifiedUsers() {
            var unverifiedUsers = await _context.User.Where(user => user.IsAccountVerified == false).ToListAsync();

            return View("UnverifiedUsers", unverifiedUsers);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers() {
            var allUsers = await _context.User.ToListAsync();

            return View("AllUsersList", allUsers);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyUser(string userId) {
            var user = await _context.User.FirstAsync(item => item.Id == userId);

            user.IsAccountVerified = true;

            await _context.SaveChangesAsync();

            return RedirectToAction("GetUnverifiedUsers");
        }


        [HttpPost]
        public async Task<IActionResult> UnblockUser(string userId) {
            var user = await _context.User.FirstAsync(item => item.Id == userId);

            user.IsBlocked = false;

            await _context.SaveChangesAsync();

            return RedirectToAction("GetAllUsers");
        }

        [HttpPost]
        public async Task<IActionResult> BlockUser(string userId) {
            var user = await _context.User.FirstAsync(item => item.Id == userId);

            user.IsBlocked = true;

            await _context.SaveChangesAsync();

            return RedirectToAction("GetAllUsers");
        }
    }
}