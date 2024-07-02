using LogisticCompany.models;
using Microsoft.AspNetCore.Identity;

namespace LogisticCompany.Data.Models.auth;

public class ApplicationUserRole : IdentityUserRole<Guid>
{
    public virtual ApplicationUser User { get; set; }
    public virtual UserRole Role { get; set; }
}