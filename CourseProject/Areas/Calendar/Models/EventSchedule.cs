namespace CourseProject.Models;

public class EventSchedule : ScheduleBase
{

    public int EventScheduleId { get; set; }
    public List<Employee> Employees { get; set; } // Navigation Property (Not stored)
    public int? ResidentId { get; set; }
    public Resident? Resident { get; set; }
    public int? ServiceID { get; set; } // Foreign Key (Stored in DB)
    public Service Service { get; set; } // Navigation Property (Not stored)
    //public string RangeOfHours { get; set; }

    //public int EventPID { get; set; }
    //public string? RecType { get; set; }
    //public long EventLength { get; set; }
}
