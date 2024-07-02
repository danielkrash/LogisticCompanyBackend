using LogisticCompany.Data.Models.auth;
using LogisticCompany.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LogisticCompany.models;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;

namespace LogisticCompany.data;

public partial class LogisticCompanyContext : IdentityDbContext<ApplicationUser, UserRole, Guid,
    IdentityUserClaim<Guid>, ApplicationUserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>,
    IdentityUserToken<Guid>>
{
    public LogisticCompanyContext()
    {
    }

    public LogisticCompanyContext(DbContextOptions<LogisticCompanyContext> options)
        : base(options)
    {
    }

    public DbSet<Company> Companies { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<Position> Positions { get; set; }

    public DbSet<Office> Offices { get; set; }

    public DbSet<CompanyRate> CompanyRates { get; set; }
    
    public DbSet<CompanyRevenue> CompanyRevenues { get; set; }

    public DbSet<Package> Packages { get; set; }

    public DbSet<PackageInfo> PackageInfos { get; set; }
    public DbSet<PackageStatus> PackageStatus { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.UseNpgsql(
        //     $" Host={Environment.GetEnvironmentVariable("HOST")};" +
        //     $" Database={Environment.GetEnvironmentVariable("DATABASE")};" +
        //     $" Username={Environment.GetEnvironmentVariable("USER")};" +
        //     $" Password={Environment.GetEnvironmentVariable("PASSWORD")};" +
        //     $" SearchPath={Environment.GetEnvironmentVariable("SEARCHPATH")};");
        const string connectionString = "Host=localhost;" +
                                        "Database=logistic_company_db;" +
                                        "Username=postgres;" +
                                        "Password=VP4TcpEJmttRfQ7pmDJV;" +
                                        "SEARCHPATH=logistic_company;";
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);
        var searchPaths = connectionStringBuilder.SearchPath?.Split(',');
        optionsBuilder.UseNpgsql(connectionString, o =>
        {
            if (searchPaths is not { Length: > 0 }) return;
            var mainSchema = searchPaths[0];
            o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, mainSchema);
        }).UseValidationCheckConstraints();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Auth models
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            entity.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        });
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(30)
                .HasColumnName("phone_number");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
        });
        // Logistic models
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("company_pk");
            entity.ToTable("company");
            // entity.ToTable("company", "logistic_company" , t => t.ExcludeFromMigrations());
            entity.Property(e => e.Address)
                .HasMaxLength(90)
                .HasColumnName("address");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("creation_date");
            entity.Property(e => e.Name)
                .HasMaxLength(90)
                .HasColumnName("name");
        });
        modelBuilder.Entity<Position>(entity =>
        {
            entity.ToTable("position");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PositionType).HasColumnName("position_type");
            entity.Property(e => e.PositionInfo).HasColumnName("position_info");
            entity.HasIndex(b => b.PositionType).IsUnique();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("employee");
            entity.HasKey(e => e.Id).HasName("employee_pk");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OfficeId).HasColumnName("office_id");
            entity.Property(e => e.HireDate).HasColumnName("hire_date").IsRequired().HasDefaultValueSql("CURRENT_DATE");
            entity.Property(e => e.Salary).HasColumnName("salary").IsRequired().HasColumnType("numeric(12,4)");

            entity.HasOne(e => e.Position)
                .WithMany()
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Office).WithMany(p => p.Employees)
                .HasForeignKey(d => d.OfficeId)
                .HasConstraintName("employee_office_id_fk").OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.User).WithOne(e => e.Employee)
                .HasForeignKey<Employee>(e => e.Id)
                .HasConstraintName("employee_user_id_fk").OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<CompanyRevenue>(entity =>
        {
            entity.ToTable("company_revenue");
            entity.HasKey(c => c.Id).HasName("company_revenue_pk");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id").IsRequired();
            entity.Property(e => e.Revenue).HasColumnName("revenue").HasColumnType("numeric(12,4)");
            entity.Property(e => e.Date).HasColumnName("date").HasDefaultValueSql("CURRENT_DATE");
            entity.HasOne(e => e.Company)
                .WithMany(e => e.CompanyRevenues)
                .HasForeignKey(e => e.CompanyId)
                .HasConstraintName("company_revenue_company_id_fk").OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Office>(entity =>
        {
            entity.ToTable("office");
            entity.HasKey(e => e.Id).HasName("office_pk");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
            entity.Property(e => e.CompanyId).HasColumnName("company_id").IsRequired();
            entity.HasIndex(e => e.Address).IsUnique();
            entity.HasOne(d => d.Company).WithMany(p => p.Offices)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("office_company_id_fk").OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<CompanyRate>(entity =>
        {
            entity.ToTable("company_rate");
            entity.HasKey(cr => cr.CompanyId).HasName("company_rate_pk");
            entity.Property(cr => cr.HomeDeliveryRate).HasColumnName("home_delivery_rate")
                .HasColumnType("numeric(12,4)");
            entity.Property(cr => cr.PackageRatePerGram).HasColumnName("package_rate_per_gram")
                .HasColumnType("numeric(12,4)");
            entity.Property(cr => cr.OfficeDeliveryRate).HasColumnName("office_delivery_rate")
                .HasColumnType("numeric(12,4)");
            entity.Property(cr => cr.HazardousRate).HasColumnName("hazardous_rate").HasColumnType("numeric(12,4)");
            entity.Property(cr => cr.FragileRate).HasColumnName("fragile_rate").HasColumnType("numeric(12,4)");
            entity.HasOne(cr => cr.Company).WithOne(c => c.CompanyRate)
                .HasForeignKey<CompanyRate>(cr => cr.CompanyId)
                .HasConstraintName("company_rate_company_id_fk").OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<PackageInfo>(entity =>
        {
            entity.ToTable("package_info");
            entity.HasKey(t => t.PackageId).HasName("package_info_pk");
            entity.Property(t => t.PackageId).HasColumnName("package_id").HasDefaultValueSql("gen_random_uuid()");
            entity.Property(t => t.Hazardous).HasColumnName("hazardous");
            entity.Property(t => t.Description).HasColumnName("description");
            entity.Property(t => t.Fragile).HasColumnName("fragile");
            entity.Property(t => t.Weight).HasColumnName("weight").HasColumnType("numeric(12,4)");
            entity.HasOne(p => p.Package).WithOne(u => u.PackageInfo)
                .HasForeignKey<PackageInfo>(p => p.PackageId)
                .HasConstraintName("package_package_info_id_fk").OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Package>(entity =>
        {
            entity.ToTable("package");
            entity.Property(p => p.Id).HasColumnName("id");
            entity.Property(p => p.TrackerNumber).HasColumnName("tracker_number");
            entity.Property(p => p.PackageStatusId).HasColumnName("package_status_id");
            entity.Property(p => p.DeliveryDate).HasColumnName("delivery_date");
            entity.Property(p => p.ShippingDate).HasColumnName("shipping_date");
            entity.Property(t => t.DeliveryAddress).HasColumnName("delivery_address");
            entity.Property(p => p.SenderId).HasColumnName("sender_id");
            entity.Property(p => p.ReceiverId).HasColumnName("receiver_id");
            entity.Property(p => p.RegistrarEmployeeId).HasColumnName("registrar_employee_id");
            entity.Property(p => p.CourierId).HasColumnName("courier_id");
            entity.Property(t=> t.toAdress).HasColumnName("to_address");
            entity.Property(t => t.CompanyName).HasColumnName("company_name");
            entity.Property(t => t.price).HasColumnName("price").HasColumnType("numeric(12,4)");
            entity.HasIndex(t => t.TrackerNumber).IsUnique();
            entity.HasOne(p => p.Status)
                .WithMany(s => s.Packages)
                .HasForeignKey(p => p.PackageStatusId)
                .HasConstraintName("package_status_id_fk").OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(p => p.Sender).WithMany(u => u.SendPackages)
                .HasForeignKey(p => p.SenderId)
                .HasConstraintName("package_sender_id_fk").OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Receiver).WithMany(u => u.RecievedPackages)
                .HasForeignKey(p => p.ReceiverId)
                .HasConstraintName("package_receiver_id_fk").OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(p => p.RegistrarEmployee).WithMany(u => u.RegisterPackages)
                .HasForeignKey(p => p.RegistrarEmployeeId)
                .HasConstraintName("package_sender_employee_id_fk").OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(p => p.Courier).WithMany(u => u.DeliveredPackages)
                .HasForeignKey(p => p.CourierId)
                .HasConstraintName("package_courier_id_fk").OnDelete(DeleteBehavior.SetNull);
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}