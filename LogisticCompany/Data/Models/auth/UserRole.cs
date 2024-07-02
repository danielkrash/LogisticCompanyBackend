using Microsoft.AspNetCore.Identity;

namespace LogisticCompany.Data.Models.auth;

public class UserRole : IdentityRole<Guid>
{
    public override Guid Id { get; set; }

    public UserRole()
    {
    }

    public UserRole(string roleName) : base(roleName)
    {
    }

    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
}