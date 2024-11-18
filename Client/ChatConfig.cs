namespace Scribe.Client;

public class ChatConfig(Uri uri)
{
    public Uri ChatUrl { get; } = uri;
}