using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixIT.Platform.Modules.Membership
{
    public interface IMembershipData
    {
        #region Public Properties

        IMembershipDataSource DataSource { get; }
        IGroupManager GroupManager { get; }
        IRoleManager RoleManager { get; }
        ISecurityManager SecurityManager { get; }
        IUserManager UserManager { get; }

        #endregion Public Properties
    }
}