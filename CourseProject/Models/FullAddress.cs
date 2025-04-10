namespace CourseProject.Models;

public class FullAddress
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string ZipCode { get; set; }
    public override string ToString() =>
        $"{Street}, {City}, {State} {ZipCode}, {Country} ";
}