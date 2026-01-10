using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Data;
using ReservationSystem.Models;

namespace ReservationSystem.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegisterController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string email, string password)
        {
            if (_context.Users.Any(u => u.Email == email))
            {
                ViewBag.Error = "Email already exists.";
                return View();
            }

            var user = new User
            {
                Email = email,
                Role = "User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Index", "Login");
        }
    }
}