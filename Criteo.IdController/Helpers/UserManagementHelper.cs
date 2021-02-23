using System;

namespace Criteo.IdController.Helpers
{
    public interface IUserManagementHelper
    {
        Guid GenerateIfa();
    }

    public class UserManagementHelper : IUserManagementHelper
    {
        public Guid GenerateIfa()
        {
            return Guid.NewGuid();
        }
    }
}
