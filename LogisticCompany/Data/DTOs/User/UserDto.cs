namespace LogisticCompany.Data.DTOs.User;

public class UserDto
{
    public Guid? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public List<string>? Roles { get; set; }
}