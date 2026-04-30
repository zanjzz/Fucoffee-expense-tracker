using System.Collections.Generic;
using Fucoffee.Models;

namespace Fucoffee.Models.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit => TotalIncome - TotalExpenses;

        public List<Income> RecentIncomes { get; set; } = new();
        public List<Expense> RecentExpenses { get; set; } = new();

        // Chart properties
        public List<string> Last6Months { get; set; } = new();
        public List<decimal> MonthlyIncome { get; set; } = new();
        public List<decimal> MonthlyExpenses { get; set; } = new();
        public Dictionary<string, decimal> ExpensesByCategory { get; set; } = new();
    }
}