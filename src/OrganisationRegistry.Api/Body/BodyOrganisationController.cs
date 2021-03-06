﻿namespace OrganisationRegistry.Api.Body
{
    using System;
    using System.Net;
    using Infrastructure;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SqlServer.Body;
    using OrganisationRegistry.Infrastructure.Commands;
    using System.Threading.Tasks;
    using Infrastructure.Security;
    using OrganisationRegistry.Infrastructure.Authorization;
    using Queries;
    using Requests;
    using Security;
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("bodies/{bodyId}/organisations")]
    public class BodyOrganisationController : OrganisationRegistryController
    {
        public BodyOrganisationController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available organisations for a body.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId)
        {
            var filtering = Request.ExtractFilteringRequest<BodyOrganisationListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedOrganisations =
                new BodyOrganisationListQuery(context, bodyId).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedOrganisations.Items.ToListAsync());
        }

        /// <summary>Get an organisation for a body.</summary>
        /// <response code="200">If the organisation is found.</response>
        /// <response code="404">If the organisation cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BodyOrganisationListItem), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId, [FromRoute] Guid id)
        {
            var bodyOrganisation =
                await context.BodyOrganisationList.FirstOrDefaultAsync(x => x.BodyOrganisationId == id);

            if (bodyOrganisation == null)
                return NotFound();

            return Ok(bodyOrganisation);
        }

        /// <summary>Link an organisation to a body.</summary>
        /// <response code="201">If the organisation is linked, together with the location.</response>
        /// <response code="400">If the organisation information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder + "," + Roles.OrgaanBeheerder)]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromServices] ISecurityService securityService, [FromRoute] Guid bodyId, [FromBody] AddBodyOrganisationRequest message)
        {
            var internalMessage = new AddBodyOrganisationInternalRequest(bodyId, message);

            if (!securityService.CanEditBody(User, internalMessage.BodyId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit orgaan.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(AddBodyOrganisationRequestMapping.Map(internalMessage));

            return Created(Url.Action(nameof(Get), new { id = message.BodyOrganisationId }), null);
        }

        /// <summary>Update an organisation for a body.</summary>
        /// <response code="201">If the organisation is updated, together with the location.</response>
        /// <response code="400">If the organisation information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder + "," + Roles.OrgaanBeheerder)]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put([FromServices] ISecurityService securityService, [FromRoute] Guid bodyId, [FromBody] UpdateBodyOrganisationRequest message)
        {
            var internalMessage = new UpdateBodyOrganisationInternalRequest(bodyId, message);

            if (!securityService.CanEditBody(User, internalMessage.BodyId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit orgaan.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateBodyOrganisationRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.BodyId }));
        }
    }
}
