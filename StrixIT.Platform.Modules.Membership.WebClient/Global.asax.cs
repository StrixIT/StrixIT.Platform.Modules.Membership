#region Apache License

//-----------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="StrixIT">
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
using StrixIT.Platform.Framework;
using StrixIT.Platform.Web;
using System;
using System.Web.Mvc;

namespace StrixIT.Platform.Membership.Modules.WebClient
{
    public class Global : StrixWebApplication
    {
        #region Protected Methods

        protected void Application_Start(object sender, EventArgs e)
        {
            DependencyInjector.Injector = new StructureMapDependencyInjector();
            DependencyResolver.SetResolver(new StructureMapDependencyResolver());

            SetupFileWatcher();
            Bootstrapper.Run();

            Logger.Log("Web application start. Initialize Mvc.");
            var mvcService = DependencyInjector.Get<IMvcService>();
            mvcService.Initialize();

            Logger.Log("Run all web initializers");

            foreach (var initializer in DependencyInjector.GetAll<IWebInitializer>())
            {
                Logger.Log(string.Format("Start web initializer {0}.", initializer.GetType().Name));
                initializer.WebInitialize();
            }

            Logger.Log("Web application startup finished.");
        }

        #endregion Protected Methods
    }
}