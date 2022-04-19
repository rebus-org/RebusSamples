namespace PubSub.Messages;

public class StringMessage
{
    public string Text { get; }

    public StringMessage(string text)
    {
        Text = text;
    }
}