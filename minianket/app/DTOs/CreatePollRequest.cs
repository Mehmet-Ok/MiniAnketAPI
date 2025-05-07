namespace MiniAnketDapper.DTOs
{
    public class CreatePollRequest
    {
        public string Title { get; set; }
        public List<string> Options { get; set; }
    }
}
