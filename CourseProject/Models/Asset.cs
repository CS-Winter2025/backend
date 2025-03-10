using System.Data.Entity;

namespace CourseProject.Models
{
    public class Asset
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public bool Available { get; set; }
        public long ResidentID { get; set; }
        public string Details { get; set; } // Is a JSON 
    }
}
