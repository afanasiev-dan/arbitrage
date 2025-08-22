using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Arbitrage.User.Application.Contracts;
using Arbitrage.User.Domain.Contracts;
using Arbitrage.User.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Arbitrage.User.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<UserModel> RegisterAsync(string username, string password, string email, string? telegramId, string role)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(username);
            if (existingUser != null) throw new Exception("User already exists");
            
            if (existingUser!.Email == email) throw new Exception("Ошиббка регистрации, пользователь с таким email уже зарегистрирован. СЛИШКОМ ИНФРОМАТИВНОЕ СООБЩЕНИЕ, ЗАМЕНИТЬ ПОЛЕ ТЕСТОВ!!!");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new UserModel { Username = username, PasswordHash = passwordHash, Email = email,Role = role };

            return await _userRepository.CreateAsync(user);
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            user.LastActive = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(UserModel user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<UserModel> UpdateUserAsync(Guid id, string newUsername, string newRole)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            user.Username = newUsername;
            user.Role = newRole;

            await _userRepository.UpdateAsync(user);
            return user;
        }

        public async Task DeleteUserAsync(Guid id)
        {
            await _userRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<UserModel> GetUserByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }
    }
}