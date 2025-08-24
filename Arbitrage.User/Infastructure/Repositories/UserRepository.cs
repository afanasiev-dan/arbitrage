using Arbitrage.User.Domain.Contracts;
using Arbitrage.User.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Arbitrage.User.Infastructure.Repositories
{
    public class UserRepository(
        ILogger<UserRepository> logger,
        DbContext context
    ) : IUserRepository
    {
        private readonly DbContext _context = context;
        private readonly ILogger<UserRepository> _logger = logger;

        public async Task<UserModel> CreateAsync(UserModel user)
        {
            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.UtcNow;
            _context.Set<UserModel>().Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<UserModel> GetByUsernameAsync(string username)
        {
            try
            {
                var user = await _context.Set<UserModel>().FirstOrDefaultAsync(u => u.Username == username);

                if (user != null)
                    _logger.LogInformation($"Пользователь найден: {user.Username}, ID: {user.Id}");
                else
                    _logger.LogInformation($"Пользователь с именем {username} не найден");

                return user; // Возвращаем null, если пользователь не найден
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при получении пользователя: " + ex.Message);
                throw;
            }
        }

        public async Task<UserModel> GetByIdAsync(Guid id)
        {
            return await _context.Set<UserModel>().FindAsync(id) ?? null;
        }

        public async Task UpdateAsync(UserModel user)
        {
            _context.Set<UserModel>().Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _context.Set<UserModel>().FindAsync(id);
            if (user != null)
            {
                _context.Set<UserModel>().Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<UserModel>> GetAllAsync()
        {
            return await _context.Set<UserModel>().AsNoTracking().ToListAsync();
        }
    }
}