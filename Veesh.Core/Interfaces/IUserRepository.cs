using Veesh.Core.Common;
using Veesh.Domain.Entities;
using Veesh.Domain.Enums;

namespace Veesh.Core.Interfaces;

public interface IUserRepository
{
    public Task<Pageable<ApplicationUser>> GetAllAsync(UserQuery query);
    public Task<ApplicationUser?> GetUserByIdAsync(Guid id);
    public Task<ApplicationUser?> GetUserByUsernameAsync(string username);
    public Task<ApplicationUser?> GetUserByPhoneNumberAsync(string phoneNumber);
    public Task<ApplicationUser?> GetUserByEmailAsync(string email);
    public Task<ApplicationUser> CreateAsync(ApplicationUser user, string password, UserRole role);
    public Task<ApplicationUser> UpdateAsync(ApplicationUser user);
    public Task<bool> DeleteAsync(Guid id);
}
