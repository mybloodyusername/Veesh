using Veesh.Domain.Entities;

namespace Veesh.Core.Interfaces;

public interface IUserRepository
{
    public Task<ApplicationUser?> GetUserByIdAsync(Guid id);
    public Task<ApplicationUser?> GetUserByUsernameAsync(string username);
    public Task<ApplicationUser?> GetUserByPhoneNumberAsync(string phoneNumber);
    public Task<ApplicationUser?> GetUserByEmailAsync(string email);
    public Task<ApplicationUser> Create(ApplicationUser user, string password);
    public Task<ApplicationUser> Update(ApplicationUser user);
    public Task<bool> Delete(Guid id);
}