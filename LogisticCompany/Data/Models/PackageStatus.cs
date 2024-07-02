namespace LogisticCompany.models;

public class PackageStatus
{
    public int Id { get; set; }
    public string Status { get; set; }
    public ICollection<Package> Packages { get; set; } =
        new HashSet<Package>(ReferenceEqualityComparer.Instance);
}