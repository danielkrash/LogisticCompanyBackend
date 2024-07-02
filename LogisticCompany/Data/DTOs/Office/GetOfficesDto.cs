namespace LogisticCompany.Data.DTOs.Office;

public class GetOfficesDto
{
    public required int Id { get; set; }
    public required string Address { get; set; }
    public string? PhoneNumber { get; set; }
    public required int CompanyId { get; set; }
    public required string CompanyName { get; set; }
}