using Fucoffee.Data;
using Fucoffee.Models;
using Fucoffee.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Fucoffee.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Auth/Login
        public IActionResult Login()
        {
            // If already logged in, skip login page
            if (HttpContext.Session.GetString("Username") != null)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {


            if (!ModelState.IsValid) return View(model);

            var user = _context.Users
                .FirstOrDefault(u => u.Username == model.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            // Save to session
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToAction("Index", "Dashboard");
        }

        // GET: /Auth/Register (Admin only in real use — for now open for setup)
        public IActionResult Register() => View();

        // POST: /Auth/Register
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (_context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("", "Username already taken");
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = model.Role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // GET: /Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}