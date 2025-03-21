namespace CourseProject.Models;

public class History : ScheduleBase
{
    public int HistoryId { get; set; }
    public int AssetID { get; set; } // Foreign Key (Stored in DB)
    public Asset Asset { get; set; } // Navigation Property (Not stored)
    public int ResidentID { get; set; } // Foreign Key (Stored in DB) - Resident who rented
    public Resident Resident { get; set; } // Navigation Property (Not stored)
    public decimal RentPrice { get; set; } // Price for the rental period
}