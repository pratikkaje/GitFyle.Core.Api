﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions
{
    public class FailedOperationContributionTypeException : Xeption
    {
        public FailedOperationContributionTypeException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}