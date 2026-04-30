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

            var vm = new ReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate
            };

            var incomeQuery = _context.Incomes.AsQueryable();
            var expenseQuery = _context.Expenses.AsQueryable();

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

            vm.Incomes = incomeQuery.OrderByDescending(i => i.Date).ToList();
            vm.Expenses = expenseQuery.OrderByDescending(e => e.Date).ToList();

            return View(vm);
        }
    }
}