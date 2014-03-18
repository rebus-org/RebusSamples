namespace FullBlownConsoleApplication.Messages
{
    public class SomeRandomMessage
    {
        public SomeRandomMessage(string greeting)
        {
            Greeting = greeting;
        }

        public string Greeting { get; private set; }
    }
}