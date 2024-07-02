using System.Security.Claims;
using Carter;
using LogisticCompany.data;
using LogisticCompany.Data.DTOs;
using LogisticCompany.Data.DTOs.Company;
using LogisticCompany.Data.DTOs.Employee;
using LogisticCompany.Data.DTOs.User;
using LogisticCompany.models;
using LogisticCompany.models.auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace LogisticCompany.controllers;

public class EmployeeController : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("employee").WithOpenApi().WithTags("employee");
        group.MapGet("/", GetEmployees).WithName("GetEmployees").RequireAuthorization("admin_manager_employee");
        group.MapPost("/", AddEmployee).WithName("AddEmployee").RequireAuthorization("admin_manager_employee");
        group.MapPut("/", ChangeEmployee).WithName("ChangeEmployee").RequireAuthorization("admin_manager_employee");
        group.MapDelete("/", DeleteEmployee).WithName("DeleteEmployee").RequireAuthorization("admin_manager_employee");
        group.MapGet("/{id}", GetEmployeeById).WithName("GetEmployeeById").RequireAuthorization("admin_manager_employee");
        group.MapPut("/{id}", ChangeEmployeeById).WithName("ChangeEmployeeById").RequireAuthorization("admin_manager_employee");
        group.MapDelete("/{id}", DeleteEmployeeById).WithName("DeleteEmployeeById").RequireAuthorization("admin_manager_employee");
    }
    private static async Task<Results<ProblemHttpResult, Ok<List<EmployeeDto>>>> GetEmployees(LogisticCompanyContext _context)
    {
        var users = await _context.Employees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.User.FirstName,
            LastName = e.User.LastName,
            BirthDate = e.User.BirthDate,
            Address = e.User.Address,
            Email = e.User.Email,
            Roles = e.User.UserRoles.Select(ur => ur.Role.Name).ToList(),
            Position = e.Position.PositionType,
            Salary = e.Salary,
            PhoneNumber = e.User.PhoneNumber,
            HireDate = e.HireDate,
            OfficeId = e.OfficeId
        }).ToListAsync();
        return TypedResults.Ok(users);
    }
        
    private static async Task<Results<ProblemHttpResult, Created>> AddEmployee(
        [FromBody] AddEmployeeDto addEmployee, LogisticCompanyContext _context
    )
    {
        var newEmployee = new Employee
        {
            Id = new Guid(addEmployee.Id),
            HireDate = DateOnly.FromDateTime(DateTime.Now),
            PositionId = addEmployee.PositionId,
            OfficeId = addEmployee.OfficeId,
            Salary = addEmployee.Salary
        };
        _context.Employees.Add(newEmployee);
        var user = await _context.SaveChangesAsync();
        if(user != 1)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Employee not added",
                Detail = "Employee was not added to the system",
                Status = StatusCodes.Status400BadRequest
            });
        return TypedResults.Created();
    }

    private static async Task<Results<ProblemHttpResult, Ok<List<ChangeEmployeeDto>>>> ChangeEmployee(
        [FromBody] ChangeEmployeeDto[] changeEmployee, LogisticCompanyContext _context
    )
    {
        
        var result = new List<ChangeEmployeeDto>();
        foreach (var employee in changeEmployee)
        {
            if (Guid.TryParse(employee.Id, out var userId) is false)
                return TypedResults.Problem(new ProblemDetails
                {
                    Title = "Invalid user id",
                    Detail = $"User id {employee.Id}  is not a valid GUID",
                    Status = StatusCodes.Status400BadRequest
                });
            await _context.Employees.Where(e => e.Id == userId)
                .ExecuteUpdateAsync(calls => calls
                    .SetProperty(e => e.OfficeId, employee.OfficeId)
                    .SetProperty(e => e.Salary, employee.Salary)
                    .SetProperty(e => e.PositionId, employee.PositionId)
                );
        }
        return TypedResults.Ok(result);
    }


    private static async Task<Results<ProblemHttpResult, Ok>> ChangeEmployeeById(string id,
        [FromBody] ChangeEmployeeDto changeEmployee, LogisticCompanyContext _context
    )
    {
        if (Guid.TryParse(id, out var userId) is false)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Invalid user id",
                Detail = "User id is not a valid GUID",
                Status = StatusCodes.Status400BadRequest
            });
        await _context.Employees.Where(e => e.Id == userId)
            .ExecuteUpdateAsync(calls => calls
                .SetProperty(e => e.OfficeId, changeEmployee.OfficeId)
                .SetProperty(e => e.Salary, changeEmployee.Salary)
                .SetProperty(e => e.PositionId, changeEmployee.PositionId)
            );
        return TypedResults.Ok();
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> DeleteEmployee(LogisticCompanyContext _context)
    {
        var result = await _context.Employees.ExecuteDeleteAsync();
        return TypedResults.NoContent();
    }
    private static async Task<Results<NoContent, ProblemHttpResult>> DeleteEmployeeById(string id , LogisticCompanyContext _context , UserManager<ApplicationUser> userManager)
    {
        if (Guid.TryParse(id, out var userId) is false)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Invalid user id",
                Detail = "User id is not a valid GUID",
                Status = StatusCodes.Status400BadRequest
            });
        var result = await _context.Employees.Where(e => e.Id == userId).ExecuteDeleteAsync();
        return TypedResults.NoContent();
    }

    private static async Task<Results<ProblemHttpResult, Ok<EmployeeDto>>> GetEmployeeById(string id,
        LogisticCompanyContext _context
    )
    {
        if (Guid.TryParse(id, out var userId) is false)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Invalid user id",
                Detail = "User id is not a valid GUID",
                Status = StatusCodes.Status400BadRequest
            });
        var employee = await _context.Employees.Where(e => e.Id == userId).Select(e => new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.User.FirstName,
            LastName = e.User.LastName,
            BirthDate = e.User.BirthDate,
            Address = e.User.Address,
            Email = e.User.Email,
            Roles = e.User.UserRoles.Select(ur => ur.Role.Name).ToList(),
            Position = e.Position.PositionType,
            Salary = e.Salary,
            PhoneNumber = e.User.PhoneNumber,
            HireDate = e.HireDate,
            OfficeId = e.OfficeId
        }).FirstOrDefaultAsync();
        return TypedResults.Ok(employee);
    }
}