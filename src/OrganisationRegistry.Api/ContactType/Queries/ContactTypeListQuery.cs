namespace OrganisationRegistry.Api.ContactType.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using SqlServer.ContactType;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;

    public class ContactTypeListQuery: Query<ContactTypeListItem>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new ContactTypeListSorting();

        public ContactTypeListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<ContactTypeListItem> Filter(FilteringHeader<ContactTypeListItem> filtering)
        {
            var contactTypes = _context.ContactTypeList.AsQueryable();

            if (!filtering.ShouldFilter)
                return contactTypes;

            if (!filtering.Filter.Name.IsNullOrWhiteSpace())
                contactTypes = contactTypes.Where(x => x.Name.Contains(filtering.Filter.Name));

            return contactTypes;
        }

        private class ContactTypeListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(ContactTypeListItem.Name)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(ContactTypeListItem.Name), SortOrder.Ascending);
        }
    }
}
