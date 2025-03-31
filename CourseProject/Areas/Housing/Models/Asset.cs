using CourseProject.Models;

public class Asset : RootObj
{
    public int AssetID { get; set; }
    public string Type { get; set; }
    public string Status { get; set; } = "Available";

}
