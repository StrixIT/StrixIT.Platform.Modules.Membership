﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was not generated by a tool. but for stylecop suppression.
// </auto-generated>
//------------------------------------------------------------------------------
using Moq;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership.Tests
{
    public class RoleServiceMock
    {
        #region Private Fields

        private Mock<IMembershipDataSource> _dataSourceMock = new Mock<IMembershipDataSource>();
        private Mock<IRoleManager> _roleManagerMock = new Mock<IRoleManager>();
        private IRoleService _roleService;

        #endregion Private Fields

        #region Public Constructors

        public RoleServiceMock()
        {
            var user = new Mock<IUserContext>();
            user.Setup(m => m.Id).Returns(MembershipTestData.AdminId);
            user.Setup(m => m.GroupId).Returns(MembershipTestData.MainGroupId);
            _roleService = new RoleService(_dataSourceMock.Object, _roleManagerMock.Object, user.Object);
        }

        #endregion Public Constructors

        #region Properties

        public Mock<IMembershipDataSource> DataSourceMock
        {
            get
            {
                return _dataSourceMock;
            }
        }

        public Mock<IRoleManager> RoleManagerMock
        {
            get
            {
                return _roleManagerMock;
            }
        }

        public IRoleService RoleService
        {
            get
            {
                return _roleService;
            }
        }

        #endregion Properties
    }
}