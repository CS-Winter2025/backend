namespace CourseProject.Models;

public class Resident
{
    public int ResidentID { get; set; }
    public string Name { get; set; }
    public List<Invoice> Invoices { get; set; } = new List<Invoice>();
    public string DetailsJson { get; set; } 
}