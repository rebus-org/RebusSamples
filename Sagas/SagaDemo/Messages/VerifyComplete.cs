namespace SagaDemo.Messages
{
    public class VerifyComplete
    {
        public string CaseNumber { get; }

        public VerifyComplete(string caseNumber)
        {
            CaseNumber = caseNumber;
        }
    }
}