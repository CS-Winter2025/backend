namespace CourseProject.Models
{
    public class WebAPIEvent
    {
        public int id { get; set; }
        public string? text { get; set; }
        public string? start_date { get; set; }
        public string? end_date { get; set; }
        public string? rrule { get; set; }
        public int? duration { get; set; }
        public int? recurring_event_id { get; set; }
        public string? original_start { get; set; }
        public bool? deleted { get; set; }
        public int? service_id { get; set; }
        public int? resident_id { get; set; }
        public string? employee_ids { get; set; }

        public static explicit operator WebAPIEvent(EventSchedule ev)
        {
            return new WebAPIEvent
            {
                id = ev.EventScheduleId,
                text = ev.Service.Type,
                start_date = ev.StartDate.ToString("yyyy-MM-dd HH:mm"),
                end_date = ev.EndDate.ToString("yyyy-MM-dd HH:mm"),
                rrule = ev.RepeatPattern,
                duration = ev.Duration,
                recurring_event_id = ev.RecurringEventId,
                original_start = ev.OriginalStart.ToString("yyyy-MM-dd HH:mm"),
                deleted = ev.Deleted,
                //employee_ids = ev.EmployeeID.ToString(),
                service_id = ev.ServiceID,
                resident_id = ev.ResidentId
            };
        }

        public static explicit operator EventSchedule(WebAPIEvent ev)
        {
            return new EventSchedule
            {
                EventScheduleId = ev.id,
                ServiceID = ev.service_id,
                ResidentId = ev.resident_id,
                //Service.Type = ev.text,
                //EmployeeID = 1,
                StartDate = ev.start_date != null ? DateTime.Parse(ev.start_date,
                  System.Globalization.CultureInfo.InvariantCulture) : new DateTime(),
                EndDate = ev.end_date != null ? DateTime.Parse(ev.end_date,
                  System.Globalization.CultureInfo.InvariantCulture) : new DateTime(),
                RepeatPattern = ev.rrule,
                Duration = ev.duration,
                RecurringEventId = ev.recurring_event_id,
                OriginalStart = ev.original_start != null ? DateTime.Parse(ev.original_start,
                  System.Globalization.CultureInfo.InvariantCulture) : new DateTime(),
                Deleted = ev.deleted
            };
        }
    }
}