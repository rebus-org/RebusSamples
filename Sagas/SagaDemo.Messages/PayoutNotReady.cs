namespace SagaDemo.Messages
{
    public class PayoutNotReady : IEventWithCaseNumber
    {
        public string CaseNumber { get; }

        public PayoutNotReady(string caseNumber)
        {
            CaseNumber = caseNumber;
        }
    }
}