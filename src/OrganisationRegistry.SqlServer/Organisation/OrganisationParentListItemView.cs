namespace OrganisationRegistry.SqlServer.Organisation
{
    using System;
    using System.Data.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;
    using System.Linq;
    using Microsoft.Extensions.Logging;

    public class OrganisationParentListItem
    {
        public Guid OrganisationOrganisationParentId { get; set; }
        public Guid OrganisationId { get; set; }

        public Guid ParentOrganisationId { get; set; }
        public string ParentOrganisationName { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class OrganisationParentListConfiguration : EntityMappingConfiguration<OrganisationParentListItem>
    {
        public override void Map(EntityTypeBuilder<OrganisationParentListItem> b)
        {
            b.ToTable(nameof(OrganisationParentListView.ProjectionTables.OrganisationParentList), "OrganisationRegistry")
                .HasKey(p => p.OrganisationOrganisationParentId)
                .ForSqlServerIsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.ParentOrganisationId).IsRequired();
            b.Property(p => p.ParentOrganisationName).HasMaxLength(OrganisationListConfiguration.NameLength).IsRequired();

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.ParentOrganisationName).ForSqlServerIsClustered();
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class OrganisationParentListView :
        Projection<OrganisationParentListView>,
        IEventHandler<OrganisationParentAdded>,
        IEventHandler<OrganisationParentUpdated>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationParentList
        }

        private readonly IEventStore _eventStore;

        public OrganisationParentListView(
            ILogger<OrganisationParentListView> logger,
            IEventStore eventStore) : base(logger)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            UpdateParentOrganisationName(dbConnection, dbTransaction, message.Body.OrganisationId, message.Body.Name);
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            UpdateParentOrganisationName(dbConnection, dbTransaction, message.Body.OrganisationId, message.Body.Name);
        }

        private static void UpdateParentOrganisationName(DbConnection dbConnection, DbTransaction dbTransaction, Guid organisationId, string organisationName)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var organisations =
                    context.OrganisationParentList.Where(x => x.ParentOrganisationId == organisationId);
                if (!organisations.Any())
                    return;

                foreach (var organisation in organisations)
                    organisation.ParentOrganisationName = organisationName;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationParentAdded> message)
        {
            var organisationParentListItem = new OrganisationParentListItem
            {
                OrganisationOrganisationParentId = message.Body.OrganisationOrganisationParentId,
                OrganisationId = message.Body.OrganisationId,
                ParentOrganisationId = message.Body.ParentOrganisationId,
                ParentOrganisationName = message.Body.ParentOrganisationName,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.OrganisationParentList.Add(organisationParentListItem);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationParentUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var key = context.OrganisationParentList.SingleOrDefault(item => item.OrganisationOrganisationParentId == message.Body.OrganisationOrganisationParentId);

                key.OrganisationOrganisationParentId = message.Body.OrganisationOrganisationParentId;
                key.OrganisationId = message.Body.OrganisationId;
                key.ParentOrganisationId = message.Body.ParentOrganisationId;
                key.ParentOrganisationName = message.Body.ParentOrganisationName;
                key.ValidFrom = message.Body.ValidFrom;
                key.ValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}