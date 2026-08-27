using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Veesh.Core.Common;
using Veesh.Core.Exceptions;
using Veesh.Core.Interfaces;
using Veesh.Domain.Entities;
using Veesh.Infra.Data;

namespace Veesh.Infra.Repositories;

public class UserRepository(UserManager<ApplicationUser> userManager, VeeshDbContext context) : IUserRepository
{
    public async Task<Pageable<ApplicationUser>> GetAllAsync(UserQuery query)
    {
        var queryable = context.Users.AsNoTracking();

        if (query.Id != null)
        {
            queryable = queryable.Where(q => q.Id == query.Id);
        }

        if (query.UserName != null)
        {
            queryable = queryable.Where(q => q.UserName!.Contains(query.UserName));
        }

        if (query.Email != null)
        {
            queryable = queryable.Where(q => q.Email != null && q.Email.Contains(query.Email));
        }

        if (query.PhoneNumber != null)
        {
            queryable = queryable.Where(q => q.PhoneNumber!.Contains(query.PhoneNumber));
        }

        if (query.Name != null)
        {
            queryable = queryable.Where(q => q.Name != null && q.Name.Contains(query.Name));
        }

        if (query.BirthDate != null)
        {
            queryable = queryable.Where(q => q.BirthDate == query.BirthDate);
        }

        if (query.CreatedAt != null)
        {
            queryable = queryable.Where(q => q.CreatedAt == query.CreatedAt);
        }

        if (query.UpdatedAt != null)
        {
            queryable = queryable.Where(q => q.UpdatedAt == query.UpdatedAt);
        }

        if (query.OrderBy != null)
        {
            queryable = queryable.OrderBy(q => query.OrderBy);
        }

        var total = await queryable.CountAsync();
        var lastPage = total / query.Size;
        var items = await queryable.Skip((query.Page) * query.Size).Take(query.Size).ToListAsync();

        return new Pageable<ApplicationUser>(items, query.Page, query.Size, total, lastPage);
    }

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

    public async Task<ApplicationUser> CreateAsync(ApplicationUser user, string password)
    {
        if (user.UserName != null)
        {
            var userByUsername = await GetUserByUsernameAsync(user.UserName);
            if (userByUsername == null) throw new DuplicateException("UserName exists.");
        }
        
        if (user.PhoneNumber != null)
        {
            var userByPhoneNumber = await GetUserByPhoneNumberAsync(user.PhoneNumber);
            if (userByPhoneNumber == null) throw new DuplicateException("PhoneNumber exists.");
        }
        
        if (user.Email != null)
        {
            var userByEmail = await GetUserByEmailAsync(user.Email);
            if (userByEmail == null) throw new DuplicateException("Email exists.");
        }

        var userResult = await userManager.CreateAsync(user, password);
        if (userResult.Succeeded) return user;
        var errors = string.Join("; ", userResult.Errors.Select(e => e.Description));
        throw new ConflictException(errors);
    }

    public async Task<ApplicationUser> UpdateAsync(ApplicationUser user)
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
        throw new ConflictException(errors);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existingUser = await GetUserByIdAsync(id);
        if (existingUser == null) throw new NotFoundException("User not found");
        var result = await userManager.DeleteAsync(existingUser);
        return result.Succeeded;
    }
}