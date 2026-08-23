using System.Data.Common;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Veesh.Core.Common;
using Veesh.Core.DTOs.User;
using Veesh.Core.Exceptions;
using Veesh.Core.Interfaces;
using Veesh.Domain.Entities;

namespace Veesh.Core.Services;

public class UserService(IUserRepository userRepository, ILogger<UserService> logger)
{
    public async Task<Pageable<UserResponse>> GetAllAsync(UserQuery query, int page, int size)
    {
        var result = await userRepository.GetAllAsync(query, page, size);
        return result.Adapt<Pageable<UserResponse>>();
    }


    public async Task<UserResponse> GetByIdAsync(Guid id)
    {
        var result = await userRepository.GetUserByIdAsync(id);
        if (result == null) throw new NotFoundException("User not found.");
        return result.Adapt<UserResponse>();
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request)
    {
        try
        {
            var userByUsername = await userRepository.GetUserByUsernameAsync(request.UserName);
            if (userByUsername == null) throw new Exception("UserName exists.");

            var userByPhoneNumber = await userRepository.GetUserByPhoneNumberAsync(request.PhoneNumber);
            if (userByPhoneNumber == null) throw new Exception("PhoneNumber exists.");

            if (request.Email != null)
            {
                var userByEmail = await userRepository.GetUserByEmailAsync(request.Email);
                if (userByEmail == null) throw new Exception("Email exists.");
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
            var newUser = await userRepository.CreateAsync(applicationUser, "abc123456");
            return newUser.Adapt<UserResponse>();
        }
        catch (DbUpdateException e)
        {
            logger.LogError(e, "Failed to create user with name {UserName}", request.UserName);
            throw new ConflictException("Failed to create User");
        }
    }

    public async Task<UserResponse> UpdateAsync(UpdateUserRequest request)
    {
        try
        {
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