namespace LogisticCompany.Data.DTOs.User;

public class AdminSecretsDto
{
    public required string email { get; set; }
    public required List<string> userRole { get; set; }
    public required int companyCount { get; set; }
    public required int userCount { get; set; }
    public required int roleCount { get; set; }
    public required int employeeCount { get; set; }
    public required int managerCount { get; set; }
    public required int packagesCount { get; set; }
}