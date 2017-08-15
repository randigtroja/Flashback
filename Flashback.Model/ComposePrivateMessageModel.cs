namespace Flashback.Model
{
    public class ComposePrivateMessageModel
    {
        public string Subject { get; set; }
        public string Id { get; set; }
        public bool IsQuote { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public string PostToken { get; set; }
    }
}
