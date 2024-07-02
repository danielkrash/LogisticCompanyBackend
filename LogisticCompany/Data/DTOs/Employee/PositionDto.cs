namespace LogisticCompany.Data.DTOs.Employee;

public class PositionDto
{
    public int Id { get; set; }
    public required string Type { get; set; }
    public required string Description { get; set; }
}