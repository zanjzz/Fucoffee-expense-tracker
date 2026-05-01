using Fucoffee.Data;
using Fucoffee.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fucoffee.Controllers
{
    public class IncomeController : Controller
    {
        private readonly AppDbContext _context;

        public IncomeController(AppDbContext context)
        {
            _context = context;
        }

        // READ — list income records (filtered by user)
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetString("Role") == "Admin";

            var incomes = _context.Incomes.AsQueryable();

            if (!isAdmin && userId.HasValue)
            {
                incomes = incomes.Where(i => i.RecordedBy == userId.Value);
            }

            incomes = incomes.OrderByDescending(i => i.Date);

            return View(incomes.ToList());
        }

        // CREATE — show empty form
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            return View();
        }

        // CREATE — receive and save form
        [HttpPost]
        public IActionResult Create(Income model)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid) return View(model);

            model.RecordedBy = HttpContext.Session.GetInt32("UserId") ?? 0;
            _context.Incomes.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // EDIT — show form pre-filled with existing data
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetString("Role") == "Admin";

            var income = _context.Incomes.Find(id);
            if (income == null) return NotFound();

            // Check ownership
            if (!isAdmin && income.RecordedBy != userId)
                return Unauthorized();

            return View(income);
        }

        // EDIT — receive and save updated form
        [HttpPost]
        public IActionResult Edit(Income model)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid) return View(model);

            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetString("Role") == "Admin";

            var existingIncome = _context.Incomes.Find(model.IncomeId);
            if (existingIncome == null) return NotFound();

            // Check ownership
            if (!isAdmin && existingIncome.RecordedBy != userId)
                return Unauthorized();

            existingIncome.Source = model.Source;
            existingIncome.Amount = model.Amount;
            existingIncome.Description = model.Description;
            existingIncome.Date = model.Date;

            _context.Incomes.Update(existingIncome);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // DELETE — confirm page
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetString("Role") == "Admin";

            var income = _context.Incomes.Find(id);
            if (income == null) return NotFound();

            // Check ownership
            if (!isAdmin && income.RecordedBy != userId)
                return Unauthorized();

            return View(income);
        }

        // DELETE — actually delete it
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetString("Role") == "Admin";

            var income = _context.Incomes.Find(id);
            if (income == null) return NotFound();

            // Check ownership
            if (!isAdmin && income.RecordedBy != userId)
                return Unauthorized();

            _context.Incomes.Remove(income);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}