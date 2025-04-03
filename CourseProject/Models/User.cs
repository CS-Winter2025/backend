using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Models
{    
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public int? ResidentId { get; set; }
        public Resident Resident { get; set; }
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }        
    }

    public class UserView
    {
        public int Id { get; set; }
        public string Username { get; set; }        
        public UserRole Role { get; set; }
    }
}
