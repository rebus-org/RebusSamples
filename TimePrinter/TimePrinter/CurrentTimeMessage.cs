using System;

namespace TimePrinter
{
    public class CurrentTimeMessage
    {
        public DateTimeOffset Time { get; }

        public CurrentTimeMessage(DateTimeOffset time)
        {
            Time = time;
        }
    }
}