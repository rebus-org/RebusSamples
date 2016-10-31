namespace SagaDemo.Messages
{
    public class PayoutMethodSelected : IEventWithCaseNumber
    {
        public string CaseNumber { get; }

        public PayoutMethodSelected(string caseNumber)
        {
            CaseNumber = caseNumber;
        }
    }
}