namespace CourseProject.Models;

public class Event
{
    public int EventID { get; set; }
    public int EmployeeID { get; set; }
    public int? ServiceID { get; set; } 
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public bool IsRepeated { get; set; }
    public string RepeatPattern { get; set; }
  
}