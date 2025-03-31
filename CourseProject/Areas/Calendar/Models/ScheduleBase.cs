namespace CourseProject.Models;

public abstract class ScheduleBase
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? RepeatPattern { get; set; }
    public int? Duration { get; set; }
    public int? RecurringEventId { get; set; }
    public DateTime OriginalStart { get; set; }
    public bool? Deleted { get; set; }
    //public List<File>? Attachments { get; set; }
}
//public class File
//{
//    public string FileName { get; set; }
//    public byte[] FileData { get; set; } // Binary data for the file
//    public string FileType { get; set; } // e.g., "image/jpeg"
//}
