namespace CourseProject.Models
{
    public class Employee : Person
    {
        public int EmployeeId { get; set; } // Primary Key

        public int? ManagerId { get; set; }  // Foreign Key (Stored in DB)
        public Employee Manager { get; set; }  // Navigation Property (Not stored, for C# usage)

        public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();  // Navigation Property (Not stored)

        public string JobTitle { get; set; }
        public string EmploymentType { get; set; }
        public decimal PayRate { get; set; }

        public List<int> Availability { get; set; } = new List<int>();
        public List<int> HoursWorked { get; set; } = new List<int>();
        public List<string> Certifications { get; set; } = new List<string>();

        public int OrganizationId { get; set; }  // Foreign Key (Stored in DB)
        public Organization Organization { get; set; }  // Navigation Property (Not stored)

        // Navigation property for Services associated with this employee
        public List<Service> Services { get; set; } = new List<Service>();
    }
}
