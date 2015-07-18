﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was not generated by a tool. but for stylecop suppression.
// </auto-generated>
//------------------------------------------------------------------------------
using Moq;
using StrixIT.Platform.Core;
using System.Collections.Generic;

namespace StrixIT.Platform.Modules.Membership.Tests
{
    public class RoleServiceMock
    {
        private Mock<IMembershipDataSource> _dataSourceMock = new Mock<IMembershipDataSource>();
        private Mock<IRoleManager> _roleManagerMock = new Mock<IRoleManager>();
        private IRoleService _roleService;

        public RoleServiceMock()
        {
            _roleService = new RoleService(_dataSourceMock.Object, _roleManagerMock.Object);
        }

        #region Properties

        public IRoleService RoleService
        {
            get
            {
                return _roleService;
            }
        }

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

        #endregion
    }
}