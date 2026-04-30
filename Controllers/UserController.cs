using Fucoffee.Data;
using Fucoffee.Models;
using Fucoffee.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Fucoffee.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Index", "Dashboard");

            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Index", "Dashboard");

            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User model)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Index", "Dashboard");

            var user = _context.Users.Find(model.UserId);
            if (user == null) return NotFound();

            user.Username = model.Username;
            user.Role = model.Role;

            _context.Users.Update(user);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Index", "Dashboard");

            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Index", "Dashboard");

            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}