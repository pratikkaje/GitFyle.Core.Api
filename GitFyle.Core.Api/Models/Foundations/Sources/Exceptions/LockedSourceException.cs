﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Sources.Exceptions
{
    public class LockedSourceException : Xeption
    {
        public LockedSourceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}