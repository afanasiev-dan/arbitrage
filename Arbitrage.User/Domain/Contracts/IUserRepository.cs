using Arbitrage.User.Domain.Entities;

namespace Arbitrage.User.Domain.Contracts
{
    public interface IUserRepository
    {
        Task<UserModel> CreateAsync(UserModel useModelr);
        Task<UserModel> GetByUsernameAsync(string username);
        Task<UserModel> GetByIdAsync(Guid id);
        Task UpdateAsync(UserModel user);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<UserModel>> GetAllAsync();
    }
}