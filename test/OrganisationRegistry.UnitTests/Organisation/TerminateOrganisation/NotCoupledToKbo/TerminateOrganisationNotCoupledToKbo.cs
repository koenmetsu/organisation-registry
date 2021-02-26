namespace OrganisationRegistry.UnitTests.Organisation.TerminateOrganisation.NotCoupledToKbo
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using AutoFixture;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Kbo;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Tests.Shared;
    using Xunit;
    using Xunit.Abstractions;

    public class TerminateOrganisationNotCoupledToKbo: Specification<Organisation, OrganisationCommandHandlers, TerminateOrganisation>
    {
        private OrganisationRegistryConfigurationStub _organisationRegistryConfigurationStub;

        private OrganisationId _organisationId;
        private DateTimeProviderStub _dateTimeProviderStub;
        private DateTime _dateOfTermination;

        protected override IEnumerable<IEvent> Given()
        {
            var fixture = new Fixture();
            _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Today);
            _organisationRegistryConfigurationStub = new OrganisationRegistryConfigurationStub
            {
                FormalFrameworkIdsToTerminateEndOfNextYear = new []{Guid.NewGuid()},
                OrganisationCapacityTypeIdsToTerminateEndOfNextYear = new []{Guid.NewGuid()},
                OrganisationClassificationTypeIdsToTerminateEndOfNextYear = new []{Guid.NewGuid()}
            };
            _dateOfTermination = _dateTimeProviderStub.Today.AddDays(fixture.Create<int>());
            _organisationId = new OrganisationId(Guid.NewGuid());

            return new List<IEvent>
            {
                new OrganisationCreated(
                    _organisationId,
                    "organisation X",
                    "OVO001234",
                    "org",
                    "",
                    new List<Purpose>(),
                    false,
                    new ValidFrom(),
                    new ValidTo()),
                new OrganisationLabelAdded(
                    _organisationId,
                    fixture.Create<Guid>(),
                    fixture.Create<Guid>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    _dateOfTermination.AddDays(fixture.Create<int>()*-1),
                    _dateOfTermination.AddDays(fixture.Create<int>())
                ),
                new OrganisationLabelAdded(
                    _organisationId,
                    fixture.Create<Guid>(),
                    fixture.Create<Guid>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    _dateOfTermination.AddDays(fixture.Create<int>()*-1),
                    _dateOfTermination.AddDays(fixture.Create<int>())
                )
            };
        }

        protected override TerminateOrganisation When()
        {
            return new TerminateOrganisation(
                _organisationId,
                _dateOfTermination,
                false,
                new ClaimsPrincipal());
        }

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                new UniqueOvoNumberValidatorStub(false),
                _dateTimeProviderStub,
                _organisationRegistryConfigurationStub);
        }

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void TerminatesTheOrganisation()
        {
            var organisationTerminated = PublishedEvents[0].UnwrapBody<OrganisationTerminated>();
            organisationTerminated.Should().NotBeNull();

            organisationTerminated.OrganisationId.Should().Be((Guid) _organisationId);
            organisationTerminated.FieldsToTerminate.OrganisationValidity.Should().Be(_dateOfTermination);
            organisationTerminated.OvoNumber.Should().Be("OVO001234");
            organisationTerminated.FieldsToTerminate.Buildings.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.Capacities.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.Classifications.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.Contacts.Should().BeEmpty();
            organisationTerminated.DateOfTermination.Should().Be(_dateOfTermination);
            organisationTerminated.ForcedKboTermination.Should().BeFalse();
            organisationTerminated.FieldsToTerminate.Functions.Should().BeEmpty();
            organisationTerminated.KboFieldsToTerminate.FormalName.Should().BeNull();
            organisationTerminated.KboFieldsToTerminate.LegalForm.Should().BeNull();
            organisationTerminated.KboFieldsToTerminate.RegisteredOffice.Should().BeNull();
            organisationTerminated.FieldsToTerminate.Labels.Should().HaveCount(2);
            organisationTerminated.FieldsToTerminate.Locations.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.Parents.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.Relations.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.BankAccounts.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.FormalFrameworks.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.OpeningHours.Should().BeEmpty();
            organisationTerminated.KboFieldsToTerminate.BankAccounts.Should().BeEmpty();
            organisationTerminated.DateOfTerminationAccordingToKbo.Should().BeNull();
        }

        public TerminateOrganisationNotCoupledToKbo(ITestOutputHelper helper) : base(helper) { }
    }
}