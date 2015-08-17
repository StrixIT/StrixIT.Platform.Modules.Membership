#region Apache License

//-----------------------------------------------------------------------
// <copyright file="GroupManager.cs" company="StrixIT">
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
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace StrixIT.Platform.Modules.Membership
{
    public class GroupManager : IGroupManager
    {
        #region Private Fields

        private IMembershipDataSource _dataSource;

        private IUserContext _user;

        #endregion Private Fields

        #region Public Constructors

        public GroupManager(IMembershipDataSource dataSource, IUserContext user)
        {
            _dataSource = dataSource;
            _user = user;
        }

        #endregion Public Constructors

        #region Public Methods

        public Group Create(string name, bool usePermissions)
        {
            if (Exists(name, null))
            {
                var ex = new StrixMembershipException(string.Format("A group with name {0} already exists", name));
                Logger.Log(ex.Message, ex, LogLevel.Fatal);
                throw ex;
            }

            var currentUserId = _user.Id;

            if (currentUserId == null)
            {
                throw new StrixMembershipException("No active user");
            }

            var group = new Group(Guid.NewGuid(), name);
            group.UsePermissions = usePermissions;
            group = _dataSource.Save(group);

            if (group == null)
            {
                Logger.Log(string.Format("An error occurred while creating group {0}", group.Name), LogLevel.Error);
            }
            else
            {
                var args = new Dictionary<string, object>();
                args.Add("Id", group.Id);
                args.Add("GroupName", group.Name);
                PlatformEvents.Raise<GeneralEvent>(new GeneralEvent("GroupCreateEvent", args));
            }

            return group;
        }

        public void Delete(Guid id)
        {
            var group = _dataSource.Query<Group>().Include(g => g.Roles).FirstOrDefault(g => g.Id == id);

            if (group == null)
            {
                return;
            }

            _dataSource.Delete(group.Roles);
            group.Roles.Clear();
            _dataSource.Delete(group);
        }

        public bool Exists(string name, Guid? id)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            return Query().Any(g => (g.Id != id && g.Name.ToLower() == name.ToLower()));
        }

        public Group Get(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }

            return Query().FirstOrDefault(g => g.Id == id);
        }

        public Group Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            return Query().FirstOrDefault(gr => gr.Name.ToLower() == name.ToLower());
        }

        public IQueryable<Group> Query()
        {
            return _dataSource.Query<Group>();
        }

        public Group Update(Guid id, string name, bool usePermissions)
        {
            if (Exists(name, id))
            {
                var ex = new StrixMembershipException(string.Format("A group with name {0} already exists", name));
                Logger.Log(ex.Message, ex, LogLevel.Fatal);
                throw ex;
            }

            var currentUserId = _user.Id;

            if (currentUserId == null)
            {
                throw new StrixMembershipException("No active user");
            }

            var group = Get(id);

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
                PlatformEvents.Raise<GeneralEvent>(new GeneralEvent("GroupUpdateEvent", args));
            }

            return group;
        }

        #endregion Public Methods
    }
}