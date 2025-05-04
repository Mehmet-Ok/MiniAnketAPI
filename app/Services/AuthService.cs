using System.Data;
using Dapper;
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

        public string Login(string username, string password)
        {

            var query = "SELECT id, username, password FROM users WHERE username = @Username";
            var user = _dbConnection.QuerySingleOrDefault<User>(query, new { Username = username });

            if (user == null)
            {
                return "User not found";
            }

            if (user.password != password)
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
