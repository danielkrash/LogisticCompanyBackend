using Carter;
using LogisticCompany.data;
using LogisticCompany.Data.DTOs.Employee;
using LogisticCompany.models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LogisticCompany.controllers;

public class PositionController : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("position").WithOpenApi().WithTags("Positions");
        group.MapGet("/", GetPosition).WithName("GetPositions").RequireAuthorization();
    }
    
    private static async Task<Results<Ok<List<Position>> , NotFound>> GetPosition(LogisticCompanyContext _context)
    {
        var roles = await _context.Positions.ToListAsync();
        return TypedResults.Ok(roles);
    }
}