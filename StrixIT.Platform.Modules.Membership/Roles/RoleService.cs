﻿#region Apache License

//-----------------------------------------------------------------------
// <copyright file="RoleService.cs" company="StrixIT">
// Copyright 2015 StrixIT. Author R.G. Schurgers MA MSc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

#endregion Apache License

using StrixIT.Platform.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StrixIT.Platform.Modules.Membership
{
    public class RoleService : IRoleService
    {
        #region Private Fields

        private IMembershipDataSource _dataSource;
        private IRoleManager _roleManager;

        #endregion Private Fields

        #region Public Constructors

        public RoleService(IMembershipDataSource dataSource, IRoleManager roleManager)
        {
            this._dataSource = dataSource;
            this._roleManager = roleManager;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Delete(Guid id)
        {
            this.Delete(id, true);
        }

        public void Delete(Guid id, bool saveChanges)
        {
            this._roleManager.Delete(id);

            if (saveChanges)
            {
                this._dataSource.SaveChanges();
            }
        }

        public bool Exists(string name, Guid? id)
        {
            return this._roleManager.Exists(name, id);
        }

        #endregion Public Methods

        #region Get

        public RoleViewModel Get(Guid? id)
        {
            RoleViewModel model;

            if (!id.HasValue || id.Value == Guid.Empty)
            {
                model = new RoleViewModel();
            }
            else
            {
                var role = this._roleManager.Get(id.Value);

                if (role.Name.ToLower() == Resources.DefaultValues.PermissionSetName.ToLower())
                {
                    throw new StrixMembershipException("Cannot edit a permission set.");
                }

                model = role.Map<RoleViewModel>();
            }

            this.AddPermissions(model);
            return model;
        }

        public IEnumerable List(FilterOptions filter)
        {
            return this._roleManager.Query().Where(r => r.Name.ToLower() != Resources.DefaultValues.PermissionSetName.ToLower()).Filter(filter).Select(r => new RoleViewModel { Id = r.Id, Name = r.Name }).ToList();
        }

        #endregion Get

        public SaveResult<RoleViewModel> Save(RoleViewModel model)
        {
            return this.Save(model, true);
        }

        public SaveResult<RoleViewModel> Save(RoleViewModel model, bool saveChanges)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            if (this.Exists(model.Name, model.Id))
            {
                return new SaveResult<RoleViewModel> { Success = false, Message = Resources.Interface.DuplicateName };
            }

            Role role;
            List<Permission> permissions = new List<Permission>();

            if (model.Permissions != null)
            {
                var permissionIds = model.Permissions.Where(p => p.Selected).Select(p => p.Id).ToArray();
                permissions = this._roleManager.PermissionQuery().Where(p => permissionIds.Contains(p.Id)).ToList();
            }

            if (model.Id == Guid.Empty)
            {
                role = this._roleManager.Create(model.Name, model.Description, permissions);

                if (role == null)
                {
                    return new SaveResult<RoleViewModel>();
                }

                // Add the current group to the role using the permission set start and end dates.
                var groupId = StrixPlatform.User.GroupId;
                var groupPermissionSet = this._roleManager.Query().Where(r => r.GroupId == groupId && r.Name.ToLower() == Resources.DefaultValues.PermissionSetName.ToLower()).SelectMany(r => r.Groups.Where(g => g.GroupId == groupId)).FirstOrDefault();
                role.Groups = new List<GroupInRole> { new GroupInRole(groupId, role.Id, groupPermissionSet.StartDate, groupPermissionSet.EndDate) };
            }
            else
            {
                role = this._roleManager.Update(model.Id, model.Name, model.Description, permissions);
            }

            if (saveChanges)
            {
                this._dataSource.SaveChanges();
            }

            return new SaveResult<RoleViewModel>(true, role.Map<RoleViewModel>());
        }

        #region Private Methods

        private void AddPermissions(RoleViewModel model)
        {
            model.Permissions = this._roleManager.PermissionQuery().Map<AssignPermissionModel>().ToList();

            if (model.Id != Guid.Empty)
            {
                var rolePermissions = this._roleManager.Query().Where(r => r.Id == model.Id).SelectMany(r => r.Permissions).ToList();

                foreach (var permission in model.Permissions)
                {
                    var rolePermission = rolePermissions.FirstOrDefault(p => p.Id == permission.Id);

                    if (rolePermission != null)
                    {
                        permission.Selected = true;
                    }
                }
            }

            var index = 0;

            foreach (var permission in model.Permissions)
            {
                permission.Index = index;
                index++;
            }
        }

        #endregion Private Methods
    }
}