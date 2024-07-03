﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using System;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using Microsoft.EntityFrameworkCore;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<ContributionType> ContributionTypes { get; set; }

        public async ValueTask<ContributionType> UpdateContributionTypeAsync(ContributionType contributionType) =>
            await UpdateAsync(contributionType);
    }
}
