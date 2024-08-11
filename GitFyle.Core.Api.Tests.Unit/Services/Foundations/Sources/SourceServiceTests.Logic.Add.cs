﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Sources;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public partial class SourceServiceTests
    {
        [Fact]
        public async Task ShouldAddSourceAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTime;
            Source randomSource = CreateRandomSource(dateTimeOffset: now);
            Source inputSource = randomSource;
            Source insertedSource = inputSource.DeepClone();
            Source expectedSource = insertedSource.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertSourceAsync(inputSource))
                    .ReturnsAsync(insertedSource);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            Source actualSource =
                await this.sourceService.AddSourceAsync(inputSource);

            // then
            actualSource.Should().BeEquivalentTo(expectedSource);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSourceAsync(inputSource),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
