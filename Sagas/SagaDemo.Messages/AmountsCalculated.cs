namespace SagaDemo.Messages
{
    public class AmountsCalculated : IEventWithCaseNumber
    {
        public string CaseNumber { get; }

        public AmountsCalculated(string caseNumber)
        {
            CaseNumber = caseNumber;
        }
    }
}
