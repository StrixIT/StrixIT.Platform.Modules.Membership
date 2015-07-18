//-----------------------------------------------------------------------
// <copyright file="MembershipModuleConfiguration.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Collections.Generic;
using StrixIT.Platform.Core;
using StrixIT.Platform.Web;

namespace StrixIT.Platform.Modules.Membership
{
    public class MembershipModuleConfiguration : IModuleConfiguration
    {
        public string Name
        {
            get
            {
                return MembershipConstants.MEMBERSHIP;
            }
        }

        public IList<ModuleLink> ModuleLinks
        {
            get
            {
                var list = new List<ModuleLink>();
                list.Add(new ModuleLink(Resources.Interface.UserIndex, MembershipPermissions.ViewUsers, MembershipConstants.USER));

                if (StrixMembership.Configuration.UseGroups)
                {
                    list.Add(new ModuleLink(Resources.Interface.GroupIndex, MembershipPermissions.ViewGroups, MembershipConstants.GROUP));
                }

                if (StrixMembership.Configuration.UsePermissions)
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
    }
}