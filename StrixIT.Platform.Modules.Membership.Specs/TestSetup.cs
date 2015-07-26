#region Apache License
//-----------------------------------------------------------------------
// <copyright file="TestSetup.cs" company="StrixIT">
// Copyright 2015 StrixIT, author R.G. Schurgers MA MSc.
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixIT.Platform.Testing;

namespace StrixIT.Platform.Modules.Membership.Specs
{
    [TestClass]
    public class TestSetup
    {
        [AssemblyInitialize]
        public static void Setup(TestContext context)
        {
            TestManager.UseDataContext(new MembershipDataSource());
            TestManager.AuthenticationEmail = Resources.DefaultValues.AdministratorEmail;
            TestManager.AuthenticationPassword = Resources.DefaultValues.AdministratorPassword;
            TestManager.SetupTestRun("StrixIT.Platform.Modules.Membership.WebClient", TestBrowser.Chrome);
        }

        [AssemblyCleanup]
        public static void TearDown()
        {
            TestManager.TearDownTestRun();
        }
    }
}