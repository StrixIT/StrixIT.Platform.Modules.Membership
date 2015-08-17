using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrixIT.Platform.Core;
using StrixIT.Platform.Core.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixIT.Platform.Modules.Membership.Tests
{
    [TestClass]
    public class UserContextTests
    {
        #region Public Methods

        [TestMethod()]
        public void GetSessionShouldRestoreSavedSessionValues()
        {
            var sessionMock = new Mock<ISessionService>();
            var dataMock = new Mock<IMembershipDataSource>();
            dataMock.Setup(d => d.Query<UserSessionStorage>()).Returns(MembershipTestData.Sessions.AsQueryable());
            var context = new UserContext(dataMock.Object, sessionMock.Object, MembershipTestData.Administrator.Email);
            var result = context.GroupId;
            sessionMock.Verify(e => e.Store("CurrentGroupId", It.IsAny<object>()), Times.Exactly(2));
        }

        #endregion Public Methods
    }
}