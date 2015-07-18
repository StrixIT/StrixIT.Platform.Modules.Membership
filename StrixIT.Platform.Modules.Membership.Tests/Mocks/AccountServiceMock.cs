﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was not generated by a tool. but for stylecop suppression.
// </auto-generated>
//------------------------------------------------------------------------------
using Moq;
using StrixIT.Platform.Core;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;

namespace StrixIT.Platform.Modules.Membership.Tests
{
    public class AccountServiceMock
    {
        private Mock<IMembershipDataSource> _dataSourceMock = new Mock<IMembershipDataSource>();
        private Mock<ISecurityManager> _securityManagerMock = new Mock<ISecurityManager>();
        private Mock<IUserManager> _userManagerMock = new Mock<IUserManager>();
        private Mock<IRoleManager> _roleManagerMock = new Mock<IRoleManager>();
        private Mock<IMembershipMailer> _mailerMock = new Mock<IMembershipMailer>();
        private IAccountService _accountService;

        public AccountServiceMock()
        {
            _accountService = new AccountService(_dataSourceMock.Object, _securityManagerMock.Object, _userManagerMock.Object, _roleManagerMock.Object, _mailerMock.Object);
        }

        #region Properties

        public IAccountService AccountService
        {
            get
            {
                return _accountService;
            }
        }

        public Mock<IMembershipDataSource> DataSourceMock
        {
            get
            {
                return _dataSourceMock;
            }
        }

        public Mock<ISecurityManager> SecurityManagerMock
        {
            get
            {
                return _securityManagerMock;
            }
        }

        public Mock<IUserManager> UserManagerMock
        {
            get
            {
                return _userManagerMock;
            }
        }

        public Mock<IRoleManager> RoleManagerMock
        {
            get
            {
                return _roleManagerMock;
            }
        }

        public Mock<IMembershipMailer> MailerMock
        {
            get
            {
                return _mailerMock;
            }
        }

        #endregion
    }
}
