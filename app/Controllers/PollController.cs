using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniAnketDapper.DTOs;
using MiniAnketDapper.Services;

namespace MiniAnketDapper.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PollController : ControllerBase
    {
        private readonly IPollService _pollService;

        public PollController(IPollService pollService)
        {
            _pollService = pollService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePoll([FromBody] CreatePollRequest request)
        {
            try
            {
                var pollId = await _pollService.CreatePollAsync(request);
                return Ok(new { PollId = pollId, Message = "Anket başarıyla oluşturuldu." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("vote/{pollId}/{optionId}")]
        [Authorize] // Kullanıcı giriş yapmış olmalı
        public async Task<IActionResult> VoteAsync(int pollId, int optionId)
        {
            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized("Kullanıcı giriş yapmamış.");
                }

                // Oy kullanma işlemini gerçekleştirmek için VoteAsync metodunu çağır
                var result = await _pollService.VoteAsync(pollId, optionId);

                if (result)
                {
                    return Ok("Oyunuz başarıyla kaydedildi.");
                }
                else
                {
                    return BadRequest("Bu ankette zaten oy verdiniz.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"İç hata: {ex.Message}");
            }
        }

        [HttpGet("results/{pollId}")]
        [Authorize] // Kullanıcı giriş yapmış olmalı
        public async Task<IActionResult> GetPollResults(int pollId)
        {
            try
            {
                // Kullanıcı giriş yapmış mı kontrol et
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized("Kullanıcı giriş yapmamış.");
                }

                // Anket sonuçlarını al
                var results = await _pollService.GetPollResultsAsync(pollId);

                if (results == null || !results.Any())
                {
                    return NotFound("Anket sonuçları bulunamadı.");
                }

                return Ok(results); // Sonuçları kullanıcıya döndür
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"İç hata: {ex.Message}");
            }
        }

        [HttpGet("my-polls")]
        [Authorize] // Kullanıcı giriş yapmış olmalı
        public async Task<IActionResult> GetMyPolls()
        {
            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized("Kullanıcı giriş yapmamış.");
                }

                var userId = await _pollService.GetUserIdByUsernameAsync(username);
                if (userId == null)
                {
                    return NotFound("Kullanıcı bulunamadı.");
                }

                // Kullanıcının oluşturduğu anketleri ve seçeneklerini al
                var polls = await _pollService.GetPollsByUserIdAsync(userId.Value);

                return Ok(polls);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"İç hata: {ex.Message}");
            }
        }



    }
}