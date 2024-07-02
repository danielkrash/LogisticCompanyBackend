using Carter;
using LogisticCompany.data;
using LogisticCompany.Data.DTOs;
using LogisticCompany.Data.DTOs.Company;
using LogisticCompany.Data.DTOs.User;
using LogisticCompany.models;
using LogisticCompany.models.auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisticCompany.controllers;

public class CompanyController : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("company").WithOpenApi().WithTags("Company");
        group.MapGet("/", GetCompanies).WithName("GetCompanies").RequireAuthorization();
        group.MapPost("/", AddCompany).WithName("AddCompany").RequireAuthorization("admin_manager_employee");
        group.MapPut("/", ChangeCompanies).WithName("ChangeCompany").RequireAuthorization("admin_manager_employee");
        group.MapDelete("/", DeleteCompanies).WithName("DeleteCompanies").RequireAuthorization("admin_manager_employee");
        // group.MapGet("/all", GetAllCompaniesInfo).WithName("GetAllCompaniesInfo").RequireAuthorization("admin_manager_employee");
        group.MapGet("{id}", GetCompanyById).WithName("GetCompanyById").RequireAuthorization();
        group.MapDelete("{id}", DeleteCompanyById).WithName("DeleteCompanyById").RequireAuthorization("admin_manager_employee");
        group.MapPost("{id}", AddCompanyById).WithName("AddCompanyById").RequireAuthorization("admin_manager_employee");
        group.MapPut("{id}", ChangeCompanyById).WithName("ChangeCompaniesById").RequireAuthorization("admin_manager_employee");
        group.MapGet("{id}/users", GetCompanyUsers).WithName("GetCompanyUsers").RequireAuthorization();
        group.MapGet("{id}/revenue", GetCompanyRevenues).WithName("GetCompanyRevenue").RequireAuthorization("admin_manager_employee");
        group.MapPost("{id}/revenue", CreateCompanyRevenue).WithName("CreateCompanyRevenue").RequireAuthorization("admin_manager_employee");
        // group.MapGet("{id:int}", GetCompaniesById).WithName("GetCompaniesById");
    }

    private static async Task<Ok<List<CompanyGetDto>>> GetCompanies(LogisticCompanyContext context)
    {
        var companies = await context.Companies
            .Select(c => new CompanyGetDto()
            {
                Id = c.Id,
                Name = c.Name,
                CreationDate = c.CreationDate,
                Address = c.Address
            }).ToListAsync();
        // var query = from c in context.Companies
        //     join o in context.Offices on c.Id equals o.CompanyId into co
        //     from o in co.DefaultIfEmpty()
        //     join e in context.Employees on o.Id equals e.OfficeId into oe
        //     from e in oe.DefaultIfEmpty()
        //     select new CompanyDto
        //     {
        //         Id = c.Id,
        //         Name = c.Name,
        //         CreationDate = c.CreationDate,
        //         Employees = e == null ? null : new List<EmployeeDto>
        //         {
        //             new ()
        //             {
        //                 Id = e.Id,
        //                 Name = e.User.FirstName
        //             }
        //         }
        //     };
        // var companies = await query.ToListAsync();
        return TypedResults.Ok(companies);
    }
    private static async Task<Ok> ChangeCompanies([FromBody] ChangeCompanyDto[] changeCompanyDto,LogisticCompanyContext context)
    {
        foreach (var companyDto in changeCompanyDto)
        {
            var company = await context.Companies.FindAsync(companyDto.Id);
            if (company != null)
            {
                if (companyDto.Name != null)
                {
                    company.Name = companyDto.Name;
                }
                company.CreationDate = companyDto.CreationDate;
                company.Address = companyDto.Address;
            }
        }
        await context.SaveChangesAsync();
        return TypedResults.Ok();
    }
    private static async Task<Ok> ChangeCompanyById(int id ,[FromBody] ChangeCompanyDto changeCompanyDto,LogisticCompanyContext context)
    {
            var company = await context.Companies.FindAsync(id);
            if (company != null)
            {
                if (changeCompanyDto.Name != null)
                {
                    company.Name = changeCompanyDto.Name;
                }

                if (changeCompanyDto.CreationDate != null)
                {
                    company.CreationDate = changeCompanyDto.CreationDate;
                }

                if (changeCompanyDto.Address != null)
                {
                    company.Address = changeCompanyDto.Address;
                }
            }
            await context.SaveChangesAsync();
        return TypedResults.Ok();
    }
    private static async Task<NoContent> DeleteCompanies(LogisticCompanyContext context)
    {
        var companies = await context.Companies.ExecuteDeleteAsync();
        return TypedResults.NoContent();
    }
    private static async Task<NoContent> DeleteCompanyById(int id , LogisticCompanyContext context)
    {
        var companies = await context.Companies.Where(e => e.Id == id).ExecuteDeleteAsync();
        return TypedResults.NoContent();
    }
    private static async Task<Ok<CompanyGetDto>> GetCompanyById(int id , LogisticCompanyContext context)
    {
        var companies = await context.Companies
            .Where(e => e.Id == id)
            .Select(c => new CompanyGetDto()
            {
                Id = c.Id,
                Name = c.Name,
                CreationDate = c.CreationDate,
                Address = c.Address
            }).FirstOrDefaultAsync();
        // var query = from c in context.Companies
        //     join o in context.Offices on c.Id equals o.CompanyId into co
        //     from o in co.DefaultIfEmpty()
        //     join e in context.Employees on o.Id equals e.OfficeId into oe
        //     from e in oe.DefaultIfEmpty()
        //     select new CompanyDto
        //     {
        //         Id = c.Id,
        //         Name = c.Name,
        //         CreationDate = c.CreationDate,
        //         Employees = e == null ? null : new List<EmployeeDto>
        //         {
        //             new ()
        //             {
        //                 Id = e.Id,
        //                 Name = e.User.FirstName
        //             }
        //         }
        //     };
        // var companies = await query.ToListAsync();
        return TypedResults.Ok(companies);
    }

    // private static async Task<Ok<List<CompanyDto>>> GetAllCompaniesInfo(LogisticCompanyContext context)
    // {
    //     var companies = await context.Companies
    //         .Include(c => c.Offices)
    //         .ThenInclude(o => o.Employees)
    //         .ThenInclude(e => e.RegisterSentPackages)
    //         .ThenInclude(p => p.PackageInfo)
    //         .Select(c => new CompanyDto
    //         {
    //             Id = c.Id,
    //             Name = c.Name,
    //             CreationDate = c.CreationDate,
    //             Offices = c.Offices.Select(o => new OfficeDto
    //             {
    //                 Id = o.Id,
    //                 Employees = o.Employees.Select(e => new EmployeeDto
    //                 {
    //                     Id = e.Id,
    //                     Packages = e.RegisterSentPackages.Select(p => new PackageDto
    //                     {
    //                         Id = p.Id,
    //                         TrackerNumber = p.TrackerNumber,
    //                         Status = p.Status.Status,
    //                         DeliveryDate = p.DeliveryDate,
    //                         ShippingDate = p.ShippingDate,
    //                         DeliveryAddress = p.DeliveryAddress,
    //                         PackageInfo = new PackageInfoDto
    //                         {
    //                             Weight = p.PackageInfo.Weight,
    //                             Description = p.PackageInfo.Description,
    //                             Fragile = p.PackageInfo.Fragile,
    //                             Hazardous = p.PackageInfo.Hazardous
    //                         }
    //                     }).ToList()
    //                 }).ToList()
    //             }).ToList()
    //         }).AsSplitQuery().ToListAsync();
    //     ;
    //     return TypedResults.Ok(companies);
    // }

    private static async Task<Results<ProblemHttpResult, Created<AddCompanyDto>>> AddCompany(
        [FromBody] AddCompanyDto addCompany, LogisticCompanyContext context
    )
    {
        var company = new Company
        {
            Name = addCompany.Name,
            Address = addCompany.Address,
            CreationDate = addCompany.CreationDate ?? DateOnly.FromDateTime(DateTime.Now)
        };
        var dbCompanyCheck = await context.Companies.FirstOrDefaultAsync(c => c.Name == addCompany.Name);
        if (dbCompanyCheck != null)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Company already exists",
                Detail = "Company with this name already exists",
                Status = StatusCodes.Status409Conflict
            });
        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();
        return TypedResults.Created($"/company/{company.Id}", addCompany);
    }    
    private static async Task<Results<ProblemHttpResult, Created<AddCompanyDto>>> AddCompanyById( int id,
        [FromBody] AddCompanyDto addCompany, LogisticCompanyContext context
    )
    {
        var company = new Company
        {
            Name = addCompany.Name,
            Address = addCompany.Address,
            CreationDate = addCompany.CreationDate ?? DateOnly.FromDateTime(DateTime.Now)
        };
        var dbCompanyCheck = await context.Companies.FirstOrDefaultAsync(c => c.Name == addCompany.Name);
        if (dbCompanyCheck != null)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Company already exists",
                Detail = "Company with this name already exists",
                Status = StatusCodes.Status409Conflict
            });
        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();
        return TypedResults.Created($"/company/{company.Id}", addCompany);
    }
    private static async Task<Results<ProblemHttpResult, Ok<List<CompanyUserDto>>>> GetCompanyUsers(string name,
        LogisticCompanyContext _context
    )
    {
        var receivedUsers = _context.ApplicationUsers.Include(e => e.RecievedPackages)
            .ThenInclude(e => e.CompanyName)
            .Where(e => e.RecievedPackages.Any(p => p.CompanyName == name)).Select(e => new CompanyUserDto
            {
                Id = e.Id,
                Address = e.Address,
                BirthDate = e.BirthDate,
                PhoneNumber = e.PhoneNumber,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email
            });
        var sendUsers = _context.ApplicationUsers.Include(e => e.SendPackages).ThenInclude(e => e.CompanyName)
            .Where(e => e.SendPackages.Any(p => p.CompanyName == name)).Select(e => new CompanyUserDto
            {
                Id = e.Id,
                Address = e.Address,
                BirthDate = e.BirthDate,
                PhoneNumber = e.PhoneNumber,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email
            });
        var result = await receivedUsers.Union(sendUsers).ToListAsync();
        return TypedResults.Ok(result);
    }
    private static async Task<Results<ProblemHttpResult, Ok<List<CompanyRevenue>>>> GetCompanyRevenues(int id,
        LogisticCompanyContext _context
    )
    {
        var result = await _context.CompanyRevenues.Where(e => e.CompanyId == id).ToListAsync();
        return TypedResults.Ok(result);
    }
    private static async Task<Results<ProblemHttpResult, Ok<decimal>>> CreateCompanyRevenue(int id,
        LogisticCompanyContext _context
    )
    {
        var company = await _context.Companies.FindAsync(id);
        var employeeSalary =  _context.Employees.Where(e => e.Office.CompanyId == id).Select(e => e.Salary).Sum();
        var packageRevenue = _context.Packages.Where(e => e.CompanyName == company.Name).Select(e => e.price).Sum();
        var result = packageRevenue - employeeSalary;
        var res = await _context.CompanyRevenues.AddAsync(new CompanyRevenue
        {
            CompanyId = id,
            Revenue = result
        });
        await _context.SaveChangesAsync();
        return TypedResults.Ok(employeeSalary);
    }
}
// private static async Task<Results<Ok<List<CompanyDto>>, NotFound>> AddCompany(LogisticCompanyContext context)
// {
//     var company = await context.Companies
//         .Include(company => company.Employees)
//         .Select(c => new CompanyDto
//         {
//             Id = c.Id,
//             Name = c.Name,
//             CreationDate = c.CreationDate,
//             Employees = c.Employees.Select(e => new EmployeeDto
//             {
//                 Id = e.Id,
//             }).ToList()
//         }).ToListAsync();
//     if (company.Count == 0) return TypedResults.NotFound();
//     return TypedResults.Ok(company);
// }