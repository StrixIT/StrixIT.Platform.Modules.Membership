using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrixIT.Platform.Core;
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
            var environmentMock = new Mock<IEnvironment>();
            StrixPlatform.Environment = environmentMock.Object;
            StrixPlatform.ApplicationId = MembershipTestData.AppId;
            StrixPlatform.MainGroupId = MembershipTestData.MainGroupId;

            environmentMock.Setup(e => e.GetFromSession<string>(PlatformConstants.CURRENTUSEREMAIL)).Returns(MembershipTestData.Administrator.Email);

            var dataMock = new Mock<IMembershipDataSource>();

            var mock = new UserManagerMock();

            dataMock.Setup(d => d.Query<UserSessionStorage>()).Returns(MembershipTestData.Sessions.AsQueryable());
            var context = new UserContext(dataMock.Object);

            var result = context.GroupId;
            environmentMock.Verify(e => e.StoreInSession("CurrentGroupId", It.IsAny<object>()), Times.Exactly(2));
        }

        #endregion Public Methods
    }
}