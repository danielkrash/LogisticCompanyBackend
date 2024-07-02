namespace LogisticCompany.Data.DTOs.User;

public class ManagerSecretsDto
{
    public required string email { get; set; }
    public required string companyName { get; set; }
    public required int officeCount { get; set; }
    public required int packageSentCount { get; set; }
    public required int packageRecievedCount { get; set; }
    public required decimal salary { get; set; }
}