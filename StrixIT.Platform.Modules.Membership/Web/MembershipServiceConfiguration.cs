#region Apache License

//-----------------------------------------------------------------------
// <copyright file="MembershipServiceConfiguration.cs" company="StrixIT">
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
using StrixIT.Platform.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Web;

namespace StrixIT.Platform.Modules.Membership
{
    public class MembershipServiceConfiguration : IServiceConfiguration
    {
        #region Public Properties

        public IList<ServiceDescriptor> Services
        {
            get
            {
                var serviceList = new List<ServiceDescriptor>();
                serviceList.Add(new ServiceDescriptor(typeof(IMembershipDataSource), typeof(MembershipDataSource)));

                Func<string> userEmailFunc = () =>
                {
                    return HttpContext.Current != null && HttpContext.Current.User != null ? HttpContext.Current.User.Identity.Name : null;
                };

                var emailValue = new ConstructorValue<string>("userEmail", Helpers.FuncToExpression(userEmailFunc));

                serviceList.Add(new ServiceDescriptorWithConstructorValue<string>(typeof(IUserContext), typeof(UserContext), emailValue));

                Func<Uri> urlFunc = () =>
                {
                    try
                    {
                        return HttpContext.Current != null && HttpContext.Current.Request != null ? HttpContext.Current.Request.Url : null;
                    }
                    catch
                    {
                        return null;
                    }
                };

                var urlValue = new ConstructorValue<Uri>("url", Helpers.FuncToExpression(urlFunc));
                serviceList.Add(new ServiceDescriptorWithConstructorValue<Uri>(typeof(IMembershipMailer), typeof(MembershipMailer), urlValue));

                return serviceList;
            }
        }

        #endregion Public Properties
    }
}