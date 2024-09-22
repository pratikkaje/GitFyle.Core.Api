﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;

namespace GitFyle.Core.Api.Services.Foundations.Configurations
{
    internal partial class ConfigurationService
    {
        private async ValueTask ValidateConfigurationOnAdd(Configuration configuration)
        {
            ValidateConfigurationIsNotNull(configuration);

            Validate(
                (Rule: await IsInvalidAsync(configuration.Id), Parameter: nameof(configuration.Id)),
                (Rule: await IsInvalidAsync(configuration.Name), Parameter: nameof(configuration.Name)),
                (Rule: await IsInvalidAsync(configuration.Value), Parameter: nameof(configuration.Value)),
                (Rule: await IsInvalidAsync(configuration.CreatedBy), Parameter: nameof(configuration.CreatedBy)),
                (Rule: await IsInvalidAsync(configuration.CreatedDate), Parameter: nameof(configuration.CreatedDate)),
                (Rule: await IsInvalidAsync(configuration.UpdatedBy), Parameter: nameof(configuration.UpdatedBy)),
                (Rule: await IsInvalidAsync(configuration.UpdatedDate), Parameter: nameof(configuration.UpdatedDate)),
                (Rule: await IsInvalidLengthAsync(configuration.Name, 450), Parameter: nameof(Configuration.Name)),

                (Rule: await IsNotSameAsync(
                    first: configuration.CreatedBy,
                    second: configuration.UpdatedBy,
                    secondName: nameof(configuration.CreatedBy)),

                    Parameter: nameof(configuration.UpdatedBy)),

                (Rule: await IsNotSameAsync(
                    firstDate: configuration.CreatedDate,
                    secondDate: configuration.UpdatedDate,
                    secondDateName: nameof(configuration.CreatedDate)),

                    Parameter: nameof(configuration.UpdatedDate)),

                (Rule: await IsNotRecentAsync(configuration.CreatedDate),
                Parameter: nameof(configuration.CreatedDate)));
        }

        private static async ValueTask<dynamic> IsInvalidLengthAsync(string text, int maxLength) => new
        {
            Condition = await IsExceedingLengthAsync(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static async ValueTask<bool> IsExceedingLengthAsync(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

        private async ValueTask<dynamic> IsNotRecentAsync(DateTimeOffset date)
        {
            var (isNotRecent, startDate, endDate) = await IsDateNotRecentAsync(date);

            return new
            {
                Condition = isNotRecent,
                Message = $"Date is not recent. Expected a value between {startDate} and {endDate} but found {date}"
            };
        }

        private async ValueTask<(bool IsNotRecent, DateTimeOffset StartDate, DateTimeOffset EndDate)>
            IsDateNotRecentAsync(DateTimeOffset date)
        {
            int pastSeconds = 60;
            int futureSeconds = 0;

            DateTimeOffset currentDateTime =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            if (currentDateTime == default)
            {
                return (false, default, default);
            }

            TimeSpan timeDifference = currentDateTime.Subtract(date);
            DateTimeOffset startDate = currentDateTime.AddSeconds(-pastSeconds);
            DateTimeOffset endDate = currentDateTime.AddSeconds(futureSeconds);
            bool isNotRecent = timeDifference.TotalSeconds is > 60 or < 0;

            return (isNotRecent, startDate, endDate);
        }

        private static async ValueTask<dynamic> IsNotSameAsync(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not same as {secondDateName}"
            };

        private static async ValueTask<dynamic> IsNotSameAsync(
            string first,
            string second,
            string secondName) => new
            {
                Condition = first != second,
                Message = $"Text is not same as {secondName}"
            };

        private static async ValueTask ValidateConfigurationIdAsync(Guid configurationId) =>
            Validate((Rule: await IsInvalidAsync(configurationId), Parameter: nameof(Configuration.Id)));
        private static async ValueTask<dynamic> IsInvalidAsync(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid."
        };

        private static async ValueTask<dynamic> IsInvalidAsync(string name) => new
        {
            Condition = String.IsNullOrWhiteSpace(name),
            Message = "Text is required."
        };

        private static async ValueTask<dynamic> IsInvalidAsync(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is invalid."
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidConfigurationException = new InvalidConfigurationException(
                message: "Configuration is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidConfigurationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidConfigurationException.ThrowIfContainsErrors();
        }

        private static void ValidateConfigurationIsNotNull(Configuration configuration)
        {
            if (configuration is null)
            {
                throw new NullConfigurationException(message: "Configuration is null");
            }
        }
    }
}
