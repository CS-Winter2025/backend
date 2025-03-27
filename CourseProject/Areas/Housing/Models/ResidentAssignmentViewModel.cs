using CourseProject.Models;

public class ResidentAssignmentViewModel
{
    public int ResidentId { get; set; }
    public bool IsSelected { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}
