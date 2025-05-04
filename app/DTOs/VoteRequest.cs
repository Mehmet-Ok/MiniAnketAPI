namespace MiniAnketDapper.DTOs
{
    public class VoteRequest
    {
        public int PollId { get; set; }
        public int OptionId { get; set; }
    }
}
