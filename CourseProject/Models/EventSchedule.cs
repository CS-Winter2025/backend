namespace CourseProject.Models
{
    public class EventSchedule
    {
        public int EventEventScheduleId { get; set; }
        public int EmployeeID { get; set; } // Foreign Key (Stored in DB)
        public Employee Employee { get; set; } // Navigation Property (Not stored)

        public int? ServiceID { get; set; } // Foreign Key (Stored in DB)
        public Service Service { get; set; } // Navigation Property (Not stored)

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string RangeOfHours { get; set; }
        public string? RepeatPattern { get; set; }
    }
}
