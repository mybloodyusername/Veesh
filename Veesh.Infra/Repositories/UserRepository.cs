using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Veesh.Core.Exceptions;
using Veesh.Core.Interfaces;
using Veesh.Domain.Entities;
using Veesh.Infra.Data;

namespace Veesh.Infra.Repositories;

public class UserRepository(UserManager<ApplicationUser> userManager, VeeshDbContext context) : IUserRepository
{
    public async Task<ApplicationUser?> GetUserByIdAsync(Guid id)
    {
        return await userManager.FindByIdAsync(id.ToString());
    }

    public async Task<ApplicationUser?> GetUserByUsernameAsync(string username)
    {
        return await userManager.FindByNameAsync(username);
    }

    public async Task<ApplicationUser?> GetUserByPhoneNumberAsync(string phoneNumber)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<ApplicationUser> Create(ApplicationUser user, string password)
    {
        // if (user.UserName != null)
        // {
        //     var userByUsername = await GetUserByUsernameAsync(user.UserName);
        //     if (userByUsername == null) throw new Exception("UserName exists.");
        // }
        //
        // if (user.PhoneNumber != null)
        // {
        //     var userByPhoneNumber = await GetUserByPhoneNumberAsync(user.PhoneNumber);
        //     if (userByPhoneNumber == null) throw new Exception("PhoneNumber exists.");
        // }
        //
        // if (user.Email != null)
        // {
        //     var userByEmail = await GetUserByEmailAsync(user.Email);
        //     if (userByEmail == null) throw new Exception("Email exists.");
        // }

        var userResult = await userManager.CreateAsync(user, password);
        if (userResult.Succeeded) return user;
        var errors = string.Join("; ", userResult.Errors.Select(e => e.Description));
        throw new Exception(errors);
    }

    public async Task<ApplicationUser> Update(ApplicationUser user)
    {
        var existingUser = await GetUserByIdAsync(user.Id);
        if (existingUser == null) throw new NotFoundException("User not found");
        
        existingUser.UserName = user.UserName;
        existingUser.Email = user.Email;
        existingUser.PhoneNumber = user.PhoneNumber;
        existingUser.Name = user.Name;
        existingUser.Bio = user.Bio;
        existingUser.ProfileImageUrl = user.ProfileImageUrl;
        existingUser.BirthDate = user.BirthDate;
        existingUser.UpdatedAt = DateTimeOffset.UtcNow;
        
        var result = await userManager.UpdateAsync(existingUser);
        if (result.Succeeded) return existingUser;
        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
        throw new Exception(errors);
    }

    public async Task<bool> Delete(Guid id)
    {
        var existingUser = await GetUserByIdAsync(id);
        if (existingUser == null) throw new NotFoundException("User not found");
        var result = await userManager.DeleteAsync(existingUser);
        return result.Succeeded;
    }
}