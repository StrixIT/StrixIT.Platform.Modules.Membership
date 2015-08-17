using StrixIT.Platform.Core;
using StrixIT.Platform.Core.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixIT.Platform.Modules.Membership
{
    public class MembershipSettings : IMembershipSettings
    {
        #region Private Fields

        private static Guid? _adminId;
        private static Guid? _appId;
        private static Guid? _mainGroupId;
        private IConfiguration _config;
        private IMembershipDataSource _dataSource;

        #endregion Private Fields

        #region Public Constructors

        public MembershipSettings(IMembershipDataSource dataSource, IConfiguration config)
        {
            _dataSource = dataSource;
            _config = config;
        }

        #endregion Public Constructors

        #region Public Properties

        public Guid AdminId
        {
            get
            {
                if (_adminId == null)
                {
                    _adminId = _dataSource.Query<User>().Where(g => g.Email.ToLower() == Resources.DefaultValues.AdministratorEmail).Select(u => u.Id).First();
                }

                return _adminId.Value;
            }
        }

        public Guid ApplicationId
        {
            get
            {
                if (_appId == null)
                {
                    _appId = this._dataSource.Query<Application>().Where(g => g.Name.ToLower() == _config.GetConfiguration<PlatformConfiguration>().ApplicationName.ToLower()).Select(a => a.Id).First();
                }

                return _appId.Value;
            }
        }

        public Guid MainGroupId
        {
            get
            {
                if (_mainGroupId == null)
                {
                    _mainGroupId = this._dataSource.Query<Group>().Where(g => g.Name.ToLower() == Resources.DefaultValues.MainGroupName).Select(g => g.Id).First();
                }

                return _mainGroupId.Value;
            }
        }

        #endregion Public Properties
    }
}