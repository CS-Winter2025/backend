using System;
using System.Collections.Generic;
using System.Text.Json;


namespace CourseProject.Models;


public class Employee
{
    public int EmployeeID { get; set; }
    public int? ManagerID { get; set; }  
    public string Name { get; set; }
    public List<Event> Availability { get; set; } = new List<Event>(); 
    public List<string> Certifications { get; set; } = new List<string>(); 
    public String JobTitle { get; set; }
    public String  EmploymentType { get; set; } 
    public decimal PayRate { get; set; }
    public List<Event> HoursWorked { get; set; } = new List<Event>(); 
    public Employee Manager { get; set; } 
    public string DetailsJson { get; set; }
}