using System.Threading.Tasks;

namespace Scribe.Shared.Chat
{
    public interface IChatClient
    {
        Task NewMessage(NewMessage message);
    }
}