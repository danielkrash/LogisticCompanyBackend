namespace LogisticCompany.models;

public class CompanyRate
{
    public int CompanyId { get; set; }
    public decimal PackageRatePerGram { get; set; }
    public decimal OfficeDeliveryRate { get; set; }
    public decimal HomeDeliveryRate { get; set; }
    public decimal HazardousRate { get; set; }
    public decimal FragileRate { get; set; }
    public Company Company { get; set; } = null!;
}