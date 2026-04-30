using System.ComponentModel.DataAnnotations;

namespace Fucoffee.Models
{
    public class Income
    {
        public int IncomeId { get; set; }

        [Required]
        public string Source { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.Now;

        // Foreign key
        public int RecordedBy { get; set; }
        public User? User { get; set; }
    }
}