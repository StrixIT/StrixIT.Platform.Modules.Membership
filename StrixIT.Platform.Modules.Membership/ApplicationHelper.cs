#region Apache License

//-----------------------------------------------------------------------
// <copyright file="ApplicationHelper.cs" company="StrixIT">
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
using System.Linq;

namespace StrixIT.Platform.Modules.Membership
{
    public static class ApplicationHelper
    {
        #region Private Fields

        private static Guid _appId;
        private static Guid _mainGroupId;

        #endregion Private Fields

        #region Public Methods

        public static Guid GetApplicationId(IMembershipDataSource dataSource)
        {
            if (_appId == Guid.Empty)
            {
                var appName = StrixPlatform.Configuration.ApplicationName;
                _appId = dataSource.Query<Application>().Where(a => a.Name.ToLower() == appName.ToLower()).Select(a => a.Id).FirstOrDefault();
            }

            return _appId;
        }

        public static Guid GetMainGroupId(IMembershipDataSource dataSource)
        {
            if (_mainGroupId == Guid.Empty)
            {
                var mainGroupName = Resources.DefaultValues.MainGroupName;
                _mainGroupId = dataSource.Query<Group>().Where(g => g.Name.ToLower() == mainGroupName.ToLower()).Select(g => g.Id).FirstOrDefault();
            }
            return _mainGroupId;
        }

        #endregion Public Methods
    }
}