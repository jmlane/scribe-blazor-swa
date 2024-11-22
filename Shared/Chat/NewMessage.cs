namespace Scribe.Shared.Chat
{
    public class NewMessage
    {
        public NewMessage(string userId, string message)
        {
            UserId = userId;
            Message = message;
        }

        public string UserId { get; }
        public string Message { get; }
    }
}