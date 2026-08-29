using System.Data.Common;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Veesh.Core.Common;
using Veesh.Core.DTOs.User;
using Veesh.Core.Exceptions;
using Veesh.Core.Interfaces;
using Veesh.Domain.Entities;
using Veesh.Domain.Enums;

namespace Veesh.Core.Services;

public class UserService(
    IUserRepository userRepository,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    ILogger<UserService> logger)
{
    public async Task<Pageable<UserResponse>> GetAllAsync(UserQuery query)
    {
        var result = await userRepository.GetAllAsync(query);

        var roleIds = result.Items
            .SelectMany(u => u.UserRoles)
            .Select(ur => ur.RoleId)
            .Distinct()
            .ToList();

        var roleDict = await roleManager.Roles
            .Where(r => roleIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, r => r.Name);

        var responses = result.Items.Select(u =>
        {
            var roleName = u.UserRoles
                .Select(ur => roleDict.GetValueOrDefault(ur.RoleId))
                .FirstOrDefault();

            return new UserResponse(
                u.Id,
                u.UserName ?? string.Empty,
                u.PhoneNumber ?? string.Empty,
                u.Email ?? string.Empty,
                u.Name ?? string.Empty,
                u.Bio ?? string.Empty,
                u.ProfileImageUrl ?? string.Empty,
                u.BirthDate ?? DateTimeOffset.MinValue,
                Enum.TryParse<UserRole>(roleName, out var role) ? role : UserRole.User
            );
        }).ToList();

        return new Pageable<UserResponse>(responses, result.Page, result.Size, result.Total, result.LastPage);
    }

    public async Task<UserResponse> GetByIdAsync(Guid id)
    {
        var result = await userRepository.GetUserByIdAsync(id);
        if (result == null) throw new NotFoundException("User not found.");

        var roles = await userManager.GetRolesAsync(result);
        var role = roles.Count > 0 && Enum.TryParse<UserRole>(roles[0], out var parsed)
            ? parsed
            : UserRole.User;

        return new UserResponse(
            result.Id,
            result.UserName ?? string.Empty,
            result.PhoneNumber ?? string.Empty,
            result.Email ?? string.Empty,
            result.Name ?? string.Empty,
            result.Bio ?? string.Empty,
            result.ProfileImageUrl ?? string.Empty,
            result.BirthDate ?? DateTimeOffset.MinValue,
            role
        );
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, UserRole role = UserRole.User)
    {
        try
        {
            var userByUsername = await userRepository.GetUserByUsernameAsync(request.UserName);
            if (userByUsername == null) throw new DuplicateException("UserName exists.");

            var userByPhoneNumber = await userRepository.GetUserByPhoneNumberAsync(request.PhoneNumber);
            if (userByPhoneNumber == null) throw new DuplicateException("PhoneNumber exists.");

            if (request.Email != null)
            {
                var userByEmail = await userRepository.GetUserByEmailAsync(request.Email);
                if (userByEmail == null) throw new DuplicateException("Email exists.");
            }

            var applicationUser = new ApplicationUser
            {
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Name = request.Name,
                Bio = request.Bio,
                ProfileImageUrl = request.ProfileImageUrl,
                BirthDate = request.BirthDate
            };
            var newUser = await userRepository.CreateAsync(applicationUser, request.Password, role);

            return newUser.Adapt<UserResponse>();
        }
        catch (DbUpdateException e)
        {
            logger.LogError(e, "Failed to create user with name {UserName}", request.UserName);
            throw new ConflictException("Failed to create User");
        }
    }

    public async Task<UserResponse> UpdateAsync(UpdateUserRequest request, Guid userId, List<UserRole> roles)
    {
        try
        {
            if (!roles.Contains(UserRole.Admin) && request.Id != userId)
                throw new UnauthorizedAccessException("You are not authorized to update this user.");

            var applicationUser = new ApplicationUser
            {
                Id = request.Id,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Name = request.Name,
                Bio = request.Bio,
                ProfileImageUrl = request.ProfileImageUrl,
                BirthDate = request.BirthDate,
            };
            var updatedUser = await userRepository.UpdateAsync(applicationUser);
            return updatedUser.Adapt<UserResponse>();
        }
        catch (DbUpdateException e)
        {
            logger.LogError(e, "Failed to update user with id {Id}", request.Id);
            throw new ConflictException("Failed to update User");
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            return await userRepository.DeleteAsync(id);
        }
        catch (DbException e)
        {
            logger.LogError(e, "Failed to delete user with id {Id}", id);
            throw new ConflictException("Failed to create User");
        }
    }
}