using System;

namespace Messages
{
    public class Job
    {
        public Job(int jobNumber, Guid jobUuid)
        {
            JobNumber = jobNumber;
            JobUuid = jobUuid;
        }

        public int JobNumber { get; }

        public Guid JobUuid { get; }
    }
}
