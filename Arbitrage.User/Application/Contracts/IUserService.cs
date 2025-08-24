using Arbitrage.User.Domain.Entities;

namespace Arbitrage.User.Application.Contracts
{
    public interface IUserService
    {
        Task<UserModel> RegisterAsync(string username, string password, string email, string? telegramId, string role);
        Task<string> LoginAsync(string username, string password);
        Task<UserModel> UpdateUserAsync(Guid id, string newUsername, string newRole);
        Task DeleteUserAsync(Guid id);
        Task<IEnumerable<UserModel>> GetAllUsersAsync();
        Task<UserModel> GetUserByIdAsync(Guid id);
    }
}