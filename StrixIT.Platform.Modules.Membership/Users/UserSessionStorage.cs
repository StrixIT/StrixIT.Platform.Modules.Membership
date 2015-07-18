//-----------------------------------------------------------------------
// <copyright file="UserSessionStorage.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A class to store the user's session data in the database.
    /// </summary>
    internal class UserSessionStorage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserSessionStorage" /> class.
        /// </summary>
        /// <param name="userId">The id of the user the session data is for</param>
        public UserSessionStorage(Guid userId)
        {
            this.Id = userId;
        }

        private UserSessionStorage() { }

        /// <summary>
        /// Gets the id of the user the session data is for.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets or sets the user the session data is for.
        /// </summary>
        public User User { get; internal set; }

        /// <summary>
        /// Gets the user security the session data is for.
        /// </summary>
        public UserSecurity UserSecurity { get; private set; }

        /// <summary>
        /// Gets or sets the session data in JSON format.
        /// </summary>
        public string Session { get; set; }
    }
}