namespace CourseProject.Models
{
    public class Asset
    {
        public int AssetID { get; set; }
        public string Type { get; set; }
        public int? ResidentID { get; set; }
        public string DetailsJson { get; set; }
    }
}
