namespace Consumer.Messages
{
    public class Job
    {
        public char KeyChar { get; private set; }

        public Job(char keyChar)
        {
            KeyChar = keyChar;
        }
    }
}
