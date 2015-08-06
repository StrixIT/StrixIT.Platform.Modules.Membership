#region Apache License

//-----------------------------------------------------------------------
// <copyright file="UserListConfiguration.cs" company="StrixIT">
// Copyright 2015 StrixIT. Author R.G. Schurgers MA MSc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use file except in compliance with the License.
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

namespace StrixIT.Platform.Modules.Membership
{
    public class UserListConfiguration : ListConfiguration
    {
        #region Public Constructors

        public UserListConfiguration(IUserContext user) : base(typeof(UserListModel), new string[] { "Email" })
        {
            InterfaceResourceType = typeof(Resources.Interface);
            Fields.Add(new ListFieldConfiguration("LockedOut", "bool"));
            Fields.Add(new ListFieldConfiguration("Approved", "bool"));

            CanCreate = user.HasPermission(MembershipPermissions.AddUser);
            CanEdit = user.HasPermission(MembershipPermissions.EditUser);
            CanDelete = user.HasPermission(MembershipPermissions.DeleteUser);
        }

        #endregion Public Constructors
    }
}