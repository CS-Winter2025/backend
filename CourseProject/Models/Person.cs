namespace CourseProject.Models;

public class Person : RootObj
{
    //public string Name { get; set; }

    public FullName Name { get; set; }

    public FullAddress Address { get; set; }
    public List<EventSchedule> EventSchedules { get; set; } = new List<EventSchedule>();
}