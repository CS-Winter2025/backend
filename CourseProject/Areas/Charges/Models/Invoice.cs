
namespace CourseProject.Models;

public class Invoice
{
    public int InvoiceID { get; set; }
    public int ResidentId { get; set; } // Foreign Key (Stored in DB)
    public Resident Resident { get; set; } = null!; // Navigation Property (Not stored)
    public DateTime Date { get; set; }
    public decimal AmountDue { get; set; }
    public decimal AmountPaid { get; set; }
}
