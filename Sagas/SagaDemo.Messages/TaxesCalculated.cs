namespace SagaDemo.Messages
{
    public class TaxesCalculated : IEventWithCaseNumber
    {
        public string CaseNumber { get; }

        public TaxesCalculated(string caseNumber)
        {
            CaseNumber = caseNumber;
        }
    }
}