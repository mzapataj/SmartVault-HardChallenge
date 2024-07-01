using System;

namespace SmartVault.Core.Settings
{
    public interface ISettings
    {
        string DefaultConnection { get; }

        string DatabaseFileName { get; set; }

        ConnectionStrings ConnectionStrings { get; set; }
    }

    public class Settings : ISettings
    {
        public string DefaultConnection { get => string.Format(ConnectionStrings.DefaultConnection ?? "", DatabaseFileName); }
        public string DatabaseFileName { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set;}
    }
}
