namespace LogisticCompany.Data.DTOs.Company;

public class AddCompanyDto
{
    public string Name { get; set; }

    public DateOnly? CreationDate { get; set; }

    public string Address { get; set; }
}