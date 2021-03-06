namespace OrganisationRegistry.Api.LocationType.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using SqlServer.LocationType;
    using OrganisationRegistry.Organisation;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;

    public class LocationTypeListQuery: Query<LocationTypeListItem, LocationTypeListItem, LocationTypeListItemResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly IOrganisationRegistryConfiguration _configuration;

        protected override ISorting Sorting => new LocationTypeListSorting();

        public LocationTypeListQuery(OrganisationRegistryContext context, IOrganisationRegistryConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        protected override Expression<Func<LocationTypeListItem, LocationTypeListItemResult>> Transformation =>
            x => new LocationTypeListItemResult
            {
                Id = x.Id,
                Name = x.Name,
                UserPermitted = x.Id != _configuration.KboV2RegisteredOfficeLocationTypeId
            };

        protected override IQueryable<LocationTypeListItem> Filter(FilteringHeader<LocationTypeListItem> filtering)
        {
            var locationTypes = _context.LocationTypeList.AsQueryable();

            if (!filtering.ShouldFilter)
                return locationTypes;

            if (!filtering.Filter.Name.IsNullOrWhiteSpace())
                locationTypes = locationTypes.Where(x => x.Name.Contains(filtering.Filter.Name));

            return locationTypes;
        }

        private class LocationTypeListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(LocationTypeListItem.Name)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(LocationTypeListItem.Name), SortOrder.Ascending);
        }
    }

    public class LocationTypeListItemResult
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool UserPermitted { get; set; }
    }
}
