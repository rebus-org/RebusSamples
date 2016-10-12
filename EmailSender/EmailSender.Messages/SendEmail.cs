namespace EmailSender.Messages
{
    public class SendEmail
    {
        public string Recipient { get; }
        public string Subject { get; }
        public string Body { get; }

        public SendEmail(string recipient, string subject, string body)
        {
            Recipient = recipient;
            Subject = subject;
            Body = body;
        }
    }
}
