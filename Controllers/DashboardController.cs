using Fucoffee.Data;
using Fucoffee.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Fucoffee.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Get UserId from session (stored as int by your AuthController)
            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetString("Role") == "Admin";

            // If user is not logged in, redirect to login
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Base queries - using userId (int) to match RecordedBy
            var incomeQuery = _context.Incomes.AsQueryable();
            var expenseQuery = _context.Expenses.AsQueryable();

            if (!isAdmin)
            {
                incomeQuery = incomeQuery.Where(i => i.RecordedBy == userId.Value);
                expenseQuery = expenseQuery.Where(e => e.RecordedBy == userId.Value);
            }

            // 1. Totals for summary cards
            var totalIncome = incomeQuery.Sum(i => (decimal?)i.Amount) ?? 0;
            var totalExpenses = expenseQuery.Sum(e => (decimal?)e.Amount) ?? 0;

            // 2. Recent records (last 5)
            var recentIncomes = incomeQuery
                .OrderByDescending(i => i.Date)
                .Take(5)
                .ToList();
            var recentExpenses = expenseQuery
                .OrderByDescending(e => e.Date)
                .Take(5)
                .ToList();

            // 3. Chart data - Last 6 months
            var last6Months = new List<string>();
            var monthlyIncome = new List<decimal>();
            var monthlyExpenses = new List<decimal>();

            for (int i = 5; i >= 0; i--)
            {
                var month = DateTime.Now.AddMonths(-i);
                last6Months.Add(month.ToString("MMM"));

                var monthIncome = incomeQuery
                    .Where(x => x.Date.Year == month.Year && x.Date.Month == month.Month)
                    .Sum(x => (decimal?)x.Amount) ?? 0;
                monthlyIncome.Add(monthIncome);

                var monthExpense = expenseQuery
                    .Where(x => x.Date.Year == month.Year && x.Date.Month == month.Month)
                    .Sum(x => (decimal?)x.Amount) ?? 0;
                monthlyExpenses.Add(monthExpense);
            }

            // 4. Expenses by category for pie chart (handle empty case)
            var expensesByCategory = new Dictionary<string, decimal>();

            if (expenseQuery.Any())
            {
                expensesByCategory = expenseQuery
                    .GroupBy(e => e.Category)
                    .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));
            }

            var model = new DashboardViewModel
            {
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                RecentIncomes = recentIncomes,
                RecentExpenses = recentExpenses,
                Last6Months = last6Months,
                MonthlyIncome = monthlyIncome,
                MonthlyExpenses = monthlyExpenses,
                ExpensesByCategory = expensesByCategory
            };

            return View(model);
        }
    }
}