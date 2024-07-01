namespace SmartVault.DataGeneration.Dto
{
    public class DataGenerationResult
    {
        public string accountData;
        public string documentData;
        public string userData;

        public DataGenerationResult(string accountCount, string documentCount, string userCount)
        {
            this.accountData = accountCount;
            this.documentData = documentCount;
            this.userData = userCount;
        }
    }
}
