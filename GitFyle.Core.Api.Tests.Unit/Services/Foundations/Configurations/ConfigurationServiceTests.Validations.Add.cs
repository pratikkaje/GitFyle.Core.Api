﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Configurations
{
    public partial class ConfigurationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfConfigurationIsNullAndLogItAsync()
        {
            // given
            Configuration nullConfiguration = null;

            var nullConfigurationException =
                new NullConfigurationException(
                    message: "Configuration is null");

            var expectedConfigurationValidationException =
                new ConfigurationValidationException(
                    message: "Configuration validation error occurred, fix errors and try again",
                    innerException: nullConfigurationException);

            // when
            ValueTask<Configuration> addConfigurationTask =
                this.configurationService.AddConfigurationAsync(nullConfiguration);

            ConfigurationValidationException actualConfigurationValidationException =
                await Assert.ThrowsAsync<ConfigurationValidationException>(
                    addConfigurationTask.AsTask);

            // then
            actualConfigurationValidationException.Should().BeEquivalentTo(
                expectedConfigurationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConfigurationValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConfigurationAsync(It.IsAny<Configuration>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
