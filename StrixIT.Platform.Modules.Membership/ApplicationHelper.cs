//-----------------------------------------------------------------------
// <copyright file="ApplicationHelper.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    public static class ApplicationHelper
    {
        private static Guid _appId;
        private static Guid _mainGroupId;

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
    }
}