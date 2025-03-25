namespace CourseProject.Models;
public class Service
{
    public int ServiceID { get; set; }
    public string Type { get; set; }
    public decimal Rate { get; set; }
    public List<string> Requirements { get; set; } = new List<string>();
    public List<Employee> Employees { get; set; } = new List<Employee>();    // Navigation property for Employees associated with this service

}
