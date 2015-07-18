//-----------------------------------------------------------------------
// <copyright file="ActiveUser.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
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