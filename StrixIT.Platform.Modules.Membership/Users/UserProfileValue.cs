//-----------------------------------------------------------------------
// <copyright file="UserProfileValue.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel.DataAnnotations;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// Defines a user profile field value.
    /// </summary>
    public class UserProfileValue : CustomFieldValue<UserProfileField>
    {
        /// <summary>
        /// Gets or sets the id of the user the profile value is for.
        /// </summary>
        [StrixRequired]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the user the profile value is for.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the culture code the profile value is for.
        /// </summary>
        [StrixRequired]
        [StringLength(5)]
        public string Culture { get; set; }
    }
}