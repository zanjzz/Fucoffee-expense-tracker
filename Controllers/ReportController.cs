using Fucoffee.Data;
using Fucoffee.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Fucoffee.Controllers
{
    public class ReportController : Controller
    {
        private readonly AppDbContext _context;

        public ReportController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Auth");

            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetString("Role") == "Admin";

            var incomeQuery = _context.Incomes.AsQueryable();
            var expenseQuery = _context.Expenses.AsQueryable();

            // Apply user filtering
            if (!isAdmin && userId.HasValue)
            {
                incomeQuery = incomeQuery.Where(i => i.RecordedBy == userId.Value);
                expenseQuery = expenseQuery.Where(e => e.RecordedBy == userId.Value);
            }

            // Apply date filters
            if (startDate.HasValue)
            {
                incomeQuery = incomeQuery.Where(i => i.Date >= startDate.Value);
                expenseQuery = expenseQuery.Where(e => e.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                incomeQuery = incomeQuery.Where(i => i.Date <= endDate.Value);
                expenseQuery = expenseQuery.Where(e => e.Date <= endDate.Value);
            }

            var vm = new ReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Incomes = incomeQuery.OrderByDescending(i => i.Date).ToList(),
                Expenses = expenseQuery.OrderByDescending(e => e.Date).ToList()
            };

            return View(vm);
        }
    }
}