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

public class UsersController : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("users").WithOpenApi().WithTags("Users");
        group.MapGet("/", GetUsers).WithName("GetUsers").RequireAuthorization();
        group.MapPut("/", ChangeUsers).WithName("ChangeUserDto").RequireAuthorization("admin_manager_employee");
        group.MapPost("/", AddUser).WithName("AddUser").RequireAuthorization("admin_manager_employee");
        group.MapDelete("/", DeleteUsers).WithName("DeleteUsers").RequireAuthorization("admin_manager_employee");
        group.MapGet("/{id}", GetUser).WithName("GetUser").RequireAuthorization();
        group.MapPut("/{id}", ChangeUser).WithName("ChangeUser").RequireAuthorization();
        group.MapDelete("/{id}", DeleteUser).WithName("DeleteUser").RequireAuthorization();
        group.MapDelete("/{id}/role", DeleteUserRole).WithName("DeleteUserRole").RequireAuthorization();
        group.MapGet("/current", GetCurrentUser).WithName("GetCurrentUser").RequireAuthorization();
        group.MapGet("/current/Company", GetCurrentUserCompany).WithName("GetCurrentUserCompany").RequireAuthorization();
        group.MapPut("/current", UpdateCurrentUser).WithName("UpdateCurrentUser").RequireAuthorization();
        group.MapDelete("/current", DeleteCurrentUser).WithName("DeleteCurrentUser").RequireAuthorization();
        group.MapPut("/current/password", PasswordChangeCurrentUser).WithName("PasswordChangeCurrentUser").RequireAuthorization();
        group.MapGet("/current/secrets", GetCurrentUserSecrets).WithName("GetCurrentUserSecrets").RequireAuthorization();
    }

    private static async Task<Results<ProblemHttpResult, Created<CreatedUserDto>>> AddUser(
        [FromBody] AddUserDto addUser, LogisticCompanyContext _context,
        UserManager<ApplicationUser> userManager, RoleManager<UserRole> roleManager
    )
    {
        if (await userManager.FindByEmailAsync(addUser.Email) is not null)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "User duplication error",
                Detail = "User with this email already exists in the system",
                Status = StatusCodes.Status409Conflict
            });
        var user = new ApplicationUser()
        {
            FirstName = addUser.FirstName,
            UserName = addUser.Email,
            LastName = addUser.LastName,
            Email = addUser.Email,
            PhoneNumber = addUser.PhoneNumber,
            BirthDate = addUser.BirthDate,
            Address = addUser.Address
        };
        if (await roleManager.RoleExistsAsync(addUser.Role) is true)
        {
            await userManager.CreateAsync(user, addUser.Password);
            if (addUser.Role is not null) await userManager.AddToRoleAsync(user, addUser.Role);
            return TypedResults.Created($"/user/{user.Id}", new CreatedUserDto
            {
                Email = addUser.Email,
                FirstName = addUser.FirstName,
                LastName = addUser.LastName,
                BirthDate = addUser.BirthDate,
                PhoneNumber = addUser.PhoneNumber,
                Role = addUser.Role
            });
        }

        return TypedResults.Problem(new ProblemDetails
        {
            Title = $"Role '{addUser.Role}' does not exist",
            Detail = "Role does not exist in the system",
            Status = StatusCodes.Status400BadRequest
        });
    }

    private static async Task<Results<ProblemHttpResult, Ok<List<ChangeUserDto>>>> ChangeUsers(
        [FromBody] ChangeUserDto[] changeUsers, LogisticCompanyContext _context,
        UserManager<ApplicationUser> userManager, RoleManager<UserRole> roleManager
    )
    {
        var result = new List<ChangeUserDto>();
        foreach (var changeUser in changeUsers)
        {
            var user = await userManager.FindByEmailAsync(changeUser.Email);
            if (user is not null)
            {
                user.FirstName = changeUser.FirstName ?? user.FirstName;
                user.LastName = changeUser.LastName ?? user.LastName;
                user.BirthDate = changeUser.BirthDate ?? user.BirthDate;
                user.Email = changeUser.Email;
                user.PhoneNumber = changeUser.PhoneNumber ?? user.PhoneNumber;
                user.Address = changeUser.Address ?? user.Address;
                await userManager.UpdateAsync(user);
                if (changeUser.Role is not null)
                {
                    if (await roleManager.RoleExistsAsync(changeUser.Role) is false)
                        return TypedResults.Problem(new ProblemDetails
                        {
                            Title = $" On user {changeUser.Email} role '{changeUser.Role}' does not exist",
                            Detail = "Role does not exist in the system",
                            Status = StatusCodes.Status400BadRequest
                        });
                    await userManager.AddToRoleAsync(user, changeUser.Role);
                }

                result.Add(new ChangeUserDto
                {
                    Email = changeUser.Email,
                    FirstName = changeUser.FirstName,
                    LastName = changeUser.LastName,
                    BirthDate = changeUser.BirthDate,
                    PhoneNumber = changeUser.PhoneNumber,
                    Role = changeUser.Role
                });
            }
            else
            {
                return TypedResults.Problem(new ProblemDetails
                {
                    Title = $"There is not such user with email {changeUser.Email}",
                    Detail = "User does not exist in the system",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        return TypedResults.Ok(result);
    }


    private static async Task<Results<ProblemHttpResult, Ok<ChangeUserDto>>> ChangeUser(string id,
        [FromBody] ChangeUserDto changeUsers, LogisticCompanyContext _context,
        UserManager<ApplicationUser> userManager, RoleManager<UserRole> roleManager
    )
    {
        ChangeUserDto result;
        var user = await userManager.FindByIdAsync(id);
        if (user is not null)
        {
            user.FirstName = changeUsers.FirstName ?? user.FirstName;
            user.LastName = changeUsers.LastName ?? user.LastName;
            user.BirthDate = changeUsers.BirthDate ?? user.BirthDate;
            user.Email = changeUsers.Email ?? user.Email;
            user.PhoneNumber = changeUsers.PhoneNumber ?? user.PhoneNumber;
            user.Address = changeUsers.Address ?? user.Address;
            await userManager.UpdateAsync(user);
            if (changeUsers.Role is not null)
            {
                if (changeUsers.Role == "null")
                {
                    result = new ChangeUserDto
                    {
                        Email = changeUsers.Email,
                        FirstName = changeUsers.FirstName,
                        LastName = changeUsers.LastName,
                        BirthDate = changeUsers.BirthDate,
                        PhoneNumber = changeUsers.PhoneNumber,
                        Role = changeUsers.Role
                    };
                    return TypedResults.Ok(result);
                }
                if (await roleManager.RoleExistsAsync(changeUsers.Role) is false)
                    return TypedResults.Problem(new ProblemDetails
                    {
                        Title = $" On user {changeUsers.Email} role '{changeUsers.Role}' does not exist",
                        Detail = "Role does not exist in the system",
                        Status = StatusCodes.Status400BadRequest
                    });
                await userManager.AddToRoleAsync(user, changeUsers.Role);
            }

            result = new ChangeUserDto
            {
                Email = changeUsers.Email,
                FirstName = changeUsers.FirstName,
                LastName = changeUsers.LastName,
                BirthDate = changeUsers.BirthDate,
                PhoneNumber = changeUsers.PhoneNumber,
                Role = changeUsers.Role
            };
        }
        else
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Title = $"There is not such user with email {changeUsers.Email}",
                Detail = "User does not exist in the system",
                Status = StatusCodes.Status400BadRequest
            });
        }
        
        return TypedResults.Ok(result);
    }

    private static async Task<Results<ProblemHttpResult, Ok<List<UserDto>>>> GetUsers(LogisticCompanyContext _context)
    {
        var users = await _context.ApplicationUsers
            .Include(r => r.UserRoles)
            .Select(e => new UserDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                BirthDate = e.BirthDate,
                PhoneNumber = e.PhoneNumber,
                Email = e.Email,
                Address = e.Address,
                Roles = e.UserRoles.Select(r => r.Role.Name).ToList()
            }).ToListAsync();
        return TypedResults.Ok(users);
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> DeleteUsers(LogisticCompanyContext _context)
    {
        var result = await _context.ApplicationUsers
            .Where(e => e.UserRoles.Any(r => r.Role.Name != "admin"
                                             && r.Role.Name != "manager"
                                             || r.Role.Name == null))
            .ExecuteDeleteAsync();
        return TypedResults.NoContent();
    }
    private static async Task<Results<NoContent, ProblemHttpResult>> DeleteUser(string id , LogisticCompanyContext _context , UserManager<ApplicationUser> userManager)
    {
        if (Guid.TryParse(id, out var userId) is false)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Invalid user id",
                Detail = "User id is not a valid GUID",
                Status = StatusCodes.Status400BadRequest
            });
        var result = await _context.ApplicationUsers.Where(e => e.Id == userId).ExecuteDeleteAsync();
        return TypedResults.NoContent();
    }
    private static async Task<Results<Ok, ProblemHttpResult>> DeleteUserRole(string id , [FromBody] DeleteUserRoleDto userRoleDto, LogisticCompanyContext _context , UserManager<ApplicationUser> userManager , RoleManager<UserRole> roleManager)
    {
        if (Guid.TryParse(id, out var userId) is false)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Invalid user id",
                Detail = "User id is not a valid GUID",
                Status = StatusCodes.Status400BadRequest
            });
        if(await userManager.FindByIdAsync(userId.ToString()) is not { } user)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "User not found",
                Detail = "User with this id does not exist",
                Status = StatusCodes.Status404NotFound
            });
        if (await roleManager.RoleExistsAsync(userRoleDto.Role))
        {
            await userManager.RemoveFromRoleAsync(user, userRoleDto.Role);
            return TypedResults.Ok();
        }

        return TypedResults.Problem(new ProblemDetails
        {
            Title = "Role does not exist",
            Detail = "Role does not exist in the system",
            Status = StatusCodes.Status400BadRequest
        });
    }

    private static async Task<Results<ProblemHttpResult, Ok<UserDto>>> GetUser(string id, string type,
        LogisticCompanyContext _context, UserManager<ApplicationUser> userManager
    )
    {
        UserDto user;
        if (type == "email")
        {
            user = await _context.ApplicationUsers.Include(e => e.UserRoles)
                .Where(e => e.Email == id)
                .Select(e => new UserDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email!,
                    BirthDate = e.BirthDate,
                    PhoneNumber = e.PhoneNumber,
                    Address = e.Address,
                    Roles = e.UserRoles.Select(r => r.Role.Name).ToList()
                }).FirstOrDefaultAsync();
            return TypedResults.Ok(user);
        }

        if (type != "id")
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Invalid user type",
                Detail = "User type is not valid",
                Status = StatusCodes.Status400BadRequest
            });
        var userId = Guid.Empty;
        if (Guid.TryParse(id, out userId) is false)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Invalid user id",
                Detail = "User id is not a valid GUID",
                Status = StatusCodes.Status400BadRequest
            });
        user = await _context.ApplicationUsers.Include(e => e.UserRoles)
            .Where(e => e.Id == userId)
            .Select(e => new UserDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                BirthDate = e.BirthDate,
                PhoneNumber = e.PhoneNumber,
                Address = e.Address,
                Roles = e.UserRoles.Select(r => r.Role.Name).ToList()
            }).FirstOrDefaultAsync();
        return TypedResults.Ok(user);
    }
    private static async Task<Results<NotFound, Ok<UserDto>>> GetCurrentUser(ClaimsPrincipal claimsPrincipal,LogisticCompanyContext _context,
        UserManager<ApplicationUser> userManager
    )
    {
        if (await userManager.GetUserAsync(claimsPrincipal) is not { } user) return TypedResults.NotFound();
        var roles = await userManager.GetRolesAsync(user);
        var userDto = new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            BirthDate = user.BirthDate,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            Roles = roles.ToList()
        };
        return TypedResults.Ok(userDto);
    } 
    private static async Task<Results<NotFound, Ok<int>>> GetCurrentUserCompany(ClaimsPrincipal claimsPrincipal,LogisticCompanyContext _context,
        UserManager<ApplicationUser> userManager
    )
    {
        if (await userManager.GetUserAsync(claimsPrincipal) is not { } user) return TypedResults.NotFound();
        var companyId = await _context.Users.Where(e => e.Id == user.Id).Select(e => e.Employee.Office.CompanyId).FirstOrDefaultAsync();
        return TypedResults.Ok(companyId);
    } 
    private static async Task<Results<NotFound, ProblemHttpResult , Ok<ChangeUserDto>>> UpdateCurrentUser(ClaimsPrincipal claimsPrincipal,
        [FromBody] ChangeUserDto changeUserDto,
        UserManager<ApplicationUser> userManager,
        RoleManager<UserRole> roleManager
    )
    {
        if (await userManager.GetUserAsync(claimsPrincipal) is not { } user) return TypedResults.NotFound();
        user.FirstName = changeUserDto.FirstName ?? user.FirstName;
        user.LastName = changeUserDto.LastName ?? user.LastName;
        user.BirthDate = changeUserDto.BirthDate ?? user.BirthDate;
        user.Email = changeUserDto.Email ?? user.Email;
        user.PhoneNumber = changeUserDto.PhoneNumber ?? user.PhoneNumber;
        user.Address = changeUserDto.Address ?? user.Address;
        await userManager.UpdateAsync(user);
        if (changeUserDto.Role is null) return TypedResults.Ok(changeUserDto);
        if(await roleManager.RoleExistsAsync(changeUserDto.Role) is not true)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Role does not exist",
                Detail = "Role does not exist in the system",
                Status = StatusCodes.Status400BadRequest
            });
        await userManager.AddToRoleAsync(user, changeUserDto.Role);
        return TypedResults.Ok(changeUserDto);
    }
    private static async Task<Results<NotFound, ProblemHttpResult , NoContent>> PasswordChangeCurrentUser(ClaimsPrincipal claimsPrincipal,
        [FromBody] UserPasswordChangeDto changePassword,
        UserManager<ApplicationUser> userManager
    )
    {
        if (await userManager.GetUserAsync(claimsPrincipal) is not { } user) return TypedResults.NotFound();
        var result = await userManager.ChangePasswordAsync(user, changePassword.currentPassword, changePassword.newPassword);
        if(result.Succeeded is false)
            return TypedResults.Problem(new ValidationProblemDetails
            {
                Title = "Invalid password",
                Detail = "Current password is invalid",
                Status = StatusCodes.Status400BadRequest,
                Errors = result.Errors.ToDictionary(e => e.Code, e => new[] {e.Description})
            });
        return TypedResults.NoContent();
    }
    private static async Task<Results<NotFound, NoContent>> DeleteCurrentUser(ClaimsPrincipal claimsPrincipal,
        UserManager<ApplicationUser> userManager
    )
    {
        if (await userManager.GetUserAsync(claimsPrincipal) is not { } user) return TypedResults.NotFound();
        await userManager.DeleteAsync(user);
        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<AdminSecretsDto>,Ok<UserSecretsDto>,NotFound>> GetCurrentUserSecrets(ClaimsPrincipal claimsPrincipal,
        UserManager<ApplicationUser> userManager ,
        LogisticCompanyContext _context)
    {
        if(await userManager.GetUserAsync(claimsPrincipal) is not { } user) return TypedResults.NotFound();
        var role = await userManager.GetRolesAsync(user);
        var userId = new Guid(await userManager.GetUserIdAsync(user));
        if (role.Contains("admin") || role.Contains("manager") || role.Contains("employee"))
        {
            var userSecrets = await _context.ApplicationUsers.Where(e => e.Id == userId)
                .Include(e => e.UserRoles)
                .Select(e => new AdminSecretsDto
                {
                    companyCount = _context.Companies.Count(),
                    employeeCount = _context.Employees.Count(),
                    managerCount = _context.Users.Count(e => e.UserRoles.Any(r => r.Role.Name == "manager")),
                    userCount = _context.Users.Count(e => e.UserRoles.Any(r => r.Role.Name == "customer")),
                    roleCount = _context.Roles.Count(),
                    packagesCount = _context.Packages.Count(),
                    userRole = e.UserRoles.Select(e => e.Role.Name).ToList(),
                    email = e.Email
                }).FirstOrDefaultAsync();
            return TypedResults.Ok(userSecrets);
        }

        if (role.Contains("customer"))
        {
            var userSecrets = await _context.ApplicationUsers.Where(e => e.Id == userId)
                .Include(e => e.UserRoles)
                .Select(e => new UserSecretsDto
                {
                    email = e.Email,
                    Id = e.Id.ToString(),
                    receivedPackageCount = _context.Packages.Count(e => e.ReceiverId == userId),
                    sentPackageCount = _context.Packages.Count(e => e.SenderId == userId),
                }).FirstOrDefaultAsync();
            return TypedResults.Ok(userSecrets);
        }

        return TypedResults.NotFound();
    }
}