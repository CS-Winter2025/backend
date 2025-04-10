using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;

namespace CourseProject.Models;

public class EventSchedule : ScheduleBase
{
    //public new int EventScheduleId { get => base.ScheduleBaseId; set => base.ScheduleBaseId = value; }
    public List<Employee> Employees { get; set; } // Navigation Property (Not stored)
    public int? ResidentId { get; set; }
    public Resident? Resident { get; set; }
    public int? ServiceID { get; set; } // Foreign Key (Stored in DB)
    public Service Service { get; set; } // Navigation Property (Not stored)
    public string? Status { get; set; }
}
