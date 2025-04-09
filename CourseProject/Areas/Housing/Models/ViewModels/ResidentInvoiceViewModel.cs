public class ResidentInvoiceViewModel
{
    public Resident Resident { get; set; }
    public List<InvoiceItemViewModel> Items { get; set; } = new();
    public decimal TotalDue { get; set; }
}
