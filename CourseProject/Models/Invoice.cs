namespace CourseProject.Models
{

    public class Invoice
    {
        public int InvoiceID { get; set; }
        public int ResidentID { get; set; }
        public DateTime Date { get; set; }
        public decimal AmountDue { get; set; }
        public decimal AmountPaid { get; set; }

    }
}
