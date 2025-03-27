using CourseProject.Models;

public class ResidentAssetRequest
{
    public int Id { get; set; }

    public int ResidentId { get; set; }
    public Resident Resident { get; set; }

    public int AssetID { get; set; }
    public Asset Asset { get; set; }

    public DateTime RequestDate { get; set; } = DateTime.Now;
}
