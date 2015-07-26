#region Apache License

//-----------------------------------------------------------------------
// <copyright file="MembershipAreaRegistration.cs" company="StrixIT">
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
using System;
using System.Web.Mvc;

namespace StrixIT.Platform.Modules.Membership
{
    public class MembershipAreaRegistration : AreaRegistration
    {
        #region Public Properties

        public override string AreaName
        {
            get
            {
                return "Membership";
            }
        }

        #endregion Public Properties

        #region Public Methods

        public override void RegisterArea(AreaRegistrationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var culture = StrixPlatform.DefaultCultureCode.ToLower();

            context.MapLocalizedRoute(
                "Account_ResetLink",
                "{language}/Account/SendPasswordLink/{userId}",
                new { language = culture, controller = MembershipConstants.ACCOUNT, action = "SendPasswordLink", key = UrlParameter.Optional });

            context.MapLocalizedRoute(
                "Account_Default",
                "{language}/Account/{action}/{key}",
                new { language = culture, controller = MembershipConstants.ACCOUNT, action = MvcConstants.INDEX, key = UrlParameter.Optional });

            context.MapLocalizedRoute(
                "Membership_admin",
                "{language}/Admin/Membership/{controller}/{action}/{id}",
                new { language = culture, controller = "Base", action = MvcConstants.INDEX, id = UrlParameter.Optional });
        }

        #endregion Public Methods
    }
}