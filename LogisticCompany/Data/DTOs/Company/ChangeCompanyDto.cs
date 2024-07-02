namespace LogisticCompany.Data.DTOs.Company;

public class ChangeCompanyDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateOnly CreationDate { get; set; }
    public string Address { get; set; }
}