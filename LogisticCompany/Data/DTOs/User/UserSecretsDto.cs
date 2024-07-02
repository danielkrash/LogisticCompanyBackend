namespace LogisticCompany.Data.DTOs.User;

public class UserSecretsDto
{
    public required string Id { get; set; }
    public required string email { get; set; }
    public required int receivedPackageCount { get; set; }
    public required int sentPackageCount { get; set; }
}