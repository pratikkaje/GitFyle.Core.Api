﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions
{
    public class FailedStorageConfigurationException : Xeption
    {
        public FailedStorageConfigurationException(string message, Exception 
            innerException) : base(message, innerException)
        { }
    }
}
