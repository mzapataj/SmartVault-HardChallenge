using System;

namespace SmartVault.Core.BusinessObjects
{
    public partial class User : ObjectBase
    {
        public string FullName => $"{FirstName} {LastName}";
    }
}
