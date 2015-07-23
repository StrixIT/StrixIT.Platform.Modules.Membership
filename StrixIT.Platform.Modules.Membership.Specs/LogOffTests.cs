#region Apache License
//-----------------------------------------------------------------------
// <copyright file="LogOffTests.cs" company="StrixIT">
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

using System.Web.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoryQ;
using StrixIT.Platform.Testing;

namespace StrixIT.Platform.Modules.Membership.Specs
{
    [TestClass]
    public class LogOffTests
    {
        [TestMethod]
        public void SecureUserLogOff()
        {
            new Story("Secure user logoff")
                .InOrderTo("have a secure account")
                .AsA("user")
                .IWant("to be sure my account can no longer be accessed after I log out")

                        .WithScenario("User logs off")
                            .Given(TheUserIsLoggedIn)
                            .When(LoggingTheUserOff)
                            .Then(TheUsersAuthenticationCookieIsInvalidatedAndHisSessionAbandoned)
                                .And(TheUserIsRedirectedToTheHomePage)
             .Execute();
        }

        private void TheUserIsLoggedIn()
        {
            TestManager.Browser.LogOn();
        }

        private void LoggingTheUserOff()
        {
            TestManager.Browser.LogOff();
        }

        private void TheUsersAuthenticationCookieIsInvalidatedAndHisSessionAbandoned()
        {
            var cookie = TestManager.Browser.Manage().Cookies.GetCookieNamed(FormsAuthentication.FormsCookieName);
            Assert.IsNull(cookie);
        }

        private void TheUserIsRedirectedToTheHomePage()
        {
            Assert.IsTrue(TestManager.Browser.IsAt("/"));
        }
    }
}