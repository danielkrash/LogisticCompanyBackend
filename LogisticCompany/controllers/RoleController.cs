using System.Security.Claims;
using Carter;
using LogisticCompany.data;
using LogisticCompany.Data.DTOs.User;
using LogisticCompany.Data.Models.auth;
using LogisticCompany.models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisticCompany.controllers;

public class RoleController : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("role").WithOpenApi().WithTags("Roles");
        group.MapGet("/", GetRoles).WithName("GetRoles").RequireAuthorization();
    }

    private static async Task<Results<Ok<List<UserRole>> , NotFound>> GetRoles(LogisticCompanyContext _context)
    {
        var roles = await _context.Roles.ToListAsync();
        return TypedResults.Ok(roles);
    }
}