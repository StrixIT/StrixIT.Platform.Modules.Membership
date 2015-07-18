//-----------------------------------------------------------------------
// <copyright file="IGroupService.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using StrixIT.Platform.Core;
using StrixIT.Platform.Web;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// The group service interface.
    /// </summary>
    public interface IGroupService : ICrudService<Guid, GroupViewModel>
    {
    }
}