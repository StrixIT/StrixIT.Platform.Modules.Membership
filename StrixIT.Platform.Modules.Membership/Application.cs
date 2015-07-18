//-----------------------------------------------------------------------
// <copyright file="Application.cs" company="StrixIT">
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
    /// The application for users and entities. Used to allow multiple applications to use the same database with or without sharing data.
    /// </summary>
    public class Application : ValidationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Application" /> class.
        /// </summary>
        /// <param name="id">The id of the application</param>
        /// <param name="name">The name of the application</param>
        public Application(Guid id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        private Application() { }

        /// <summary>
        /// Gets the application id.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        [StrixRequired]
        [StringLength(250)]
        public string Name { get; private set; }
    }
}