﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;
using GitFyle.Core.Api.Services.Foundations.Contributions;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace GitFyle.Core.Api.Controllers
{
    [ApiController]
    [Route("api/contributions")]
    public class ContributionsController : RESTFulController
    {
        private readonly IContributionService contributionService;

        public ContributionsController(IContributionService contributionService) =>
            this.contributionService = contributionService;

        [HttpGet]
        public async ValueTask<ActionResult<IQueryable<Contribution>>> GetAllContributionsAsync()
        {
            try
            {
                IQueryable<Contribution> contributions =
                     await this.contributionService.RetrieveAllContributionsAsync();

                return Ok(contributions);
            }
            catch (ContributionDependencyException contributionDependencyException)
            {
                return InternalServerError(contributionDependencyException);
            }
            catch (ContributionServiceException contributionServiceException)
            {
                return InternalServerError(contributionServiceException);
            }
        }
    }
}
