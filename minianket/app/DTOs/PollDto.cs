namespace MiniAnketDapper.DTOs
{
    public class PollDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<PollOptionDto> Options { get; set; } = new List<PollOptionDto>();
    }
}
