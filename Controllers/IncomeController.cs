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

        // READ — list all income records
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            var incomes = _context.Incomes
                .OrderByDescending(i => i.Date)
                .ToList();

            return View(incomes);
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

            var income = _context.Incomes.Find(id);
            if (income == null) return NotFound();

            return View(income);
        }

        // EDIT — receive and save updated form
        [HttpPost]
        public IActionResult Edit(Income model)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid) return View(model);

            _context.Incomes.Update(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // DELETE — confirm page
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            var income = _context.Incomes.Find(id);
            if (income == null) return NotFound();

            return View(income);
        }

        // DELETE — actually delete it
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            var income = _context.Incomes.Find(id);
            if (income == null) return NotFound();

            _context.Incomes.Remove(income);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}