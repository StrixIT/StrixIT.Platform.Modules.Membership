using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixIT.Platform.Modules.Membership
{
    public class MembershipData : IMembershipData
    {
        #region Private Fields

        private IMembershipDataSource _dataSource;
        private IGroupManager _groupManager;
        private IRoleManager _roleManager;
        private ISecurityManager _securityManager;
        private IUserManager _userManager;

        #endregion Private Fields

        #region Public Constructors

        public MembershipData(IMembershipDataSource dataSource, GroupManager groupManager, RoleManager roleManager, SecurityManager securityManager, UserManager userManager)
        {
            _dataSource = dataSource;
            _groupManager = groupManager;
            _roleManager = roleManager;
            _securityManager = securityManager;
            _userManager = userManager;
        }

        #endregion Public Constructors

        #region Public Properties

        public IMembershipDataSource DataSource
        {
            get
            {
                return _dataSource;
            }
        }

        public IGroupManager GroupManager
        {
            get
            {
                return _groupManager;
            }
        }

        public IRoleManager RoleManager
        {
            get
            {
                return _roleManager;
            }
        }

        public ISecurityManager SecurityManager
        {
            get
            {
                return _securityManager;
            }
        }

        public IUserManager UserManager
        {
            get
            {
                return _userManager;
            }
        }

        #endregion Public Properties
    }
}