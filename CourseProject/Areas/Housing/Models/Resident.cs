using CourseProject.Models;

public class Resident : Person
{
    public int ResidentId { get; set; } // Primary Key for Resident
    public List<int> ServiceSubscriptionIds { get; set; } = new List<int>(); // Foreign keys for services
    public List<Service> Services { get; set; } = new List<Service>(); // Navigation property for related services
    public bool IsCurrentlyLiving { get; set; } = true;

}
