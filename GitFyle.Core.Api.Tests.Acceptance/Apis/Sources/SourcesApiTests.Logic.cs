﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Tests.Acceptance.Brokers;

namespace GitFyle.Core.Api.Tests.Acceptance.Apis.Sources
{
    [Collection(nameof(ApiTestCollection))]
    public partial class SourcesApiTests
    {
        [Fact]
        public async Task ShouldPostSourceAsync()
        {
            // given
            DateTimeOffset currentDate = DateTimeOffset.UtcNow;
            Source randomSource = CreateRandomSource(currentDate);
            Source inputSource = randomSource;
            Source expectedSource = inputSource;

            // when
            Source actualSource =
                await this.gitFyleCoreApiBroker.PostSourceAsync(inputSource);

            // then
            actualSource.Should().BeEquivalentTo(expectedSource);
            await this.gitFyleCoreApiBroker.DeleteSourceByIdAsync(actualSource.Id);
        }
    }
}
