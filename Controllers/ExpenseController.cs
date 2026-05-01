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

            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetString("Role") == "Admin";

            var expenses = _context.Expenses.AsQueryable();

            if (!isAdmin && userId.HasValue)
            {
                expenses = expenses.Where(e => e.RecordedBy == userId.Value);
            }

            expenses = expenses.OrderByDescending(e => e.Date);

            return View(expenses.ToList());
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

            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetString("Role") == "Admin";

            var expense = _context.Expenses.Find(id);
            if (expense == null) return NotFound();

            if (!isAdmin && expense.RecordedBy != userId)
                return Unauthorized();

            return View(expense);
        }

        [HttpPost]
        public IActionResult Edit(Expense model)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid) return View(model);

            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetString("Role") == "Admin";

            var existingExpense = _context.Expenses.Find(model.ExpenseId);
            if (existingExpense == null) return NotFound();

            if (!isAdmin && existingExpense.RecordedBy != userId)
                return Unauthorized();

            existingExpense.Category = model.Category;
            existingExpense.Amount = model.Amount;
            existingExpense.Description = model.Description;
            existingExpense.Date = model.Date;

            _context.Expenses.Update(existingExpense);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetString("Role") == "Admin";

            var expense = _context.Expenses.Find(id);
            if (expense == null) return NotFound();

            if (!isAdmin && expense.RecordedBy != userId)
                return Unauthorized();

            return View(expense);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetString("Role") == "Admin";

            var expense = _context.Expenses.Find(id);
            if (expense == null) return NotFound();

            if (!isAdmin && expense.RecordedBy != userId)
                return Unauthorized();

            _context.Expenses.Remove(expense);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}