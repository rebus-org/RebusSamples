using System;
using System.Threading.Tasks;
using Rebus.Handlers;
#pragma warning disable 1998

namespace UnitOfWork.Handlers
{
    public class FailSometimes : IHandleMessages<string>
    {
        public async Task Handle(string message)
        {
            var hashCode = message.GetHashCode();
            var remainder = hashCode%InsertRowsIntoDatabase.Modulo;

            if (remainder == 0)
            {
                throw new ApplicationException(string.Format("OMG the hash code was {0} and the remainder was {1}!", hashCode, remainder));
            }
        }
    }
}