namespace CourseProject.Models;

public class File
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public byte[] FileData { get; set; } // Binary data for the file
    public string FileType { get; set; } // e.g., "image/jpeg"
    public int ScheduleBaseID { get; set; }
    public ScheduleBase ScheduleBase { get; set; }

}
