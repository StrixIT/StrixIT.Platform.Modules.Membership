//-----------------------------------------------------------------------
// <copyright file="User.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// The user class.
    /// </summary>
    public class User : ValidationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="User" /> class.
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <param name="email">The user's email</param>
        /// <param name="name">The user's name</param>
        public User(Guid id, string email, string name)
        {
            this.Id = id;
            this.Email = email;
            this.Name = name;
        }

        private User() { }

        /// <summary>
        /// Gets the user Id.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the user's name.
        /// </summary>
        [StringLength(250)]
        public string Name { get; internal set; }

        /// <summary>
        /// Gets user's email address.
        /// </summary>
        [StrixRequired]
        [StringLength(250)]
        public string Email { get; internal set; }

        /// <summary>
        /// Gets or sets the user's preferred culture.
        /// </summary>
        [StrixRequired]
        [StringLength(5)]
        public string PreferredCulture { get; set; }

        /// <summary>
        /// Gets or sets the date at which the user has accepted the terms of usage.
        /// </summary>
        [StrixNotDefault]
        public DateTime? DateAcceptedTerms { get; set; }

        /// <summary>
        /// Gets or sets the roles this user is in.
        /// </summary>
        public ICollection<UserInRole> Roles { get; set; }

        /// <summary>
        /// Gets or sets the profile values of this user.
        /// </summary>
        public ICollection<UserProfileValue> ProfileValues { get; set; }
    }
}