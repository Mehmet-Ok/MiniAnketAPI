using MiniAnketDapper.DTOs;
using MiniAnketDapper.Models;

namespace MiniAnketDapper.Services
{
    public interface IAuthService
    {
        string Login(string username, string password);
        Task<bool> RegisterAsync(RegisterDto dto);
        Task<User?> GetUserByUsernameAsync(string username);


    }
}