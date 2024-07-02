namespace LogisticCompany.Data.DTOs.Package;

public class GetPackagesDto
{
    public string Id { get; set; }
    public string TrackingNumber { get; set; }
    public string Status { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public DateTime? ShippingDate { get; set; }
    public string SenderEmail { get; set; }
    public string ReceiverEmail { get; set; }
    public string CompanyName { get; set; }
    public string? RegistarEmployeeEmail { get; set; }
    public string? CourierId { get; set; }
    public string DeliveryAddress { get; set; }
    public bool toAdress { get; set; }
    public decimal price { get; set; }
    public PackageInfoDto PackageInfo { get; set; }
}