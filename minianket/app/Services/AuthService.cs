using System.Data;
using Dapper;
using MiniAnketDapper.DTOs;
using MiniAnketDapper.Models;

namespace MiniAnketDapper.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IDbConnection _dbConnection;

        public AuthService(ITokenService tokenService, IDbConnection dbConnection)
        {
            _tokenService = tokenService;
            _dbConnection = dbConnection;
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            _dbConnection.Open();

            var existingUser = await _dbConnection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM users WHERE username = @Username", new { dto.username });

            if (existingUser != null)
                return false;

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.password);

            var sql = "INSERT INTO users (username, password) VALUES (@Username, @Password)";
            await _dbConnection.ExecuteAsync(sql, new { dto.username, password = passwordHash });

            return true;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM users WHERE username = @Username", new { Username = username });
        }

        public string Login(string username, string password)
        {

            var query = "SELECT id, username, password FROM users WHERE username = @Username";
            var user = _dbConnection.QuerySingleOrDefault<User>(query, new { Username = username });

            if (user == null)
            {
                return "User not found";
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.password))
            { 
                return "Invalid password"; 
            }
            else
            {
                return _tokenService.GenerateToken(user.username);
            }

        }
    }
}
