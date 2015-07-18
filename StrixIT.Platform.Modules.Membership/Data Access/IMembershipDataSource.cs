//-----------------------------------------------------------------------
// <copyright file="IMembershipDataSource.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// The membership data source interface.
    /// </summary>
    public interface IMembershipDataSource : IDataSource
    {
        /// <summary>
        /// Gets a query for the specified object type with the specified includes.
        /// </summary>
        /// <typeparam name="T">The type of the object to get the query for</typeparam>
        /// <param name="includes">The includes</param>
        /// <returns>The query</returns>
        IQueryable<T> Query<T>(string includes) where T : class;

        /// <summary>
        /// Finds an object in memory or the data source using its key values.
        /// </summary>
        /// <typeparam name="T">The type of the object to find</typeparam>
        /// <param name="keys">The object's key values</param>
        /// <returns>The object</returns>
        T Find<T>(object[] keys) where T : class;
    }
}
