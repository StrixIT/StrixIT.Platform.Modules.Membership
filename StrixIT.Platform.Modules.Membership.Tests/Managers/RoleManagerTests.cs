﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was not generated by a tool. but for stylecop suppression.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership.Tests
{
    [TestClass]
    public class RoleManagerTests
    {
        private Mock<IUserContext> _userContextMock;

        [TestInitialize]
        public void Init()
        {
            _userContextMock = TestHelpers.MockUserContext();
            StrixPlatform.Environment = new DefaultEnvironment();
            StrixPlatform.MainGroupId = MembershipTestData.MainGroupId;
            Logger.LoggingService = new Mock<ILoggingService>().Object;
        }

        [TestCleanup]
        public void Cleanup()
        {
            StrixPlatform.Environment = null;
            Logger.LoggingService = null;
            StrixPlatform.MainGroupId = MembershipTestData.MainGroupId;
        }

        #region Create

        [TestMethod()]
        public void CreateShouldReturnExistingRoleWhenARoleAlreadyExists()
        {
            var mock = new RoleManagerMock();
            var result = mock.RoleManager.Create(MembershipTestData.GroupAdminRoleName, "test", new List<Permission> { new Permission("one"), new Permission("two") });
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void CreateShouldReturnTrueWhenANewRoleIsAddedWithDescriptionAndPermissions()
        {
            var mock = new RoleManagerMock();
            mock.DataSourceMock.Mock.Setup(m => m.Save(It.IsAny<Role>())).Returns<Role>(r => r);
            var result = mock.RoleManager.Create("Test B", "test", new List<Permission> { new Permission("one"), new Permission("two") });
            Assert.IsNotNull(result);
            Assert.AreEqual("test", result.Description);
            Assert.AreEqual(2, result.Permissions.Count);
            mock.DataSourceMock.Mock.Verify(m => m.Save(It.IsAny<Role>()), Times.Once());
        }

        #endregion

        #region Update

        [TestMethod()]
        public void UpdateShouldUpdateTheRoleDescriptionAndPermissionsAndHitSave()
        {
            var mock = new RoleManagerMock();
            mock.DataSourceMock.Mock.Setup(m => m.Save(It.IsAny<Role>())).Returns<Role>(r => r);
            var result = mock.RoleManager.Update(MembershipTestData.GroupAdminRoleId, "test", "test", new List<Permission> { new Permission("one"), new Permission("two") } );
            Assert.IsNotNull(result);
            Assert.AreEqual("test", result.Description);
            Assert.AreEqual(2, result.Permissions.Count);
            mock.DataSourceMock.Mock.Verify(m => m.Save(It.IsAny<Role>()), Times.Once());
        }

        #endregion

        #region Exists

        public void ExistsShouldReturnFalseWhenGroupWithNameDoesNotExist()
        {
            var mock = new RoleManagerMock();
            var result = mock.RoleManager.Exists("Test", null);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ExistsShouldReturnTrueWhenGroupWithNameExists()
        {
            var mock = new RoleManagerMock();
            var result = mock.RoleManager.Exists(MembershipTestData.GroupAdminRoleName, null);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void ExistsShouldReturnFalseWhenGroupWithNameExistsButIdIsGroupsOwnId()
        {
            var mock = new RoleManagerMock();
            var result = mock.RoleManager.Exists(MembershipTestData.UserRoleName, MembershipTestData.UserRoleId);
            Assert.IsFalse(result);
        }

        #endregion

        #region IsUserInRole

        [TestMethod()]
        public void IsUserInRoleShouldReturnTrueWhenUserIsInRole()
        {
            var mock = new RoleManagerMock();
            var result = mock.RoleManager.IsUserInRole(MembershipTestData.AdminId, PlatformConstants.ADMINROLE);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void IsUserInRoleShouldReturnFalseWhenUserIsNotInRole()
        {
            var mock = new RoleManagerMock();
            var result = mock.RoleManager.IsUserInRole(MembershipTestData.AdminId, MembershipTestData.EditorRoleName);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsUserInRoleShouldReturnFalseWhenUserIsInRoleButMembershipExpired()
        {
            var mock = new RoleManagerMock();
            var result = mock.RoleManager.IsUserInRole(MembershipTestData.DivingManagerId, MembershipTestData.EditorRoleName);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsUserInRoleShouldReturnFalseWhenUserIsInRoleButMembershipNotYetStarted()
        {
            var mock = new RoleManagerMock();
            var result = mock.RoleManager.IsUserInRole(MembershipTestData.KarateManagerId, PlatformConstants.GROUPADMINROLE);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsUserInRolesShouldReturnTrueWhenUserIsInOneOfTheRoles()
        {
            var mock = new RoleManagerMock();
            var result = mock.RoleManager.IsUserInRoles(MembershipTestData.AdminId, new string[] { PlatformConstants.ADMINROLE, PlatformConstants.GROUPADMINROLE });
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void IsUserInRolesShouldReturnFalseWhenUserIsInNoneOfTheRoles()
        {
            var mock = new RoleManagerMock();
            var result = mock.RoleManager.IsUserInRoles(MembershipTestData.DivingEditorId, new string[] { PlatformConstants.ADMINROLE, PlatformConstants.GROUPADMINROLE });
            Assert.IsFalse(result);
        }

        #endregion

        #region Get

        [TestMethod()]
        public void GetRoleShouldReturnRoleWhenAccessible()
        {
            var mock = new RoleManagerMock();
            var result = mock.RoleManager.Get(MembershipTestData.EditorRoleId);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void GetRoleShouldReturnNullWhenNotAccessible()
        {
            var mock = new RoleManagerMock();
            var result = mock.RoleManager.Get(MembershipTestData.AdminRoleId);
            Assert.IsNull(result);
        }

        #endregion

        #region Get Roles

        [TestMethod()]
        public void GetAllRolesShouldReturnAllRolesForCurrentAppForMainGroup()
        {
            var mock = new RoleManagerMock();
            _userContextMock.Setup(m => m.IsInMainGroup).Returns(true);
            var result = mock.RoleManager.Query().ToList();
            _userContextMock.Setup(m => m.IsInMainGroup).Returns(false);
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod()]
        public void GetAllRolesShouldReturnAllRolesExceptAdminRoleForCurrentAppForOtherGroups()
        {
            var mock = new RoleManagerMock();
            var result = mock.RoleManager.Query();
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod()]
        public void GetRolesForGroupShouldReturnAllRolesForGroup()
        {
            var mock = new RoleManagerMock();
            mock.DataSourceMock.RegisterData<Group>(MembershipTestData.Groups);
            mock.FixRelations();
            var result = mock.RoleManager.QueryForGroup(MembershipTestData.DivingGroupId);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod()]
        public void GetRolesForUserShouldReturnAllRolesForUser()
        {
            var mock = new RoleManagerMock();
            mock.FixRelations();
            var result = mock.RoleManager.QueryForUser(MembershipTestData.DivingManagerId);
            Assert.AreEqual(2, result.Count());
        }

        #endregion

        #region Delete

        [TestMethod()]
        public void DeleteRoleShouldNotHitDataSourceDeleteWhenDeletingAdminRole()
        {
            var mock = new RoleManagerMock();
            _userContextMock.Setup(m => m.IsInMainGroup).Returns(true);
            mock.RoleManager.Delete(MembershipTestData.AdminRoleId);
            _userContextMock.Setup(m => m.IsInMainGroup).Returns(false);
            mock.DataSourceMock.Mock.Verify(m => m.Delete(It.IsAny<Role>()), Times.Never());
        }

        [TestMethod()]
        public void DeleteRoleShouldHitDataSourceDeleteWhenDeletingAccessibleNonAdminRole()
        {
            var mock = new RoleManagerMock();
            _userContextMock.Setup(m => m.GroupId).Returns(MembershipTestData.DivingGroupId);
            mock.RoleManager.Delete(MembershipTestData.DivingPermissionSetRoleId);
            _userContextMock.Setup(m => m.GroupId).Returns(MembershipTestData.MainGroupId);
            mock.DataSourceMock.Mock.Verify(m => m.Delete(It.IsAny<Role>()), Times.Once());
        }

        #endregion

        #region AddToRole

        [TestMethod()]
        public void AddGroupToNewRoleShouldSaveNewGroupInRoleEntity()
        {
            var mock = new RoleManagerMock();
            mock.RoleManager.AddGroupToRole(MembershipTestData.KarateGroupId, MembershipTestData.UserRoleName);
            mock.DataSourceMock.Mock.Verify(m => m.Save<GroupInRole>(It.IsAny<GroupInRole>()), Times.Once());
        }

        [TestMethod()]
        public void AddGroupToNewRoleWithInitialDateTimesShouldSetCorrectDatesAndSaveNewGroupInRoleEntity()
        {
            var mock = new RoleManagerMock();
            mock.RoleManager.AddGroupToRole(MembershipTestData.KarateGroupId, MembershipTestData.UserRoleName, new DateTime(), new DateTime());
            mock.DataSourceMock.Mock.Verify(m => m.Save<GroupInRole>(It.Is<GroupInRole>(g => g.StartDate != new DateTime() && g.EndDate != new DateTime())), Times.Once());
        }

        [TestMethod()]
        public void AddGroupToExistingRoleShouldUpdateGroupInRoleEntity()
        {
            var mock = new RoleManagerMock();
            var startDate = DateTime.Now;
            var groupRole = mock.DataSourceMock.DataList<GroupInRole>().First(g => g.GroupId == MembershipTestData.KarateGroupId && g.RoleId == MembershipTestData.GroupAdminRoleId);
            mock.RoleManager.AddGroupToRole(MembershipTestData.KarateGroupId, MembershipTestData.GroupAdminRoleName, startDate.AddDays(5), startDate.AddDays(10));
            Assert.AreEqual(startDate.AddDays(5), groupRole.StartDate);
            Assert.AreEqual(startDate.AddDays(10), groupRole.EndDate);
        }

        [TestMethod()]
        public void AddGroupToExistingRoleWithNullStartDateShouldNotUpdateStartDateInGroupInRoleEntity()
        {
            var mock = new RoleManagerMock();
            var startDate = DateTime.Now;
            var groupRole = mock.DataSourceMock.DataList<GroupInRole>().First(g => g.GroupId == MembershipTestData.KarateGroupId && g.RoleId == MembershipTestData.GroupAdminRoleId);
            mock.RoleManager.AddGroupToRole(MembershipTestData.KarateGroupId, MembershipTestData.GroupAdminRoleName);
            Assert.AreNotEqual(startDate.Date, groupRole.StartDate.Date);
        }

        [TestMethod()]
        public void AddGroupToExistingRoleWithLaterStartDateAndEarlierEndDateShouldUpdateStartDateAndEndDatesForUserEntries()
        {
            var mock = new RoleManagerMock();
            var groupRole = mock.DataSourceMock.DataList<GroupInRole>().First(g => g.GroupId == MembershipTestData.KarateGroupId && g.RoleId == MembershipTestData.GroupAdminRoleId);
            var userRoles = mock.DataSourceMock.DataList<User>().SelectMany(u => u.Roles).Where(g => g.GroupRoleGroupId == MembershipTestData.KarateGroupId && g.GroupRoleRoleId == MembershipTestData.GroupAdminRoleId).ToList();
            var newStartDate = groupRole.StartDate.AddDays(2);
            var newEndDate = groupRole.EndDate.Value.AddDays(-2);

            mock.RoleManager.AddGroupToRole(MembershipTestData.KarateGroupId, MembershipTestData.GroupAdminRoleName, newStartDate, newEndDate);

            foreach (var entry in userRoles)
            {
                Assert.AreEqual(newStartDate, entry.StartDate);
                Assert.AreEqual(newEndDate, entry.EndDate);
            }
        }

        [TestMethod()]
        public void AddGroupToExistingRoleWithEarlierStartDateAndLaterEndDateShouldUpdateStartDateAndEndDatesForUserEntriesWithMatchingStartOrEndDates()
        {
            var mock = new RoleManagerMock();
            var groupRole = mock.DataSourceMock.DataList<GroupInRole>().First(g => g.GroupId == MembershipTestData.DivingGroupId && g.RoleId == MembershipTestData.EditorRoleId);
            var userRoles = mock.DataSourceMock.DataList<User>().SelectMany(u => u.Roles).Where(g => g.GroupRoleGroupId == MembershipTestData.DivingGroupId && g.GroupRoleRoleId == MembershipTestData.EditorRoleId).ToList();
            userRoles.First().StartDate = groupRole.StartDate;
            userRoles.First().EndDate = groupRole.EndDate.Value.AddDays(-5);
            userRoles.Last().StartDate = groupRole.StartDate.AddDays(2);
            userRoles.Last().EndDate = groupRole.EndDate;   
            var newStartDate = groupRole.StartDate.AddDays(-2);
            var newEndDate = groupRole.EndDate.Value.AddDays(2);
            mock.RoleManager.AddGroupToRole(MembershipTestData.DivingGroupId, MembershipTestData.EditorRoleName, newStartDate, newEndDate);
            Assert.AreEqual(userRoles.First().StartDate, groupRole.StartDate);
            Assert.AreNotEqual(userRoles.First().EndDate, groupRole.EndDate);
            Assert.AreNotEqual(userRoles.Last().StartDate, groupRole.StartDate);
            Assert.AreEqual(userRoles.Last().EndDate, groupRole.EndDate);
        }

        [TestMethod()]
        [ExpectedException(typeof(StrixMembershipException))]
        public void AddNonMainGroupToAdminRoleShouldThrowException()
        {
            var mock = new RoleManagerMock();
            mock.RoleManager.AddGroupToRole(MembershipTestData.KarateGroupId, MembershipTestData.AdminRoleName);
        }

        [TestMethod()]
        public void AddMainGroupToAdminRoleIsAllowed()
        {
            var mock = new RoleManagerMock();
            _userContextMock.Setup(m => m.IsInMainGroup).Returns(true);
            mock.RoleManager.AddGroupToRole(MembershipTestData.KarateGroupId, MembershipTestData.AdminRoleName);
            _userContextMock.Setup(m => m.IsInMainGroup).Returns(false);
        }

        [TestMethod()]
        public void AddUserToNewRoleShouldSaveNewUserInRoleEntity()
        {
            var mock = new RoleManagerMock();
            var groupRole = new GroupInRole(MembershipTestData.KarateGroupId, MembershipTestData.UserRoleId);
            mock.DataSourceMock.AddDataItem<GroupInRole>(groupRole);
            mock.RoleManager.AddUserToRole(MembershipTestData.KarateGroupId, MembershipTestData.KarateManagerId, MembershipTestData.UserRole.Name);
            mock.DataSourceMock.Mock.Verify(m => m.Save<UserInRole>(It.IsAny<UserInRole>()), Times.Once());
        }

        [TestMethod()]
        [ExpectedException(typeof(StrixMembershipException))]
        public void AddUserToRoleThatHisGroupHasNoAccessToShouldThrowException()
        {
            var mock = new RoleManagerMock();
            mock.RoleManager.AddUserToRole(MembershipTestData.KarateGroupId, MembershipTestData.KarateManagerId, MembershipTestData.AdminRoleName);
        }

        [TestMethod()]
        public void AddUserToExistingRoleShouldUpdateUserInRoleEntity()
        {
            var mock = new RoleManagerMock();
            var startDate = DateTime.Now;
            var userRole = mock.DataSourceMock.DataList<UserInRole>().First(g => g.UserId == MembershipTestData.KarateManagerId && g.GroupRoleGroupId == MembershipTestData.KarateGroupId && g.GroupRoleRoleId == MembershipTestData.GroupAdminRoleId);
            mock.RoleManager.AddUserToRole(MembershipTestData.KarateGroupId, MembershipTestData.KarateManagerId, MembershipTestData.GroupAdminRoleName, startDate.AddDays(5), startDate.AddDays(10));
            Assert.AreEqual(startDate.AddDays(5), userRole.StartDate);
            Assert.AreEqual(startDate.AddDays(10), userRole.EndDate);
        }

        [TestMethod()]
        public void AddUserToExistingRoleWithNullStartDateShouldNotUpdateStartDateInUserInRoleEntity()
        {
            var mock = new RoleManagerMock();
            var startDate = DateTime.Now;
            var userRole = mock.DataSourceMock.DataList<UserInRole>().First(g => g.UserId == MembershipTestData.KarateManagerId && g.GroupRoleGroupId == MembershipTestData.KarateGroupId && g.GroupRoleRoleId == MembershipTestData.GroupAdminRoleId);
            mock.RoleManager.AddUserToRole(MembershipTestData.KarateGroupId, MembershipTestData.KarateManagerId, MembershipTestData.GroupAdminRole.Name);
            Assert.AreNotEqual(startDate, userRole.StartDate);
        }

        [TestMethod()]
        public void AddUserToExistingRoleWithStartDateBeforeStartDateOfGroupShouldSetStartDateToGroupStartDate()
        {
            var mock = new RoleManagerMock();
            var startDate = DateTime.Now;
            var userRole = mock.DataSourceMock.DataList<UserInRole>().First(g => g.UserId == MembershipTestData.KarateManagerId && g.GroupRoleGroupId == MembershipTestData.KarateGroupId && g.GroupRoleRoleId == MembershipTestData.GroupAdminRoleId);
            mock.RoleManager.AddUserToRole(MembershipTestData.KarateGroupId, MembershipTestData.KarateManagerId, MembershipTestData.GroupAdminRole.Name, startDate);
            Assert.AreNotEqual(startDate, userRole.StartDate);
        }

        [TestMethod()]
        public void AddUserToExistingRoleWithEndDateAfterEndDateOfGroupShouldSetEndDateToGroupEndDate()
        {
            var mock = new RoleManagerMock();
            var groupRole = mock.DataSourceMock.DataList<GroupInRole>().First(g => g.GroupId == MembershipTestData.KarateGroupId && g.RoleId == MembershipTestData.EditorRoleId);
            var endDate = DateTime.Now.AddDays(10);
            groupRole.EndDate = DateTime.Now.AddDays(3);
            var userRole = mock.DataSourceMock.DataList<UserInRole>().First(g => g.UserId == MembershipTestData.KarateManagerId && g.GroupRoleGroupId == MembershipTestData.KarateGroupId && g.GroupRoleRoleId == MembershipTestData.EditorRoleId);
            mock.RoleManager.AddUserToRole(MembershipTestData.KarateGroupId, MembershipTestData.KarateManagerId, MembershipTestData.EditorRoleName, null, DateTime.Now.AddDays(10));
            Assert.AreNotEqual(endDate, userRole.EndDate);
        }

        [TestMethod()]
        public void AddUserToExistingRoleWithoutEndDateWhileGroupHasAnEndDateShouldSetEndDateToGroupEndDate()
        {
            var mock = new RoleManagerMock();
            var groupRole = mock.DataSourceMock.DataList<GroupInRole>().First(g => g.GroupId == MembershipTestData.KarateGroupId && g.RoleId == MembershipTestData.EditorRoleId);
            groupRole.EndDate = DateTime.Now.AddDays(3);
            var userRole = mock.DataSourceMock.DataList<UserInRole>().First(g => g.UserId == MembershipTestData.KarateManagerId && g.GroupRoleGroupId == MembershipTestData.KarateGroupId && g.GroupRoleRoleId == MembershipTestData.EditorRoleId);
            mock.RoleManager.AddUserToRole(MembershipTestData.KarateGroupId, MembershipTestData.KarateManagerId, MembershipTestData.EditorRoleName, null, null);
            Assert.AreEqual(groupRole.EndDate, userRole.EndDate);
        }

        [TestMethod()]
        public void AddMainGroupUserToRoleThatNoGroupRecordExistsForShouldAddThatRecord()
        {
            var mock = new RoleManagerMock();
            _userContextMock.Setup(m => m.IsInMainGroup).Returns(true);
            mock.RoleManager.AddUserToRole(MembershipTestData.MainGroupId, MembershipTestData.GeneralManagerId, MembershipTestData.UserRoleName);
            _userContextMock.Setup(m => m.IsInMainGroup).Returns(false);
            mock.DataSourceMock.Mock.Verify(m => m.Save(It.IsAny<GroupInRole>()), Times.Once());
            mock.DataSourceMock.Mock.Verify(m => m.Save(It.IsAny<UserInRole>()), Times.Once());
        }

        [TestMethod()]
        [ExpectedException(typeof(StrixMembershipException))]
        public void AddUserToRoleThatNoMoreLicensesAreAvailableForShouldThrowException()
        {
            var mock = new RoleManagerMock();
            var role = mock.DataSourceMock.DataList<GroupInRole>().First(g => g.GroupId == MembershipTestData.DivingGroupId && g.RoleId == MembershipTestData.GroupAdminRoleId);
            role.MaxNumberOfUsers = 2;
            role.CurrentNumberOfUsers = 2;
            mock.RoleManager.AddUserToRole(MembershipTestData.DivingGroupId, MembershipTestData.GeneralManagerId, MembershipTestData.GroupAdminRoleName);
        }

        #endregion

        #region RemoveFromRole

        [TestMethod()]
        public void RemoveGroupFromRoleShouldRemoveGroupInRoleEntity()
        {
            var mock = new RoleManagerMock();
            mock.RoleManager.RemoveGroupFromRoles(MembershipTestData.KarateGroupId, new string[] { MembershipTestData.GroupAdminRoleName });
            mock.DataSourceMock.Mock.Verify(m => m.Delete(It.IsAny<IEnumerable<GroupInRole>>()), Times.Once());
        }

        [TestMethod()]
        public void RemoveUserFromRoleShouldRemoveUserInRoleEntityAndUpdateGroupInRoleUserCount()
        {
            var mock = new RoleManagerMock();
            var groupInRole = mock.DataSourceMock.DataList<GroupInRole>().First(g => g.GroupId == MembershipTestData.KarateGroupId && g.RoleId == MembershipTestData.GroupAdminRoleId);
            var oldCount = groupInRole.CurrentNumberOfUsers;
            mock.RoleManager.RemoveUsersFromRoles(new Guid[] { MembershipTestData.KarateManagerId }, new string[] { MembershipTestData.GroupAdminRoleName });
            mock.DataSourceMock.Mock.Verify(m => m.Delete(It.IsAny<IEnumerable<UserInRole>>()), Times.Once());
            Assert.AreEqual(oldCount - 1, groupInRole.CurrentNumberOfUsers);
        }

        #endregion

        [TestMethod]
        public void GroupUsesPermissionsShouldReturnTrueWhenGroupUsesPermissions()
        {
            var mock = new RoleManagerMock();
            var group = mock.DataSourceMock.DataList<Group>().First(g => g.Id == MembershipTestData.DivingGroupId);
            group.UsePermissions = true;
            var result = mock.RoleManager.GroupUsesPermissions(MembershipTestData.DivingGroupId);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetPermissionSetForGroupShouldReturnProperPermissionSet()
        {
            var mock = new RoleManagerMock();
            var role = mock.DataSourceMock.DataList<Role>().First(r => r.Id == MembershipTestData.DivingPermissionSetRoleId);
            role.Groups.Add(MembershipTestData.DivingGroupPermissionSet);
            var result = mock.RoleManager.GetPermissionSetForGroup(MembershipTestData.DivingGroupId);
            Assert.IsNotNull(result);
            Assert.AreEqual(MembershipTestData.DivingGroupId, result.GroupId);
            Assert.AreEqual(Resources.DefaultValues.PermissionSetName, result.Role.Name);
        }
    }
}
