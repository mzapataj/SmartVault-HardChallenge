using SmartVault.Core.BusinessObjects;

namespace SmartVault.Program
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var xd = new Account()
            {
                Name = "popo"
            }
            ;
            WriteEveryThirdFileToFile(args[0]);
            GetAllFileSizes();
        }

        private static void GetAllFileSizes()
        {
            var lol = new Account();
            
            // TODO: Implement functionality
        }

        private static void WriteEveryThirdFileToFile(string accountId)
        {
            // TODO: Implement functionality
        }
    }
}