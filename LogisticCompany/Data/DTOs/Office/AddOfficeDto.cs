namespace LogisticCompany.Data.DTOs.Office;

public class AddOfficeDto
{
    public required string Address { get; set; }
    public string? PhoneNumber { get; set; }
    public required string CompanyName { get; set; }
}