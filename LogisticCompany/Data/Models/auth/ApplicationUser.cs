using LogisticCompany.Data.Models.auth;
using Microsoft.AspNetCore.Identity;

namespace LogisticCompany.models;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? Address { get; set; }
    public Employee? Employee { get; set; }
    public ICollection<Package> SendPackages { get; set; } = new HashSet<Package>(ReferenceEqualityComparer.Instance);

    public ICollection<Package> RecievedPackages { get; set; } =
        new HashSet<Package>(ReferenceEqualityComparer.Instance);

    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
}