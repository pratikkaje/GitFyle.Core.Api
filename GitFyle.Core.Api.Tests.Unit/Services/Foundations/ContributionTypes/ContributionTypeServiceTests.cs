﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using GitFyle.Core.Api.Brokers.DateTimes;
using GitFyle.Core.Api.Brokers.Loggings;
using GitFyle.Core.Api.Brokers.Storages;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using GitFyle.Core.Api.Services.Foundations.ContributionTypes;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.ContributionTypes
{
    public partial class ContributionTypeServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ContributionTypeService contributionTypeService;

        public ContributionTypeServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.contributionTypeService = new ContributionTypeService(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(
            Xeption expectedException)
        {
            return actualException =>
                actualException.SameExceptionAs(expectedException);
        }

        private static SqlException CreateSqlException()
        {
            return (SqlException)RuntimeHelpers.GetUninitializedObject(
                type: typeof(SqlException));
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            return new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length)
                .GetValue();
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static ContributionType CreateRandomContributionType() =>
            CreateRandomContributionType(dateTimeOffset: GetRandomDateTimeOffset());

        private static ContributionType CreateRandomContributionType(DateTimeOffset dateTimeOffset) =>
            CreateContributionTypeFiller(dateTimeOffset).Create();

        private static IQueryable<ContributionType> CreateRandomContributionTypes()
        {
            return CreateContributionTypeFiller(GetRandomDateTimeOffset())
                .Create(GetRandomNumber())
                .AsQueryable();
        }

        private static ContributionType CreateRandomModifyContributionType(DateTimeOffset dateTimeOffset)
        {
            int randomDaysInThePast = GetRandomNegativeNumber();
            ContributionType randomContributionType = CreateRandomContributionType(dateTimeOffset);
            randomContributionType.CreatedDate = dateTimeOffset.AddDays(randomDaysInThePast);

            return randomContributionType;
        }

        private static Filler<ContributionType> CreateContributionTypeFiller(DateTimeOffset dateTimeOffset)
        {
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<ContributionType>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(contribution => contribution.CreatedBy).Use(user)
                .OnProperty(contribution => contribution.UpdatedBy).Use(user)
                .OnProperty(contribution => contribution.Contributions).IgnoreIt();

            return filler;
        }
    }
}
