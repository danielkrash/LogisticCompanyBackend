using System;
using System.Collections.Generic;

namespace LogisticCompany.models;

public partial class Employee
{
    public Guid Id { get; set; }
    public DateOnly HireDate { get; set; }
    public int PositionId { get; set; }
    public int OfficeId { get; set; }
    public decimal Salary { get; set; }
    public Office? Office { get; set; }
    public Position? Position { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public ICollection<Package> RegisterPackages { get; set; } =
        new HashSet<Package>(ReferenceEqualityComparer.Instance);

    public ICollection<Package> DeliveredPackages { get; set; } =
        new HashSet<Package>(ReferenceEqualityComparer.Instance);
}