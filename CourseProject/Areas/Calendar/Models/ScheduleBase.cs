namespace CourseProject.Models;

public abstract class ScheduleBase
{
    public int ScheduleBaseId { get; set; } // Ensures a primary key for EF
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? RepeatPattern { get; set; }
    public int? Duration { get; set; }
    public int? RecurringEventId { get; set; }
    public DateTime OriginalStart { get; set; }
    public bool? Deleted { get; set; }
    public List<File>? Attachments { get; set; }
}

