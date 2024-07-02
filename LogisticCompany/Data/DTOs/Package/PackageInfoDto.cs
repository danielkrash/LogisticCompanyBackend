namespace LogisticCompany.Data.DTOs.Package;

public class PackageInfoDto
{
    public decimal Weight { get; set; }
    public string? Description { get; set; }
    public bool? Fragile { get; set; }
    public bool? Hazardous { get; set; }
}