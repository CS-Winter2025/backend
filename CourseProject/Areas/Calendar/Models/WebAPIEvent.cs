namespace CourseProject.Models
{
    public class WebAPIEvent
    {
        public int id { get; set; }
        public string? text { get; set; }
        public string? start_date { get; set; }
        public string? end_date { get; set; }
        public string? rrule { get; set; }

        public static explicit operator WebAPIEvent(EventSchedule ev)
        {
            return new WebAPIEvent
            {
                id = ev.EventScheduleId,
                text = ev.Service.Type,
                start_date = ev.StartDate.ToString("yyyy-MM-dd HH:mm"),
                end_date = ev.EndDate.ToString("yyyy-MM-dd HH:mm"),
                rrule = ev.RepeatPattern
            };
        }

        public static explicit operator EventSchedule(WebAPIEvent ev)
        {
            return new EventSchedule
            {
                EventScheduleId = ev.id,
                ServiceID = 1,
                //Service.Type = ev.text,
                EmployeeID = 1,
                StartDate = ev.start_date != null ? DateTime.Parse(ev.start_date,
                  System.Globalization.CultureInfo.InvariantCulture) : new DateTime(),
                EndDate = ev.end_date != null ? DateTime.Parse(ev.end_date,
                  System.Globalization.CultureInfo.InvariantCulture) : new DateTime(),
                RepeatPattern = ev.rrule
            };
        }
    }
}