﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was not generated by a tool. but for stylecop suppression.
// </auto-generated>
//------------------------------------------------------------------------------
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrixIT.Platform.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrixIT.Platform.Modules.Membership.Tests
{
    [TestClass()]
    public class GroupManagerTests
    {
        #region Private Fields

        private Mock<IUserContext> _userContextMock;

        #endregion Private Fields

        #region Public Methods

        [TestCleanup]
        public void Cleanup()
        {
            DependencyInjector.Injector = null;
        }

        [TestInitialize]
        public void Init()
        {
            _userContextMock = TestHelpers.MockUserContext();
            DependencyInjector.Injector = new Mock<IDependencyInjector>().Object;
        }

        #endregion Public Methods

        #region Save

        [TestMethod()]
        public void CreateGroupShouldSaveANewGroup()
        {
            var mock = new GroupManagerMock();
            var group = mock.GroupManager.Create("Test", false);
            Assert.IsNotNull(group);
            mock.DataSourceMock.Mock.Verify(m => m.Save<Group>(It.IsAny<Group>()), Times.Once());
        }

        [TestMethod()]
        public void UpdateGroupWithNewNameShouldUpdateName()
        {
            var mock = new GroupManagerMock();
            var group = mock.GroupManager.Update(MembershipTestData.DivingGroupId, "Test", false);
            Assert.AreEqual("Test", group.Name);
        }

        #endregion Save

        #region Delete

        [TestMethod()]
        public void DeleteGroupForInvalidIdShouldNotHitDataSource()
        {
            var mock = new GroupManagerMock();
            mock.GroupManager.Delete(Guid.NewGuid());
            mock.DataSourceMock.Mock.Verify(d => d.Delete<Group>(It.IsAny<Group>()), Times.Never());
        }

        [TestMethod()]
        public void DeleteGroupForValidIdShouldDeleteTheGroupItsRolesAndItsProfileFields()
        {
            var mock = new GroupManagerMock();
            var group = mock.DataSourceMock.DataList<Group>().First(g => g.Id == MembershipTestData.DivingGroupId);
            mock.GroupManager.Delete(MembershipTestData.DivingGroupId);
            mock.DataSourceMock.Mock.Verify(d => d.Delete<ICollection<GroupInRole>>(It.IsAny<ICollection<GroupInRole>>()), Times.Once());
            mock.DataSourceMock.Mock.Verify(d => d.Delete<Group>(group), Times.Once());
        }

        #endregion Delete

        #region Exists

        [TestMethod()]
        public void ExistsShouldReturnFalseWhenGroupWithNameDoesNotExist()
        {
            var mock = new GroupManagerMock();
            var result = mock.GroupManager.Exists("Test", null);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ExistsShouldReturnFalseWhenGroupWithNameExistsButIdIsGroupsOwnId()
        {
            var mock = new GroupManagerMock();
            var result = mock.GroupManager.Exists(MembershipTestData.DivingGroup.Name, MembershipTestData.DivingGroupId);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ExistsShouldReturnTrueWhenGroupWithNameExists()
        {
            var mock = new GroupManagerMock();
            var result = mock.GroupManager.Exists(MembershipTestData.DivingGroup.Name, null);
            Assert.IsTrue(result);
        }

        #endregion Exists

        #region Get

        [TestMethod()]
        public void GetGroupByIdShouldReturnGroup()
        {
            var mock = new GroupManagerMock();
            var result = mock.GroupManager.Get(MembershipTestData.DivingGroupId);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void GetGroupByNameShouldReturnGroup()
        {
            var mock = new GroupManagerMock();
            var result = mock.GroupManager.Get(MembershipTestData.DivingGroup.Name);
            Assert.IsNotNull(result);
        }

        #endregion Get

        #region Query

        [TestMethod()]
        public void QueryShouldReturnAllGroups()
        {
            var mock = new GroupManagerMock();
            var result = mock.GroupManager.Query();
            Assert.AreEqual(4, result.Count());
        }

        #endregion Query

        #region Private Classes

        private class GroupSaveHandler : IHandlePlatformEvent<GeneralEvent>
        {
            #region Public Methods

            public void Handle(GeneralEvent args)
            {
                return;
            }

            #endregion Public Methods
        }

        #endregion Private Classes
    }
}