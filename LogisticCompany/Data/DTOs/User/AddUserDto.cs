namespace LogisticCompany.Data.DTOs.User;

public class AddUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required string Email { get; set; }
    public DateOnly? BirthDate { get; set; }
    public required string Password { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Role { get; set; }
}