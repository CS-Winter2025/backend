namespace CourseProject.Models;

public abstract class ScheduleBase
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? RepeatPattern { get; set; } 
}