namespace OrganisationRegistry.Api.Organisation.Requests
{
    using System;

    public class OrganisationTerminationRequest
    {
        public DateTime DateOfTermination { get; set; }
        public bool ForceKboTermination { get; set; }
    }
}
