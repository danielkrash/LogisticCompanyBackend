namespace LogisticCompany.models;

public class PackageInfo
{
    public Guid PackageId { get; set; }
    public decimal Weight { get; set; }
    public string? Description { get; set; }
    public bool? Fragile { get; set; }
    public bool? Hazardous { get; set; }
    public Package? Package { get; set; }
}