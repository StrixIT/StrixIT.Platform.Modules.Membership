#region Apache License
//-----------------------------------------------------------------------
// <copyright file="ActiveUser.cs" company="StrixIT">
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

using System;
using System.Collections.Generic;

namespace StrixIT.Platform.Modules.Membership
{
    internal class ActiveUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public IList<ActiveUserGroup> Groups { get; set; }
        public IList<ActiveUserRole> Roles { get; set; }
        public IList<ActiveUserPermission> Permissions { get; set; }
        // public IDictionary<string, string> Preferences { get; set; }
    }

    internal class ActiveUserGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    internal class ActiveUserRole
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    internal class ActiveUserPermission
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}