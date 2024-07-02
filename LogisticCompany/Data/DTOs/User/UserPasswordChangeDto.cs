namespace LogisticCompany.Data.DTOs.User;

public class UserPasswordChangeDto
{
    public required string currentPassword { get; set; }
    public required string newPassword { get; set; }
}