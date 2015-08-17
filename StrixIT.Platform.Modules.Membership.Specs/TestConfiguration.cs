using StrixIT.Platform.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StrixIT.Platform.Modules.Membership.Specs
{
    public class TestConfiguration : IConfiguration
    {
        #region Public Properties

        public bool CustomErrorsEnabled
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string FromAddress
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string MailPickupDirectory
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion Public Properties

        #region Public Methods

        public dynamic GetConfigSectionGroup(string group)
        {
            throw new NotImplementedException();
        }

        public T GetConfiguration<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        public T GetConfiguration<T>(string key) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public string GetConnectionString(string connectionStringName)
        {
            return @"Data Source=.\sqlexpress;Initial Catalog=StrixMembershipTests;Integrated Security=True;multipleActiveResultSets=true;App=StrixMembershipSpecifications";
        }

        public T GetSetting<T>(string key)
        {
            throw new NotImplementedException();
        }

        public T GetSetting<T>(string module, string key)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}