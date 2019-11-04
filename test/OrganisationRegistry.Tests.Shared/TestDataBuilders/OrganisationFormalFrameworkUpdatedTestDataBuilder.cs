namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using Organisation.Events;

    public class OrganisationFormalFrameworkUpdatedTestDataBuilder
    {
        public Guid OrganisationFormalFrameworkId { get; }
        public Guid OrganisationId { get; }
        public Guid FormalFrameworkId { get; }
        public Guid PreviousParentOrganisationId { get; }
        public Guid ParentOrganisationId { get; }
        public string ParentOrganisationName { get; }
        public DateTime? ValidFrom { get; private set; }
        public DateTime? ValidTo { get; private set; }

        public OrganisationFormalFrameworkUpdatedTestDataBuilder(
            Guid organisationFormalFrameworkId,
            Guid organisationId,
            Guid formalFrameworkId,
            Guid previousParentOrganisationId,
            Guid parentOrganisationId)
        {
            OrganisationFormalFrameworkId = organisationFormalFrameworkId;
            OrganisationId = organisationId;
            FormalFrameworkId = formalFrameworkId;
            PreviousParentOrganisationId = previousParentOrganisationId;
            ParentOrganisationId = parentOrganisationId;
            ParentOrganisationName = parentOrganisationId.ToString();
            ValidFrom = null;
            ValidTo = null;
        }

        public OrganisationFormalFrameworkUpdatedTestDataBuilder WithValidity(DateTime? from, DateTime? to)
        {
            ValidFrom = from;
            ValidTo = to;
            return this;
        }

        public OrganisationFormalFrameworkUpdated Build()
            => new OrganisationFormalFrameworkUpdated(
                OrganisationId,
                OrganisationFormalFrameworkId,
                FormalFrameworkId, FormalFrameworkId.ToString(),
                ParentOrganisationId, ParentOrganisationName,
                ValidFrom, ValidTo,
                PreviousParentOrganisationId, PreviousParentOrganisationId.ToString(), null, null);
    }
}
