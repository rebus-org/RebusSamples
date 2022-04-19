using System;

namespace PubSub.Messages;

public class DateTimeMessage
{
    public DateTime DateTime { get; }

    public DateTimeMessage(DateTime dateTime)
    {
        DateTime = dateTime;
    }
}