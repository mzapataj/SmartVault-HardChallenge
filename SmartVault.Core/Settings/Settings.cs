using System;
using System.IO;

namespace SmartVault.Core.Settings
{
    public interface ISettings
    {
        string DefaultConnection { get; }
        string AppName { get; set; }

        string DatabaseFileName { get; set; }

        ConnectionStrings ConnectionStrings { get; set; }
    }

    public class Settings : ISettings
    {
        private string appDataPath => 
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public string DefaultConnection => 
            string.Format(ConnectionStrings.DefaultConnection ?? "", Path.Combine(appDataPath, AppName, DatabaseFileName));
        public string AppName { get; set; }
        public string DatabaseFileName { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set;}
    }
}
