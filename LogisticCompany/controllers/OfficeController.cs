using Carter;
using LogisticCompany.data;
using LogisticCompany.Data.DTOs;
using LogisticCompany.Data.DTOs.Company;
using LogisticCompany.Data.DTOs.Office;
using LogisticCompany.models;
using LogisticCompany.models.auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisticCompany.controllers;

public class OfficeController : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("office").WithOpenApi().WithTags("office");
        group.MapGet("/", GetOffices).WithName("GetOffices").RequireAuthorization("admin_manager_employee");
        group.MapPost("/", AddOffice).WithName("AddOffices").RequireAuthorization("admin_manager_employee");
        group.MapPut("/", ChangeOffices).WithName("ChangeOffices").RequireAuthorization("admin_manager_employee");
        group.MapDelete("/", DeleteOffices).WithName("DeleteOffices").RequireAuthorization("admin_manager_employee");
        group.MapGet("{id}", GetOfficeById).WithName("GetOfficesById").RequireAuthorization("admin_manager_employee");
        group.MapDelete("{id}", DeleteOfficeById).WithName("DeleteOfficesById").RequireAuthorization("admin_manager_employee");
        group.MapPut("{id}", ChangeOfficeById).WithName("ChangeOfficesById").RequireAuthorization("admin_manager_employee");
        // group.MapGet("{id:int}", GetCompaniesById).WithName("GetCompaniesById");
    }

    private static async Task<Ok<List<GetOfficesDto>>> GetOffices(LogisticCompanyContext context)
    {
        var companies = await context.Offices
            .Include(e => e.Company)
            .Select(c => new GetOfficesDto()
            {
                Id = c.Id,
                Address = c.Address,
                CompanyId = c.CompanyId,
                PhoneNumber = c.PhoneNumber,
                CompanyName = c.Company.Name
            }).ToListAsync();
        return TypedResults.Ok(companies);
    }
    private static async Task<Ok> ChangeOffices([FromBody] ChangeOfficeDto[] changeOfficeDtos,LogisticCompanyContext context)
    {
        foreach (var officeDto in changeOfficeDtos)
        {
            var office = await context.Offices.FindAsync(officeDto.Id);
            if (office != null)
            {
                if (officeDto.Address != null)
                {
                    office.Address = officeDto.Address;
                }
                if(office.PhoneNumber != null)
                {
                    office.PhoneNumber = officeDto.PhoneNumber;
                }
            }
        }
        await context.SaveChangesAsync();
        return TypedResults.Ok();
    }
    private static async Task<Ok> ChangeOfficeById(int id ,[FromBody] ChangeOfficeDto changeOfficeDto,LogisticCompanyContext context)
    {
            var company = await context.Offices.FindAsync(id);
            if (company != null)
            {
                if (changeOfficeDto.Address != null)
                {
                    company.Address = changeOfficeDto.Address;
                }

                if (changeOfficeDto.PhoneNumber != null)
                {
                    company.PhoneNumber = changeOfficeDto.PhoneNumber;
                }
            }
            await context.SaveChangesAsync();
        return TypedResults.Ok();
    }
    private static async Task<NoContent> DeleteOffices(LogisticCompanyContext context)
    {
        var companies = await context.Offices.ExecuteDeleteAsync();
        return TypedResults.NoContent();
    }
    private static async Task<NoContent> DeleteOfficeById(int id , LogisticCompanyContext context)
    {
        var companies = await context.Offices.Where(e => e.Id == id).ExecuteDeleteAsync();
        return TypedResults.NoContent();
    }
    private static async Task<Ok<GetOfficesDto>> GetOfficeById(int id , LogisticCompanyContext context)
    {
        var companies = await context.Offices
            .Where(e => e.Id == id)
            .Include(e => e.Company)
            .Select(c => new GetOfficesDto()
            {
                Id = c.Id,
                Address = c.Address,
                CompanyId = c.CompanyId,
                CompanyName = c.Company.Name,
                PhoneNumber = c.PhoneNumber
            }).FirstOrDefaultAsync();
        return TypedResults.Ok(companies);
    }

    private static async Task<Results<ProblemHttpResult, Created<AddOfficeDto>>> AddOffice(
        [FromBody] AddOfficeDto addOfficeDto, LogisticCompanyContext _context
    )
    {
        var company_id = await _context.Companies.Where(e => e.Name == addOfficeDto.CompanyName).Select(e => e.Id).FirstOrDefaultAsync();
        var newOffice = new Office
        {
            Address = addOfficeDto.Address,
            PhoneNumber = addOfficeDto.PhoneNumber,
            CompanyId = company_id
        };
        var dbCompanyCheck = await _context.Offices.FirstOrDefaultAsync(c => c.Address == addOfficeDto.Address);
        if (dbCompanyCheck != null)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Company already exists",
                Detail = "Company with this name already exists",
                Status = StatusCodes.Status409Conflict
            });
        await _context.Offices.AddAsync(newOffice);
        await _context.SaveChangesAsync();
        return TypedResults.Created($"/company/{newOffice.Id}", addOfficeDto);
    }    
}