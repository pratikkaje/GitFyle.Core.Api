﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public partial class SourceServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionIfSqlErrorOccurredAndLogItAsync()
        {
            // given
            Source someSource = CreateRandomSource();
            SqlException sqlException = CreateSqlException();

            var failedStoragSourceeException =
                new FailedStorageSourceException(
                    message: "Failed source storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedSourceDependencyException =
                new SourceDependencyException(
                    message: "Source dependency error occurred, contact support.",
                    innerException: failedStoragSourceeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Source> addSourceTask =
                this.sourceService.AddSourceAsync(
                    someSource);

            SourceDependencyException actualSourceDependencyException =
                await Assert.ThrowsAsync<SourceDependencyException>(
                    testCode: addSourceTask.AsTask);

            // then
            actualSourceDependencyException.Should().BeEquivalentTo(
                expectedSourceDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedSourceDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSourceAsync(It.IsAny<Source>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfSourceAlreadyExistsAndLogItAsync()
        {
            // given
            Source someSource = CreateRandomSource();

            var duplicateKeyException =
                new DuplicateKeyException(
                    message: "Duplicate key error occurred");

            var alreadyExistsSourceException =
                new AlreadyExistsSourceException(
                    message: "Source already exists error occurred.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedSourceDependencyValidationException =
                new SourceDependencyValidationException(
                    message: "Source dependency validation error occurred, fix errors and try again.",
                    innerException: alreadyExistsSourceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<Source> addSourceTask =
                this.sourceService.AddSourceAsync(
                    someSource);

            SourceDependencyValidationException actualSourceDependencyValidationException =
                await Assert.ThrowsAsync<SourceDependencyValidationException>(
                    testCode: addSourceTask.AsTask);

            // then
            actualSourceDependencyValidationException.Should().BeEquivalentTo(
                expectedSourceDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSourceDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSourceAsync(It.IsAny<Source>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
