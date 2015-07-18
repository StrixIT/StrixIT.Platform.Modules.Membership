//-----------------------------------------------------------------------
// <copyright file="MembershipRegistry.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Security.Principal;
using StructureMap.Configuration.DSL;
using StrixIT.Platform.Core;
using StructureMap.Web.Pipeline;

namespace StrixIT.Platform.Modules.Membership
{
    public class MembershipRegistry : Registry
    {
        public MembershipRegistry()
        {
            For<IMembershipDataSource>().Use<MembershipDataSource>();
            For<IUserContext>().LifecycleIs(new HybridLifecycle());
        }
    }
}
