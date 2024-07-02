namespace LogisticCompany.Data.DTOs.Employee;

public class EmployeeDto
{
    public Guid? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public List<string>? Roles { get; set; }
    public DateOnly HireDate { get; set; }
    public string? Position { get; set; }
    public int OfficeId { get; set; }
    public decimal Salary { get; set; }
}