namespace CourseProject.Models;

public class Organization : RootObj
{
    public int OrganizationId { get; set; }
    public List<Employee> Employees { get; set; } = new List<Employee>();
    public List<Service> Services { get; set; } = new List<Service>();
    
}