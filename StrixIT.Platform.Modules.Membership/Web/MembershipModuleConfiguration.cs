﻿#region Apache License

//-----------------------------------------------------------------------
// <copyright file="MembershipModuleConfiguration.cs" company="StrixIT">
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
using StrixIT.Platform.Web;
using System.Collections.Generic;

namespace StrixIT.Platform.Modules.Membership
{
    public class MembershipModuleConfiguration : IModuleConfiguration
    {
        #region Private Fields

        private IConfiguration _config;

        #endregion Private Fields

        #region Public Constructors

        public MembershipModuleConfiguration(IConfiguration config)
        {
            _config = config;
        }

        #endregion Public Constructors

        #region Public Properties

        public IList<ModuleLink> ModuleLinks
        {
            get
            {
                var list = new List<ModuleLink>();
                list.Add(new ModuleLink(Resources.Interface.UserIndex, MembershipPermissions.ViewUsers, MembershipConstants.USER));

                if (_config.GetConfiguration<MembershipConfiguration>().UseGroups)
                {
                    list.Add(new ModuleLink(Resources.Interface.GroupIndex, MembershipPermissions.ViewGroups, MembershipConstants.GROUP));
                }

                if (_config.GetConfiguration<MembershipConfiguration>().UsePermissions)
                {
                    list.Add(new ModuleLink(Resources.Interface.RoleIndex, MembershipPermissions.ViewRoles, MembershipConstants.ROLE));
                }

                return list;
            }
        }

        public IDictionary<string, IList<string>> ModulePermissions
        {
            get
            {
                var dictionary = new Dictionary<string, IList<string>>();

                var adminPermissions = new List<string>
                {
                    MembershipPermissions.ViewUsers,
                    MembershipPermissions.AddUser,
                    MembershipPermissions.EditUser,
                    MembershipPermissions.DeleteUser,
                    MembershipPermissions.ViewGroups,
                    MembershipPermissions.AddGroup,
                    MembershipPermissions.EditGroup,
                    MembershipPermissions.DeleteGroup,
                    MembershipPermissions.ViewRoles,
                    MembershipPermissions.AddRole,
                    MembershipPermissions.EditRole,
                    MembershipPermissions.DeleteRole
                };

                var groupAdminPermissions = new List<string>
                {
                    PlatformPermissions.AccessSite,
                    MembershipPermissions.ViewUsers,
                    MembershipPermissions.AddUser,
                    MembershipPermissions.EditUser,
                    MembershipPermissions.DeleteUser,
                    MembershipPermissions.ViewRoles,
                    MembershipPermissions.AddRole,
                    MembershipPermissions.EditRole,
                    MembershipPermissions.DeleteRole,
                };

                dictionary.Add(PlatformConstants.ADMINROLE, adminPermissions);
                dictionary.Add(PlatformConstants.GROUPADMINROLE, groupAdminPermissions);
                return dictionary;
            }
        }

        public string Name
        {
            get
            {
                return MembershipConstants.MEMBERSHIP;
            }
        }

        #endregion Public Properties
    }
}