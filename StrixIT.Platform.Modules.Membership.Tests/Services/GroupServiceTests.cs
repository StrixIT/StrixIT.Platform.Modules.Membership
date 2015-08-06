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
    [TestClass]
    public class GroupServiceTests
    {
        #region Public Methods

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            DataMapper.CreateMap<GroupInRole, AssignRoleModel>().ForMember(ar => ar.Id, c => c.MapFrom(gr => gr.RoleId));
        }

        [TestInitialize]
        public void Init()
        {
            StrixPlatform.ApplicationId = MembershipTestData.AppId;
        }

        #endregion Public Methods

        #region Get

        [TestMethod()]
        public void GetNewShouldRetrieveFilledGroupModel()
        {
            var mock = new GroupServiceMock();
            mock.DataSourceMock.Setup(m => m.Query<Group>()).Returns(MembershipTestData.Groups.AsQueryable());
            mock.GroupManagerMock.Setup(g => g.Get(MembershipTestData.MainGroupId)).Returns(MembershipTestData.MainGroup);
            mock.RoleManagerMock.Setup(r => r.Query()).Returns(MembershipTestData.Roles.Where(r => r.Permissions.Any(p => p.ApplicationId == MembershipTestData.AppId)).AsQueryable());
            mock.RoleManagerMock.Setup(r => r.QueryForGroup(MembershipTestData.MainGroupId)).Returns(MembershipTestData.GroupsInRoles.Where(g => g.GroupId == MembershipTestData.MainGroupId).Select(g => g.Role).Map<AssignRoleModel>().AsQueryable());
            mock.RoleManagerMock.Setup(p => p.PermissionQuery()).Returns(MembershipTestData.Permissions.Where(p => p.ApplicationId == MembershipTestData.AppId).AsQueryable());
            var result = mock.GroupService.Get(Guid.Empty);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Roles.Count);
            Assert.IsFalse(result.Roles.Any(r => r.Name == Resources.DefaultValues.PermissionSetName));
            Assert.AreEqual(0, result.Roles.Where(r => r.Selected).Count());
            Assert.IsFalse(result.Roles.Any(r => r.Name.ToLower() == MembershipTestData.AdminRoleName.ToLower()));
            Assert.IsTrue(new int[] { 0, 1, 2 }.Except(result.Roles.Select(r => r.Index)).Count() == 0);
            Assert.AreEqual(4, result.Permissions.Count);
        }

        [TestMethod()]
        public void GetShouldRetrieveFilledGroupModelForUsingPermissions()
        {
            var mock = new GroupServiceMock();
            mock.DataSourceMock.Setup(m => m.Query<Group>()).Returns(MembershipTestData.Groups.AsQueryable());
            mock.GroupManagerMock.Setup(g => g.Get(MembershipTestData.DivingGroupId)).Returns(MembershipTestData.DivingGroup);
            mock.RoleManagerMock.Setup(r => r.Query()).Returns(MembershipTestData.Roles.Where(r => r.Permissions.Any(p => p.ApplicationId == MembershipTestData.AppId)).AsQueryable());
            var groupRoles = MembershipTestData.GroupsInRoles;
            var groupInRole = new GroupInRole(MembershipTestData.DivingGroupId, MembershipTestData.DivingPermissionSetRoleId);
            groupInRole.Role = MembershipTestData.DivingPermissionSetRole;
            groupRoles.Add(groupInRole);
            var groupRoleQuery = groupRoles.Where(g => g.GroupId == MembershipTestData.DivingGroupId).Select(g => g.Role).Map<AssignRoleModel>().AsQueryable();
            mock.RoleManagerMock.Setup(r => r.QueryForGroup(MembershipTestData.DivingGroupId)).Returns(groupRoleQuery);
            mock.RoleManagerMock.Setup(p => p.PermissionQuery()).Returns(MembershipTestData.Permissions.Where(p => p.ApplicationId == MembershipTestData.AppId).AsQueryable());
            mock.RoleManagerMock.Setup(p => p.GetPermissionSetForGroup(MembershipTestData.DivingGroupId)).Returns(groupInRole);
            mock.UserMock.Setup(m => m.GroupId).Returns(MembershipTestData.DivingGroupId);
            var result = mock.GroupService.Get(MembershipTestData.DivingGroupId);
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Roles.Count);
            Assert.AreEqual(2, result.Roles.Where(r => r.Selected).Count());
            Assert.AreEqual(4, result.Permissions.Count);
            Assert.AreEqual(1, result.Permissions.Where(r => r.Selected).Count());
            Assert.AreEqual(MembershipTestData.CreateUserPermission.Name, result.Permissions.First(r => r.Selected).Name);
        }

        [TestMethod()]
        public void GetShouldRetrieveFilledGroupModelForUsingRoles()
        {
            var mock = new GroupServiceMock();
            mock.DataSourceMock.Setup(m => m.Query<Group>()).Returns(MembershipTestData.Groups.AsQueryable());
            mock.GroupManagerMock.Setup(g => g.Get(MembershipTestData.DivingGroupId)).Returns(MembershipTestData.DivingGroup);
            mock.RoleManagerMock.Setup(r => r.Query()).Returns(MembershipTestData.Roles.Where(r => r.Permissions.Any(p => p.ApplicationId == MembershipTestData.AppId)).AsQueryable());
            mock.RoleManagerMock.Setup(r => r.QueryForGroup(MembershipTestData.DivingGroupId)).Returns(MembershipTestData.GroupsInRoles.Where(g => g.GroupId == MembershipTestData.DivingGroupId).Select(g => g.Role).Map<AssignRoleModel>().AsQueryable());
            mock.RoleManagerMock.Setup(p => p.PermissionQuery()).Returns(MembershipTestData.Permissions.Where(p => p.ApplicationId == MembershipTestData.AppId).AsQueryable());
            mock.UserMock.Setup(m => m.GroupId).Returns(MembershipTestData.DivingGroupId);
            var result = mock.GroupService.Get(MembershipTestData.DivingGroupId);
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Roles.Count);
            Assert.AreEqual(2, result.Roles.Where(r => r.Selected).Count());
            Assert.AreEqual(4, result.Permissions.Count);
        }

        #endregion Get

        #region Save

        [TestMethod()]
        public void ChangingFromUsingPermissionsToUsingRolesShouldRemoveGroupFromCustomRolesAndDeleteCustomRoles()
        {
            var mock = new GroupServiceMock();
            mock.RoleManagerMock.Setup(r => r.QueryForGroup(MembershipTestData.DivingGroupId)).Returns(MembershipTestData.GroupsInRoles.Where(g => g.GroupId == MembershipTestData.DivingGroupId).Select(g => new AssignRoleModel { Id = g.RoleId, Name = g.Role.Name, CurrentNumberOfUsers = g.CurrentNumberOfUsers, MaxNumberOfUsers = g.MaxNumberOfUsers }).AsQueryable());
            var group = MembershipTestData.DivingGroup;
            group.UsePermissions = false;
            mock.GroupManagerMock.Setup(m => m.Query()).Returns(MembershipTestData.Groups.AsQueryable());
            mock.GroupManagerMock.Setup(g => g.Update(MembershipTestData.DivingGroupId, MembershipTestData.DivingGroup.Name, It.IsAny<bool>())).Returns(group);
            mock.GroupManagerMock.Setup(g => g.Get(MembershipTestData.DivingGroupId)).Returns(group);
            mock.RoleManagerMock.Setup(r => r.Query()).Returns(MembershipTestData.Roles.AsQueryable());
            mock.RoleManagerMock.Setup(m => m.PermissionQuery()).Returns(MembershipTestData.Permissions.AsQueryable());
            var id = Guid.NewGuid();
            Role role = new Role(id, MembershipTestData.DivingGroupId, Resources.DefaultValues.PermissionSetName);
            role.Groups = new List<GroupInRole> { new GroupInRole(MembershipTestData.DivingGroupId, id) };
            mock.RoleManagerMock.Setup(r => r.Create(Resources.DefaultValues.PermissionSetName, It.IsAny<string>(), It.IsAny<IList<Permission>>())).Returns(role);
            var model = MembershipTestData.DivingGroup.Map<GroupViewModel>();
            model.UsePermissions = true;
            model.Permissions = new List<AssignPermissionModel>();
            var result = mock.GroupService.Save(model);
            mock.RoleManagerMock.Verify(m => m.RemoveGroupFromRoles(MembershipTestData.DivingGroupId, It.IsAny<string[]>()), Times.Once());
            mock.DataSourceMock.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod()]
        public void ChangingFromUsingRolesToUsingPermissionsShouldRemoveGroupFromRoles()
        {
            var mock = new GroupServiceMock();
            mock.RoleManagerMock.Setup(r => r.QueryForGroup(MembershipTestData.DivingGroupId)).Returns(MembershipTestData.GroupsInRoles.Where(g => g.GroupId == MembershipTestData.DivingGroupId).Select(g => new AssignRoleModel { Id = g.RoleId, Name = g.Role.Name, CurrentNumberOfUsers = g.CurrentNumberOfUsers, MaxNumberOfUsers = g.MaxNumberOfUsers }).AsQueryable());
            var group = MembershipTestData.DivingGroup;
            group.UsePermissions = true;
            mock.GroupManagerMock.Setup(g => g.Update(MembershipTestData.DivingGroupId, MembershipTestData.DivingGroup.Name, It.IsAny<bool>())).Returns(group);
            mock.GroupManagerMock.Setup(g => g.Get(MembershipTestData.DivingGroupId)).Returns(group);
            mock.RoleManagerMock.Setup(r => r.Query()).Returns(MembershipTestData.Roles.AsQueryable());
            var model = MembershipTestData.DivingGroup.Map<GroupViewModel>();
            model.Roles = new List<AssignRoleModel> { new AssignRoleModel { Id = MembershipTestData.GroupAdminRoleId, Name = MembershipTestData.GroupAdminRoleName, Selected = true }, new AssignRoleModel { Id = MembershipTestData.EditorRoleId, Name = MembershipTestData.EditorRoleName, Selected = true } };
            model.UsePermissions = false;
            var result = mock.GroupService.Save(model);
            mock.RoleManagerMock.Verify(m => m.RemoveGroupFromRoles(MembershipTestData.DivingGroupId, It.IsAny<string[]>()), Times.Once());
            mock.RoleManagerMock.Verify(m => m.Delete(MembershipTestData.DivingPermissionSetRoleId), Times.Once());
            mock.DataSourceMock.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod()]
        public void SaveANewGroupWhenUsingPermissionsShouldSaveGroupCreatePermissionSetRoleForGroupAndAddAllSelectedPermissions()
        {
            var mock = new GroupServiceMock();
            mock.RoleManagerMock.Setup(r => r.QueryForGroup(MembershipTestData.DivingGroupId)).Returns(MembershipTestData.GroupsInRoles.Where(g => g.GroupId == MembershipTestData.DivingGroupId).Select(g => new AssignRoleModel { Id = g.RoleId, Name = g.Role.Name, CurrentNumberOfUsers = g.CurrentNumberOfUsers, MaxNumberOfUsers = g.MaxNumberOfUsers }).AsQueryable());
            mock.GroupManagerMock.Setup(g => g.Create(MembershipTestData.DivingGroup.Name, It.IsAny<bool>())).Returns(MembershipTestData.DivingGroup);
            mock.RoleManagerMock.Setup(r => r.Query()).Returns(MembershipTestData.Roles.AsQueryable());
            var id = Guid.NewGuid();
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(10);
            Role role = new Role(id, Guid.NewGuid(), Resources.DefaultValues.PermissionSetName);
            role.Groups = new List<GroupInRole> { new GroupInRole(MembershipTestData.DivingGroupId, id) };
            mock.RoleManagerMock.Setup(r => r.Create(Resources.DefaultValues.PermissionSetName, It.IsAny<string>(), It.IsAny<IList<Permission>>())).Returns(role);
            mock.RoleManagerMock.Setup(m => m.PermissionQuery()).Returns(MembershipTestData.Permissions.AsQueryable());
            var model = MembershipTestData.DivingGroup.Map<GroupViewModel>();
            model.PermissionSetStartDate = startDate;
            model.PermissionSetEndDate = endDate;
            model.Id = Guid.Empty;
            model.UsePermissions = true;
            model.Roles = new List<AssignRoleModel>();
            model.Permissions = new List<AssignPermissionModel> { new AssignPermissionModel { Name = MembershipTestData.CreateUserPermission.Name, Id = MembershipTestData.CreateUserPermissionId } };
            var result = mock.GroupService.Save(model);
            mock.RoleManagerMock.Verify(m => m.Create(Resources.DefaultValues.PermissionSetName, null, It.IsAny<IList<Permission>>()), Times.Once());
            mock.DataSourceMock.Verify(m => m.SaveChanges(), Times.Once());
            Assert.AreEqual(startDate, role.Groups.First().StartDate);
            Assert.AreEqual(endDate, role.Groups.First().EndDate);
            Assert.AreEqual(1, role.Permissions.Count);
        }

        [TestMethod()]
        public void SaveANewGroupWhenUsingRolesShouldSaveGroupAndAddAllSelectedMainRoles()
        {
            var mock = new GroupServiceMock();
            mock.RoleManagerMock.Setup(r => r.QueryForGroup(MembershipTestData.DivingGroupId)).Returns(MembershipTestData.GroupsInRoles.Where(g => g.GroupId == MembershipTestData.DivingGroupId).Select(g => new AssignRoleModel { Id = g.RoleId, Name = g.Role.Name, CurrentNumberOfUsers = g.CurrentNumberOfUsers, MaxNumberOfUsers = g.MaxNumberOfUsers }).AsQueryable());
            mock.GroupManagerMock.Setup(g => g.Create(MembershipTestData.DivingGroup.Name, It.IsAny<bool>())).Returns(MembershipTestData.DivingGroup);
            var groupRoleQuery = MembershipTestData.GroupsInRoles.Where(g => g.GroupId == MembershipTestData.DivingGroupId).Select(g => g).Map<AssignRoleModel>().AsQueryable();
            mock.RoleManagerMock.Setup(r => r.QueryForGroup(MembershipTestData.DivingGroupId)).Returns(new List<AssignRoleModel>().AsQueryable());
            var userRoleQuery = MembershipTestData.UsersInRoles.Where(g => g.UserId == MembershipTestData.DivingManagerId).Select(g => g.GroupRole).Map<AssignRoleModel>().AsQueryable();
            mock.RoleManagerMock.Setup(r => r.QueryForUser(MembershipTestData.DivingManagerId)).Returns(userRoleQuery);
            var model = MembershipTestData.DivingGroup.Map<GroupViewModel>();
            model.Id = Guid.Empty;
            model.Roles = new List<AssignRoleModel> { new AssignRoleModel { Id = MembershipTestData.GroupAdminRoleId, Name = MembershipTestData.GroupAdminRoleName, Selected = true }, new AssignRoleModel { Id = MembershipTestData.EditorRoleId, Name = MembershipTestData.EditorRoleName, Selected = true } };
            var result = mock.GroupService.Save(model);
            mock.RoleManagerMock.Verify(m => m.AddGroupToRole(MembershipTestData.DivingGroupId, It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int?>()), Times.Exactly(2));
            mock.DataSourceMock.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod()]
        public void SaveAnExistingGroupWhenUsingPermissionsShouldUpdateGroupUpdatePermissionSetRoleForGroupAndAllGroupRoleDatesAndAddAllSelectedPermissions()
        {
            var mock = new GroupServiceMock();
            mock.RoleManagerMock.Setup(r => r.QueryForGroup(MembershipTestData.DivingGroupId)).Returns(MembershipTestData.GroupsInRoles.Where(g => g.GroupId == MembershipTestData.DivingGroupId).Select(g => new AssignRoleModel { Id = g.RoleId, Name = g.Role.Name, CurrentNumberOfUsers = g.CurrentNumberOfUsers, MaxNumberOfUsers = g.MaxNumberOfUsers }).AsQueryable());
            var group = MembershipTestData.DivingGroup;
            group.UsePermissions = true;
            mock.GroupManagerMock.Setup(g => g.Update(MembershipTestData.DivingGroupId, MembershipTestData.DivingGroup.Name, It.IsAny<bool>())).Returns(group);
            mock.GroupManagerMock.Setup(g => g.Get(MembershipTestData.DivingGroupId)).Returns(group);
            mock.RoleManagerMock.Setup(m => m.PermissionQuery()).Returns(MembershipTestData.Permissions.AsQueryable());
            var permissionSet = MembershipTestData.DivingGroupPermissionSet;
            permissionSet.StartDate = DateTime.Now.AddDays(-10);
            permissionSet.EndDate = DateTime.Now.AddDays(20);
            mock.RoleManagerMock.Setup(m => m.GetPermissionSetForGroup(MembershipTestData.DivingGroupId)).Returns(permissionSet);
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(10);
            var model = MembershipTestData.DivingGroup.Map<GroupViewModel>();
            model.PermissionSetStartDate = startDate;
            model.PermissionSetEndDate = endDate;
            model.UsePermissions = true;
            model.Roles = new List<AssignRoleModel>();
            model.Permissions = new List<AssignPermissionModel> { new AssignPermissionModel { Name = MembershipTestData.CreateUserPermission.Name, Id = MembershipTestData.CreateUserPermissionId }, new AssignPermissionModel { Name = MembershipTestData.ErrorLogPermission.Name, Id = MembershipTestData.ErrorLogPermissionId } };
            var result = mock.GroupService.Save(model);
            Assert.AreEqual(startDate, permissionSet.StartDate);
            Assert.AreEqual(endDate, permissionSet.EndDate);
            Assert.AreEqual(2, permissionSet.Role.Permissions.Count);
        }

        [TestMethod()]
        public void SaveAnExistingGroupWhenUsingRolesShouldSaveGroupAndRemoveUnselectedMainRoles()
        {
            var mock = new GroupServiceMock();
            var group = MembershipTestData.DivingGroup;
            mock.RoleManagerMock.Setup(r => r.QueryForGroup(MembershipTestData.DivingGroupId)).Returns(MembershipTestData.GroupsInRoles.Where(g => g.GroupId == MembershipTestData.DivingGroupId).Select(g => new AssignRoleModel { Id = g.RoleId, Name = g.Role.Name, CurrentNumberOfUsers = g.CurrentNumberOfUsers, MaxNumberOfUsers = g.MaxNumberOfUsers }).AsQueryable());
            mock.GroupManagerMock.Setup(g => g.Get(MembershipTestData.DivingGroupId)).Returns(group);
            mock.GroupManagerMock.Setup(g => g.Update(MembershipTestData.DivingGroupId, It.IsAny<string>(), It.IsAny<bool>())).Returns(MembershipTestData.DivingGroup);
            mock.UserManagerMock.Setup(m => m.Query()).Returns(MembershipTestData.Users.Where(u => u.Roles.Any(r => r.GroupRoleGroupId == MembershipTestData.DivingGroupId)).ToList().AsQueryable());
            var model = MembershipTestData.DivingGroup.Map<GroupViewModel>();
            model.Roles = new List<AssignRoleModel> { new AssignRoleModel { Id = MembershipTestData.GroupAdminRoleId, Name = MembershipTestData.GroupAdminRoleName, Selected = true }, new AssignRoleModel { Id = MembershipTestData.EditorRoleId, Name = MembershipTestData.EditorRoleName, Selected = false } };
            var result = mock.GroupService.Save(model);
            mock.DataSourceMock.Verify(m => m.SaveChanges(), Times.Once());
            mock.RoleManagerMock.Verify(m => m.RemoveGroupFromRoles(MembershipTestData.DivingGroupId, It.IsAny<string[]>()), Times.Once());
        }

        #endregion Save

        #region Delete

        [TestMethod()]
        public void DeleteGroupWithUsersShouldDeleteGroupAndClearPermissionCache()
        {
            var mock = new GroupServiceMock();
            var groups = MembershipTestData.Groups;
            var group = groups.First(g => g.Id == MembershipTestData.DivingGroupId);
            mock.GroupManagerMock.Setup(g => g.Query()).Returns(groups.AsQueryable());
            mock.GroupService.Delete(MembershipTestData.DivingGroupId);
            mock.GroupManagerMock.Verify(m => m.Delete(MembershipTestData.DivingGroupId), Times.Once());
            mock.DataSourceMock.Verify(m => m.SaveChanges(), Times.Once());
        }

        #endregion Delete
    }
}