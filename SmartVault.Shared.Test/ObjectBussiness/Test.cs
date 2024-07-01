using System.Drawing;

namespace SmartVault.Shared.Test.TestTables
{
    public class Bar
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class NoPrimaryKey
    {        
        public string Name { get; set; }
    }

    public class NotSupportedProperty
    {
        public Point Point { get; set; }
    }
}