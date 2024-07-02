namespace LogisticCompany.Data.DTOs.Company;

public class CompanyGetDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string Address { get; set; }
    public DateOnly CreationDate { get; set; }
}