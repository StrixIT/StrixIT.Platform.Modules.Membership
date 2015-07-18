//-----------------------------------------------------------------------
// <copyright file="IRoleService.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// The interface for the role service.
    /// </summary>
    public interface IRoleService : ICrudService<Guid, RoleViewModel>
    {
    }
}
