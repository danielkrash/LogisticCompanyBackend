namespace LogisticCompany.models;

public class Package
{
    public Guid Id { get; set; }
    public string? TrackerNumber { get; set; }
    public int PackageStatusId { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public DateTime? ShippingDate { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public Guid? RegistrarEmployeeId { get; set; }
    public Guid? CourierId { get; set; }
    public decimal price { get; set; }
    
    public string CompanyName { get; set; }
    public virtual Employee? RegistrarEmployee { get; set; }
    public virtual PackageInfo PackageInfo { get; set; }
    public string DeliveryAddress { get; set; }
    
    public bool toAdress { get; set; }
    public PackageStatus? Status { get; set; }
    public Employee? Courier { get; set; }
    public ApplicationUser? Sender { get; set; }
    public ApplicationUser? Receiver { get; set; }
}