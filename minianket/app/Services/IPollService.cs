using MiniAnketDapper.DTOs;

namespace MiniAnketDapper.Services
{
    public interface IPollService
    {
        Task<int> CreatePollAsync(CreatePollRequest request);

        Task<bool> VoteAsync(int pollId, int optionId);

        Task<List<PollResultDto>> GetPollResultsAsync(int pollId);

        Task<List<PollDto>> GetPollsByUserIdAsync(int userId);

        Task<int?> GetUserIdByUsernameAsync(string username);
    }
}
