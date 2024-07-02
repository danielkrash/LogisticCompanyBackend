using System;
using System.Collections.Generic;

namespace LogisticCompany.models;

public partial class Company
{
    public int Id { get; set; }

    public string Name { get; set; }

    public DateOnly CreationDate { get; set; }

    public string Address { get; set; }

    public ICollection<Office> Offices { get; set; } = new List<Office>();
    public ICollection<CompanyRevenue> CompanyRevenues { get; set; }
    public CompanyRate? CompanyRate { get; set; }

}