﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions
{
    public class LockedContributionException : Xeption
    {
        public LockedContributionException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}