//-----------------------------------------------------------------------
// <copyright file="AssignPermissionModel.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A class to transfer permission data and assign permissions.
    /// </summary>
    public class AssignPermissionModel
    {
        /// <summary>
        /// Gets or sets the permission id.
        /// </summary> 
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the permission name.
        /// </summary> 
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the permission description.
        /// </summary> 
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the permission has been selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        ///  Gets or sets the index of the permission in the checkbox list.
        /// </summary>
        public int Index { get; set; }
    }
}