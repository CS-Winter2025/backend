using System.ComponentModel.DataAnnotations;

namespace CourseProject.Models
{
    public class Service
    {
        public int ServiceID { get; set; }
        public string Type { get; set; }
        public decimal Rate { get; set; }
        public List<string> Requirements { get; set; } = new List<string>();

        // Navigation property for Employees associated with this service
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
