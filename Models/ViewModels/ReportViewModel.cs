namespace Fucoffee.Models.ViewModels
{
    public class ReportViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<Income> Incomes { get; set; } = new();
        public List<Expense> Expenses { get; set; } = new();

        public decimal TotalIncome => Incomes.Sum(i => i.Amount);
        public decimal TotalExpenses => Expenses.Sum(e => e.Amount);
        public decimal NetProfit => TotalIncome - TotalExpenses;
    }
}