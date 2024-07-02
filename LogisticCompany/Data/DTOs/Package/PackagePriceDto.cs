namespace LogisticCompany.Data.DTOs.Package;

public class PackagePriceDto
{
    public string CompanyName { get; set; }
    public bool ToAddress { get; set; }
    public decimal Weight { get; set; }
    public string? Fragile { get; set; }
    public string? Hazardous { get; set; }
}