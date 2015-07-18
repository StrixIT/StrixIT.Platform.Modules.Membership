//-----------------------------------------------------------------------
// <copyright file="StrixMembershipException.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace StrixIT.Platform.Modules.Membership
{
    [Serializable]
    public class StrixMembershipException : Exception
    {
        public StrixMembershipException() : base() { }

        public StrixMembershipException(string message) : base(message) { }

        public StrixMembershipException(string message, Exception inner) : base(message, inner) { }

        protected StrixMembershipException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}