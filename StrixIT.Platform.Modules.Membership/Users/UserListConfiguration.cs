#region Apache License
//-----------------------------------------------------------------------
// <copyright file="UserListConfiguration.cs" company="StrixIT">
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
#endregion

using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    public class UserListConfiguration : ListConfiguration
    {
        public UserListConfiguration(IUserContext userContext) : base(typeof(UserListModel), new string[] { "Email" })
        {
            this.InterfaceResourceType = typeof(Resources.Interface);
            this.Fields.Add(new ListFieldConfiguration("LockedOut", "bool"));
            this.Fields.Add(new ListFieldConfiguration("Approved", "bool"));

            this.CanCreate = userContext.HasPermission(MembershipPermissions.AddUser);
            this.CanEdit = userContext.HasPermission(MembershipPermissions.EditUser);
            this.CanDelete = userContext.HasPermission(MembershipPermissions.DeleteUser);
        }
    }
}