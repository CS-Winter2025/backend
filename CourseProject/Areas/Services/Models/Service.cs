namespace CourseProject.Models;

public class Service
{
    public int ServiceID { get; set; }
    public String ServiceType { get; set; } 
    public List<Employee> Employees { get; set; } = new List<Employee>(); 
    public decimal Rate { get; set; }
    public List<string> Requirements { get; set; } = new List<string>();

}