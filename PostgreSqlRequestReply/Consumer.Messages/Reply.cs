namespace Consumer.Messages
{
    public class Reply
    {
        public char KeyChar { get; private set; }
        public int OsProcessId { get; private set; }

        public Reply(char keyChar, int osProcessId)
        {
            KeyChar = keyChar;
            OsProcessId = osProcessId;
        }
    }
}