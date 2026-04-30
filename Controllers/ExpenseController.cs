using Fucoffee.Data;
using Fucoffee.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fucoffee.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly AppDbContext _context;

        public ExpenseController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            var expenses = _context.Expenses
                .OrderByDescending(e => e.Date)
                .ToList();

            return View(expenses);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        public IActionResult Create(Expense model)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid) return View(model);

            model.RecordedBy = HttpContext.Session.GetInt32("UserId") ?? 0;
            _context.Expenses.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            var expense = _context.Expenses.Find(id);
            if (expense == null) return NotFound();

            return View(expense);
        }

        [HttpPost]
        public IActionResult Edit(Expense model)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid) return View(model);

            _context.Expenses.Update(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            var expense = _context.Expenses.Find(id);
            if (expense == null) return NotFound();

            return View(expense);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            var expense = _context.Expenses.Find(id);
            if (expense == null) return NotFound();

            _context.Expenses.Remove(expense);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}