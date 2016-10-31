namespace SagaDemo.Messages
{
    public class PayoutReady : IEventWithCaseNumber
    {
        public string CaseNumber { get; }

        public PayoutReady(string caseNumber)
        {
            CaseNumber = caseNumber;
        }
    }
}