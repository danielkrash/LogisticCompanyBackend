namespace LogisticCompany.Data.DTOs.Employee;

public class ChangeEmployeeDto
{
    public string Id { get; set; }
    public required decimal Salary { get; set; }
    public required int PositionId { get; set; }
    public required int OfficeId { get; set; }
}