namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationFormalFramework.WhenDayHasPassed
{
    using System;
    using System.Collections.Generic;
    using Day.Events;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Organisation.ScheduledActions.FormalFramework;
    using TestBases;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using UnitTests;
    using Xunit;

    [Collection(SqlServerTestsCollection.Name)]
    public class GivenAnOrganisationFormalFrameworksLastDayHasNotPassed : ReactionCommandsTestBase<ActiveOrganisationFormalFrameworkListView, DayHasPassed>
    {
        private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();

        public GivenAnOrganisationFormalFrameworksLastDayHasNotPassed(SqlServerFixture fixture) : base()
        {
            Guid.NewGuid();
        }

        protected override ActiveOrganisationFormalFrameworkListView BuildReactionHandler(IContextFactory contextFactory)
        {
            return new ActiveOrganisationFormalFrameworkListView(
                Mock.Of<ILogger<ActiveOrganisationFormalFrameworkListView>>(),
                null,
                new DateTimeProviderStub(DateTime.Now),
                contextFactory);
        }

        protected override IEnumerable<IEvent> Given()
        {
            var formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedTestDataBuilder();
            var formalFrameworkCreated = new FormalFrameworkCreatedTestDataBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);
            var parentOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            var childOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            var organisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedTestDataBuilder(childOrganisationCreated.Id, formalFrameworkCreated.Id, parentOrganisationCreated.Id)
                .WithValidity(DateTime.Today, DateTime.Today.AddDays(2));
            return new List<IEvent>
            {
                parentOrganisationCreated.Build(),
                childOrganisationCreated.Build(),
                formalFrameworkCategoryCreated.Build(),
                formalFrameworkCreated.Build(),
                organisationFormalFrameworkAdded.Build(),
            };
        }

        protected override DayHasPassed When()
        {
            return new DayHasPassed(Guid.NewGuid(), DateTime.Today);
        }

        protected override int ExpectedNumberOfCommands => 0;
    }
}
