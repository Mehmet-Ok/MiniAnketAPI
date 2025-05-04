using System.Data;
using Dapper;
using MiniAnketDapper.DTOs;
using Newtonsoft.Json;
using Npgsql;

namespace MiniAnketDapper.Services
{
    public class PollService : IPollService
    {
        private readonly IDbConnection _db;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PollService(IDbConnection db, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> CreatePollAsync(CreatePollRequest request)
        {
            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            // HttpContext üzerinden login olmuş kullanıcının username'ini alın
            var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                throw new UnauthorizedAccessException("Kullanıcı giriş yapmamış.");
            }

            // Kullanıcı adı ile userId'yi sorgulamak için SQL sorgusunu yazın
            var getUserIdSql = "SELECT id FROM users WHERE username = @Username";
            var userId = await connection.QueryFirstOrDefaultAsync<int?>(getUserIdSql, new { Username = username });

            if (userId == null)
            {
                throw new Exception("Kullanıcı bulunamadı");
            }

            // Anketi eklemek için SQL sorgusunu yazın
            var insertPollSql = @"INSERT INTO polls (title, created_by) 
                      VALUES (@Title, @CreatedBy) 
                      RETURNING id;";

            var pollId = await connection.ExecuteScalarAsync<int>(insertPollSql, new { Title = request.Title, CreatedBy = userId });

            // Seçenekleri eklemek için SQL sorgusunu yazın
            var insertOptionSql = @"INSERT INTO poll_options (poll_id, option_text) 
                        VALUES (@PollId, @OptionText);";

            foreach (var option in request.Options)
            {
                await connection.ExecuteAsync(insertOptionSql, new { PollId = pollId, OptionText = option });
            }

            return pollId;
        }

        public async Task<bool> VoteAsync(int pollId, int optionId)
        {
            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            // Kullanıcının login olup olmadığını kontrol et
            var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                throw new UnauthorizedAccessException("Kullanıcı giriş yapmamış.");
            }

            // Kullanıcı ID'sini al
            var userId = await GetUserIdByUsernameAsync(username);
            if (userId == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            // Seçeneğin geçerli olup olmadığını kontrol et
            var isValidOption = await connection.ExecuteScalarAsync<bool>(
                "SELECT EXISTS (SELECT 1 FROM poll_options WHERE id = @OptionId AND poll_id = @PollId)",
                new { OptionId = optionId, PollId = pollId });

            if (!isValidOption)
            {
                return false;
            }

            // Oy ekle (duplicate oy verme engellendi)
            try
            {
                var sql = "INSERT INTO votes (poll_id, option_id, user_id) VALUES (@PollId, @OptionId, @UserId)";
                await connection.ExecuteAsync(sql, new { PollId = pollId, OptionId = optionId, UserId = userId });
                return true;
            }
            catch (PostgresException ex) when (ex.SqlState == "23505")
            {
                // Aynı kullanıcı aynı ankete tekrar oy veremez
                return false;
            }
        }

        public async Task<List<PollResultDto>> GetPollResultsAsync(int pollId)
        {
            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            // SQL sorgusunu yazalım
            var query = @"
                SELECT po.option_text, 
                       COALESCE(COUNT(v.id), 0) AS vote_count
                FROM poll_options po
                LEFT JOIN votes v ON po.id = v.option_id AND po.poll_id = v.poll_id
                WHERE po.poll_id = @PollId
                GROUP BY po.id, po.option_text
                ORDER BY vote_count DESC;";

            var pollResults = await connection.QueryAsync< PollResultDto>(query, new { PollId = pollId });

            // Eğer sonuçları kontrol etmek isterseniz
            foreach (var result in pollResults)
            {
                Console.WriteLine($"OptionText: {result.option_text ?? "No Option Text"}, VoteCount: {result.vote_count}");
            }

            return pollResults.ToList();
        }

        public async Task<List<PollDto>> GetPollsByUserIdAsync(int userId)
        {
            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            // Kullanıcının oluşturduğu anketleri al
            var sql = @"
        SELECT p.id, p.title, p.created_at
        FROM polls p
        WHERE p.created_by = @UserId";

            // Anketleri al
            var polls = await connection.QueryAsync<PollDto>(sql, new { UserId = userId });

            foreach (var poll in polls)
            {
                // Her bir anket için PollOption'ları al
                var optionsSql = "SELECT id, option_text FROM poll_options WHERE poll_id = @PollId";
                var options = await connection.QueryAsync<PollOptionDto>(optionsSql, new { PollId = poll.Id });
                poll.Options = options.ToList();  // Seçenekleri PollDTO'ya ekleyin
            }

            return polls.ToList();
        }





        public async Task<int?> GetUserIdByUsernameAsync(string username)
        {
            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var getUserIdSql = "SELECT id FROM users WHERE username = @Username";
            var userId = await connection.QueryFirstOrDefaultAsync<int?>(getUserIdSql, new { Username = username });

            return userId;
        }

    }
}
