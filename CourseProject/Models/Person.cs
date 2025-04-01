namespace CourseProject.Models;

public class Person : RootObj
{
    public FullName Name { get; set; }
    public FullAddress Address { get; set; }
    public byte[]? ProfilePicture { get; set; }
    public List<EventSchedule> EventSchedules { get; set; } = new List<EventSchedule>();
}