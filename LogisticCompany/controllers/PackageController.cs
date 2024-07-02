using System.Security.Claims;
using Carter;
using LogisticCompany.data;
using LogisticCompany.Data.DTOs;
using LogisticCompany.Data.DTOs.Company;
using LogisticCompany.Data.DTOs.Package;
using LogisticCompany.models;
using LogisticCompany.models.auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NanoidDotNet;

namespace LogisticCompany.controllers;

public class PackageController : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("package").WithOpenApi().WithTags("package");
        group.MapGet("/", GetPackages).WithName("GetPackages").RequireAuthorization();
        group.MapPost("/", AddPackage).WithName("AddPackages").RequireAuthorization();
        group.MapPut("/", ChangePackages).WithName("ChangePackages").RequireAuthorization();
        group.MapDelete("/", DeletePackages).WithName("DeletePackages").RequireAuthorization();
        group.MapGet("{id}", GetPackagesById).WithName("GetPackagesById");
        group.MapDelete("{id}", DeletePackageById).WithName("DeletePackagesById").RequireAuthorization();
        group.MapPut("{id}", ChangePackageById).WithName("ChangePackagesById").RequireAuthorization();
        group.MapGet("track/{id}", GetTrackPackagesById).WithName("GetTrackPackagesById");
        group.MapDelete("track/{id}", DeleteTrackPackageById).WithName("DeleteTrackPackagesById").RequireAuthorization();
        group.MapPut("track/{id}", ChangeTrackPackageById).WithName("ChangeTrackPackagesById").RequireAuthorization();
        group.MapGet("/price", GetPackagePrice).WithName("GetPackagePrice");
        group.MapGet("/user/{id}", GetPackagesByUser).WithName("GetPackagesByUser");
        group.MapGet("/status", GetPackageStatuses).WithName("GetPackagesStatuses").RequireAuthorization();
        group.MapPut("{id}/register", RegisterPackageById).WithName("RegisterPackageById").RequireAuthorization();
    }

    private static async Task<Ok<List<GetPackagesDto>>> GetPackages(string? registered , LogisticCompanyContext context , ClaimsPrincipal claimsPrincipal , UserManager<ApplicationUser> userManager)
    {
        if (registered == "false")
        {
            var user = await userManager.GetUserAsync(claimsPrincipal);
            var userCompany = await context.ApplicationUsers.Where(e => e.Id == user.Id)
                .Select(e => e.Employee.Office.Company.Name).FirstOrDefaultAsync();
            var companies = await context.Packages
                .Where(e => e.PackageStatusId == 3)
                .Where(e => e.CompanyName == userCompany)
                .Select(c => new GetPackagesDto()
                {
                    Id = c.Id.ToString(),
                    TrackingNumber = c.TrackerNumber,
                    Status = c.Status.Status,
                    CompanyName = c.CompanyName,
                    DeliveryAddress = c.DeliveryAddress,
                    DeliveryDate = c.DeliveryDate,
                    ShippingDate = c.ShippingDate,
                    toAdress = c.toAdress,
                    CourierId = c.CourierId.ToString(),
                    ReceiverEmail = c.Receiver.Email,
                    SenderEmail = c.Sender.Email,
                    RegistarEmployeeEmail = c.RegistrarEmployee.User.Email,
                    price = c.price,
                    PackageInfo = new PackageInfoDto
                    {
                        Weight = c.PackageInfo.Weight,
                        Description = c.PackageInfo.Description,
                        Fragile = c.PackageInfo.Fragile,
                        Hazardous = c.PackageInfo.Hazardous
                    }
                }).ToListAsync();
            return TypedResults.Ok(companies);
        }
        else
        {
            var companies = await context.Packages
                .Select(c => new GetPackagesDto()
                {
                    Id = c.Id.ToString(),
                    TrackingNumber = c.TrackerNumber,
                    Status = c.Status.Status,
                    CompanyName = c.CompanyName,
                    DeliveryAddress = c.DeliveryAddress,
                    DeliveryDate = c.DeliveryDate,
                    ShippingDate = c.ShippingDate,
                    toAdress = c.toAdress,
                    CourierId = c.CourierId.ToString(),
                    ReceiverEmail = c.Receiver.Email,
                    SenderEmail = c.Sender.Email,
                    RegistarEmployeeEmail = c.RegistrarEmployee.User.Email,
                    price = c.price,
                    PackageInfo = new PackageInfoDto
                    {
                        Weight = c.PackageInfo.Weight,
                        Description = c.PackageInfo.Description,
                        Fragile = c.PackageInfo.Fragile,
                        Hazardous = c.PackageInfo.Hazardous
                    }
                }).ToListAsync();
            return TypedResults.Ok(companies);
        }
    }
    private static async Task<Results<Ok<List<GetPackagesDto>>,ProblemHttpResult>> GetPackagesByUser(string id , LogisticCompanyContext context )
    {
        if (Guid.TryParse(id, out var userId) is false)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Invalid user id",
                Detail = "User id is not a valid GUID",
                Status = StatusCodes.Status400BadRequest
            });
        var companies = await context.Packages
            .Where(e => e.Sender.Id == userId || e.Receiver.Id == userId)
            .Select(c => new GetPackagesDto()
            {
                Id = c.Id.ToString(),
                TrackingNumber = c.TrackerNumber,
                Status = c.Status.Status,
                DeliveryAddress = c.DeliveryAddress,
                DeliveryDate = c.DeliveryDate,
                ShippingDate = c.ShippingDate,
                toAdress = c.toAdress,
                CourierId = c.CourierId.ToString(),
                ReceiverEmail = c.Receiver.Email,
                SenderEmail = c.Sender.Email,
                RegistarEmployeeEmail = c.RegistrarEmployee.User.Email,
                price = c.price,
                PackageInfo = new PackageInfoDto
                {
                    Weight = c.PackageInfo.Weight,
                    Description = c.PackageInfo.Description,
                    Fragile = c.PackageInfo.Fragile,
                    Hazardous = c.PackageInfo.Hazardous
                }
            }).Distinct().ToListAsync();
        return TypedResults.Ok(companies);
    }

    private static async Task<Ok> ChangePackages([FromBody] ChangePackageDto[] changePackageDtos,
        LogisticCompanyContext context
    )
    {
        foreach (var packageDto in changePackageDtos)
        {
            var package = await context.Packages.FindAsync(new Guid(packageDto.id));
            if (package != null)
            {
                if (packageDto.Address != null)
                {
                    package.DeliveryAddress = packageDto.Address;
                }

                if (packageDto.Status != null)
                {
                    package.PackageStatusId = await context.PackageStatus
                        .Where(e => e.Status == packageDto.Status).Select(e => e.Id).FirstOrDefaultAsync();
                }
                if (packageDto.toAdress != null)
                {
                    package.toAdress = bool.Parse(packageDto.toAdress);
                }
            }
        }

        await context.SaveChangesAsync();
        return TypedResults.Ok();
    }

    private static async Task<Ok> ChangePackageById(string id, [FromBody] ChangePackageDto packageDto,
        LogisticCompanyContext context
    )
    {
        var package = await context.Packages.FindAsync(new Guid(id));
        if (package != null)
        {
            if (packageDto.Address != null)
            {
                package.DeliveryAddress = packageDto.Address;
            }
            if (packageDto.Status != null)
            {
                package.PackageStatusId = await context.PackageStatus
                    .Where(e => e.Status == packageDto.Status).Select(e => e.Id).FirstOrDefaultAsync();
            }
            if (packageDto.toAdress != null)
            {
                package.toAdress = bool.Parse(packageDto.toAdress);
            }
        }

        await context.SaveChangesAsync();
        return TypedResults.Ok();
    }
    private static async Task<Ok> RegisterPackageById(string id, LogisticCompanyContext context ,
        ClaimsPrincipal claimsPrincipal , UserManager<ApplicationUser> userManager
    )
    {
        var package = await context.Packages.FindAsync(new Guid(id));
        var user = await userManager.GetUserAsync(claimsPrincipal);
        package.PackageStatusId = await context.PackageStatus
            .Where(e => e.Status == "wait courier").Select(e => e.Id).FirstOrDefaultAsync();
        package.RegistrarEmployeeId = user.Id;
        await context.SaveChangesAsync();
        return TypedResults.Ok();
    }
    private static async Task<Ok> ChangeTrackPackageById(string id, [FromBody] ChangePackageDto packageDto,
        LogisticCompanyContext _context
    )
    {
        var package = await _context.Packages.Where(e => e.TrackerNumber == id).FirstOrDefaultAsync();
        if (package != null)
        {
            if (packageDto.Address != null)
            {
                package.DeliveryAddress = packageDto.Address;
            }
            if (packageDto.Status != null)
            {
                package.PackageStatusId = await _context.PackageStatus
                    .Where(e => e.Status == packageDto.Status).Select(e => e.Id).FirstOrDefaultAsync();
            }
            if (packageDto.toAdress != null)
            {
                package.toAdress = bool.Parse(packageDto.toAdress);
            }
        }

        await _context.SaveChangesAsync();
        return TypedResults.Ok();
    }

    private static async Task<NoContent> DeletePackages(LogisticCompanyContext context)
    {
        var companies = await context.Packages.ExecuteDeleteAsync();
        return TypedResults.NoContent();
    }

    private static async Task<NoContent> DeletePackageById(string id, LogisticCompanyContext context)
    {
        var companies = await context.Packages.Where(e => e.Id == new Guid(id)).ExecuteDeleteAsync();
        return TypedResults.NoContent();
    }
    private static async Task<NoContent> DeleteTrackPackageById(string id, LogisticCompanyContext context)
    {
        var companies = await context.Packages.Where(e => e.TrackerNumber == id).ExecuteDeleteAsync();
        return TypedResults.NoContent();
    }

    private static async Task<Ok<GetPackagesDto>> GetPackagesById(string id, LogisticCompanyContext context)
    {
        var companies = await context.Packages
            .Where(e => e.Id == new Guid(id))
            .Select(c => new GetPackagesDto()
            {
                Id = c.Id.ToString(),
                TrackingNumber = c.TrackerNumber,
                Status = c.Status.Status,
                DeliveryAddress = c.DeliveryAddress,
                DeliveryDate = c.DeliveryDate,
                ShippingDate = c.ShippingDate,
                toAdress = c.toAdress,
                CourierId = c.CourierId.ToString(),
                ReceiverEmail = c.Receiver.Email,
                SenderEmail = c.Sender.Email,
                RegistarEmployeeEmail = c.RegistrarEmployee.User.Email,
                price = c.price,
                PackageInfo = new PackageInfoDto
                {
                    Weight = c.PackageInfo.Weight,
                    Description = c.PackageInfo.Description,
                    Fragile = c.PackageInfo.Fragile,
                    Hazardous = c.PackageInfo.Hazardous
                }
            }).FirstOrDefaultAsync();
        return TypedResults.Ok(companies);
    }
    private static async Task<Ok<GetPackagesDto>> GetTrackPackagesById(string id, LogisticCompanyContext context)
    {
        var companies = await context.Packages
            .Where(e => e.TrackerNumber == id)
            .Select(c => new GetPackagesDto()
            {
                Id = c.Id.ToString(),
                TrackingNumber = c.TrackerNumber,
                CompanyName = c.CompanyName,
                Status = c.Status.Status,
                DeliveryAddress = c.DeliveryAddress,
                DeliveryDate = c.DeliveryDate,
                ShippingDate = c.ShippingDate,
                toAdress = c.toAdress,
                CourierId = c.CourierId.ToString(),
                ReceiverEmail = c.Receiver.Email,
                SenderEmail = c.Sender.Email,
                RegistarEmployeeEmail = c.RegistrarEmployee.User.Email,
                price = c.price,
                PackageInfo = new PackageInfoDto
                {
                    Weight = c.PackageInfo.Weight,
                    Description = c.PackageInfo.Description,
                    Fragile = c.PackageInfo.Fragile,
                    Hazardous = c.PackageInfo.Hazardous
                }
            }).FirstOrDefaultAsync();
        return TypedResults.Ok(companies);
    }

    private static async Task<Results<ProblemHttpResult, Created<AddPackageDto>>> AddPackage(
        [FromBody] AddPackageDto addPackageDto, LogisticCompanyContext _context
    )
    {
        
        var newPackage = new Package()
        {
            Id = Guid.NewGuid(),
            SenderId = await _context.ApplicationUsers.Where(e => e.Email == addPackageDto.SenderEmail).Select(e => e.Id).FirstOrDefaultAsync(),
            ReceiverId = await _context.ApplicationUsers.Where(e => e.Email == addPackageDto.ReceiverEmail).Select(e => e.Id).FirstOrDefaultAsync(),
            DeliveryAddress = addPackageDto.DeliveryAddress,
            toAdress = bool.Parse(addPackageDto.toAdress),
            CompanyName = addPackageDto.CompanyName,
            PackageStatusId = await _context.PackageStatus.Where(e => e.Status == "on hold").Select(e => e.Id).FirstOrDefaultAsync(),
            TrackerNumber = await Nanoid.GenerateAsync(Nanoid.Alphabets.UppercaseLettersAndDigits,12)
        };
        var newPackageInfo = new PackageInfo()
        {
            PackageId = newPackage.Id,
            Weight = addPackageDto.Weight,
            Description = addPackageDto.Description,
            Fragile = bool.Parse(addPackageDto.Fragile),
            Hazardous = bool.Parse(addPackageDto.Hazardous)
        };
        var packagePrice = await _context.Companies.Where(e => e.Name == addPackageDto.CompanyName)
            .Include(e => e.CompanyRate).Select(e => e.CompanyRate.PackageRatePerGram).FirstOrDefaultAsync();
        var officeDeliveryPrice = await _context.Companies.Where(e => e.Name == addPackageDto.CompanyName)
                .Include(e => e.CompanyRate).Select(e => e.CompanyRate.OfficeDeliveryRate).FirstOrDefaultAsync();
        var homeDeliveryPrice = await _context.Companies.Where(e => e.Name == addPackageDto.CompanyName)
            .Include(e => e.CompanyRate).Select(e => e.CompanyRate.HomeDeliveryRate).FirstOrDefaultAsync();
        var hazardousPrice = await _context.Companies.Where(e => e.Name == addPackageDto.CompanyName)
            .Include(e => e.CompanyRate).Select(e => e.CompanyRate.HazardousRate).FirstOrDefaultAsync();
        var fragilePrice = await _context.Companies.Where(e => e.Name == addPackageDto.CompanyName)
            .Include(e => e.CompanyRate).Select(e => e.CompanyRate.FragileRate).FirstOrDefaultAsync();
        if (addPackageDto.Price == null || addPackageDto.Price == 0)
        {
            newPackage.price = addPackageDto.Weight * packagePrice + (bool.Parse(addPackageDto.toAdress) ? officeDeliveryPrice : homeDeliveryPrice) + (bool.Parse(addPackageDto.Hazardous) ? hazardousPrice : 0) + (bool.Parse(addPackageDto.Fragile) ? fragilePrice : 0);
        }
        else
        {
            newPackage.price = addPackageDto.Price;
        }
        await _context.Packages.AddAsync(newPackage);
        await _context.PackageInfos.AddAsync(newPackageInfo);
        await _context.SaveChangesAsync();
        return TypedResults.Created($"//{newPackage.Id}", addPackageDto);
    }
    

    private static async Task<Ok<decimal>> GetPackagePrice(string company, string toAdress , string weight , string fragile , string hazardous ,  LogisticCompanyContext _context)
    {
        var packagePrice = await _context.Companies.Where(e => e.Name == company)
            .Include(e => e.CompanyRate).Select(e => e.CompanyRate.PackageRatePerGram).FirstOrDefaultAsync();
        var officeDeliveryPrice = await _context.Companies.Where(e => e.Name == company)
                .Include(e => e.CompanyRate).Select(e => e.CompanyRate.OfficeDeliveryRate).FirstOrDefaultAsync();
        var homeDeliveryPrice = await _context.Companies.Where(e => e.Name == company)
            .Include(e => e.CompanyRate).Select(e => e.CompanyRate.HomeDeliveryRate).FirstOrDefaultAsync();
        var hazardousPrice = await _context.Companies.Where(e => e.Name == company)
            .Include(e => e.CompanyRate).Select(e => e.CompanyRate.HazardousRate).FirstOrDefaultAsync();
        var fragilePrice = await _context.Companies.Where(e => e.Name == company)
            .Include(e => e.CompanyRate).Select(e => e.CompanyRate.FragileRate).FirstOrDefaultAsync();
        var finalPrice = Decimal.Parse(weight) * packagePrice + (bool.Parse(toAdress) ? officeDeliveryPrice : homeDeliveryPrice) + (bool.Parse(fragile) ? fragilePrice : 0) + (bool.Parse(hazardous) ? hazardousPrice : 0);
        return TypedResults.Ok(finalPrice);
    }
    private static async Task<Ok<List<PackageStatusDto>>> GetPackageStatuses(LogisticCompanyContext _context)
    {
        var statuses = await _context.PackageStatus.Select(e => new PackageStatusDto
        {
            Id = e.Id,
            Status = e.Status
        }).ToListAsync();
        return TypedResults.Ok(statuses);
    }
    
}