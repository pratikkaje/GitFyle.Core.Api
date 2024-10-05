﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Repositories
{
    public partial class RepositoryServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Repository someRepository = CreateRandomRepository();
            SqlException sqlException = CreateSqlException();

            var failedRepositoryStorageException =
                new FailedStorageRepositoryException(
                    message: "Failed storage repository error occurred, contact support.",
                        innerException: sqlException);

            var expectedRepositoryDependencyException =
                new RepositoryDependencyException(
                    message: "Repository dependency error occurred, contact support.",
                        innerException: failedRepositoryStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Repository> modifyRepositoryTask =
                this.repositoryService.ModifyRepositoryAsync(someRepository);

            RepositoryDependencyException actualRepositoryDependencyException =
                await Assert.ThrowsAsync<RepositoryDependencyException>(
                    testCode: modifyRepositoryTask.AsTask);

            // then
            actualRepositoryDependencyException.Should().BeEquivalentTo(
                expectedRepositoryDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectRepositoryByIdAsync(someRepository.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedRepositoryDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateRepositoryAsync(someRepository),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            Repository randomRepository =
                CreateRandomRepository(randomDateTimeOffset);

            randomRepository.CreatedDate =
                randomDateTimeOffset.AddMinutes(minutesInPast);

            var dbUpdateException = new DbUpdateException();

            var failedOperationRepositoryException =
                new FailedOperationRepositoryException(
                    message: "Failed operation repository error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedRepositoryDependencyException =
                new RepositoryDependencyException(
                    message: "Repository dependency error occurred, contact support.",
                    innerException: failedOperationRepositoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectRepositoryByIdAsync(randomRepository.Id))
                    .ThrowsAsync(dbUpdateException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Repository> modifyRepositoryTask =
                this.repositoryService.ModifyRepositoryAsync(randomRepository);

            RepositoryDependencyException actualRepositoryDependencyException =
                await Assert.ThrowsAsync<RepositoryDependencyException>(
                    testCode: modifyRepositoryTask.AsTask);

            // then
            actualRepositoryDependencyException.Should().BeEquivalentTo(
                expectedRepositoryDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectRepositoryByIdAsync(randomRepository.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRepositoryDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}