using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Data;
using ReservationSystem.Models;

namespace ReservationSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
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
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            // Weryfikacja hasła hashowanego
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            // Zapis do sesji
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserEmail", user.Email);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}