namespace LogisticCompany.models;

public class CompanyRevenue
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public decimal Revenue { get; set; }
    public DateOnly Date { get; set; }
    public Company Company { get; set; } = null!;
}