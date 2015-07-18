//-----------------------------------------------------------------------
// <copyright file="GroupManager.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    public class GroupManager : IGroupManager
    {
        private IMembershipDataSource _dataSource;

        public GroupManager(IMembershipDataSource dataSource)
        {
            this._dataSource = dataSource;
        }

        public bool Exists(string name, Guid? id)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            return this.Query().Any(g => (g.Id != id && g.Name.ToLower() == name.ToLower()));
        }

        public Group Get(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }

            return this.Query().FirstOrDefault(g => g.Id == id);
        }

        public Group Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            return this.Query().FirstOrDefault(gr => gr.Name.ToLower() == name.ToLower());
        }

        public IQueryable<Group> Query()
        {
            return this._dataSource.Query<Group>();
        }

        public Group Create(string name, bool usePermissions)
        {
            if (this.Exists(name, null))
            {
                var ex = new StrixMembershipException(string.Format("A group with name {0} already exists", name));
                Logger.Log(ex.Message, ex, LogLevel.Fatal);
                throw ex;
            }

            var currentUserId = StrixPlatform.User.Id;

            if (currentUserId == null)
            {
                throw new StrixMembershipException("No active user");
            }

            var group = new Group(Guid.NewGuid(), name);
            group.UsePermissions = usePermissions;
            group = this._dataSource.Save(group);

            if (group == null)
            {
                Logger.Log(string.Format("An error occurred while creating group {0}", group.Name), LogLevel.Error);
            }
            else
            {
                var args = new Dictionary<string, object>();
                args.Add("Id", group.Id);
                args.Add("GroupName", group.Name);
                StrixPlatform.RaiseEvent<GeneralEvent>(new GeneralEvent("GroupCreateEvent", args));
            }

            return group;
        }

        public Group Update(Guid id, string name, bool usePermissions)
        {
            if (this.Exists(name, id))
            {
                var ex = new StrixMembershipException(string.Format("A group with name {0} already exists", name));
                Logger.Log(ex.Message, ex, LogLevel.Fatal);
                throw ex;
            }

            var currentUserId = StrixPlatform.User.Id;

            if (currentUserId == null)
            {
                throw new StrixMembershipException("No active user");
            }

            var group = this.Get(id);

            if (group != null)
            {
                if (group.Name.ToLower() != name.ToLower())
                {
                    group.Name = name;
                }

                group.UsePermissions = usePermissions;

                var args = new Dictionary<string, object>();
                args.Add("Id", group.Id);
                args.Add("GroupName", group.Name);
                StrixPlatform.RaiseEvent<GeneralEvent>(new GeneralEvent("GroupUpdateEvent", args));
            }

            return group;
        }

        public void Delete(Guid id)
        {
            var group = this._dataSource.Query<Group>().Include(g => g.Roles).FirstOrDefault(g => g.Id == id);

            if (group == null)
            {
                return;
            }

            this._dataSource.Delete(group.Roles);
            group.Roles.Clear();
            this._dataSource.Delete(group);
        }
    }
}