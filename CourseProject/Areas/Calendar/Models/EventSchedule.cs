namespace CourseProject.Models;

public class EventSchedule : ScheduleBase
{

    public int EventScheduleId { get; set; }
    public int EmployeeID { get; set; } // Foreign Key (Stored in DB)
    public Employee Employee { get; set; } // Navigation Property (Not stored)

    public int? ServiceID { get; set; } // Foreign Key (Stored in DB)
    public Service Service { get; set; } // Navigation Property (Not stored)
    //public string RangeOfHours { get; set; }
}
