using System.ComponentModel.DataAnnotations;

namespace LogisticCompany.models;

public class Office
{
    public int Id { get; set; }
    public string Address { get; set; }
    [Phone] public string? PhoneNumber { get; set; }
    public int CompanyId { get; set; }
    public Company? Company { get; set; }
    public ICollection<Employee> Employees { get; set; } = new HashSet<Employee>(ReferenceEqualityComparer.Instance);

}