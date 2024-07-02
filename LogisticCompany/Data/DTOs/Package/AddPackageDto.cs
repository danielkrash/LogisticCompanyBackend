namespace LogisticCompany.Data.DTOs.Package;

public class AddPackageDto
{
    public string SenderEmail { get; set; }
    public string ReceiverEmail { get; set; }
    public string DeliveryAddress { get; set; }
    public string toAdress { get; set; }
    public string CompanyName { get; set; }
    public decimal Price { get; set; }
    public decimal Weight { get; set; }
    public string? Description { get; set; }
    public string? Fragile { get; set; }
    public string? Hazardous { get; set; }
}