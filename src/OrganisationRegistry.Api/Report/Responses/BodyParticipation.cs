namespace OrganisationRegistry.Api.Report.Responses
{
    using Infrastructure;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.ComponentModel;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure.Search.Filtering;
    using Microsoft.EntityFrameworkCore;
    using SqlServer.Reporting;
    using OrganisationRegistry.Person;

    public class BodyParticipation
    {
        [ExcludeFromCsv] public Guid BodyId { get; set; }
        [DisplayName("Orgaan")] public string BodyName { get; set; }

        [ExcludeFromCsv] public bool? IsEffective { get; set; }
        [DisplayName("Is Effectief")] public string IsEffectiveTranslation { get; set; }

        [DisplayName("Percentage Man")] public decimal MalePercentage { get; set; }
        [DisplayName("Percentage Vrouw")] public decimal FemalePercentage { get; set; }
        [DisplayName("Percentage Onbekend")] public decimal UnknownPercentage { get; set; }

        [DisplayName("Aantal Man")] public int MaleCount { get; set; }
        [DisplayName("Aantal Vrouw")] public int FemaleCount { get; set; }
        [DisplayName("Aantal Onbekend")] public int UnknownCount { get; set; }

        [DisplayName("Totaal Aantal")] public int TotalCount { get; set; }

        [ExcludeFromCsv] public int AssignedCount { get; set; }
        [ExcludeFromCsv] public int UnassignedCount { get; set; }

        ///  <summary>
        ///
        ///  </summary>
        ///  <param name="context"></param>
        ///  <param name="bodyId"></param>
        /// <param name="filteringHeader"></param>
        /// <returns></returns>
        public static IEnumerable<BodyParticipation> Search(
            OrganisationRegistryContext context,
            Guid bodyId,
            FilteringHeader<BodyParticipationFilter> filteringHeader)
        {
            // No checkboxes are enabled
            if (!filteringHeader.Filter.EntitledToVote && !filteringHeader.Filter.NotEntitledToVote)
                return new List<BodyParticipation>();

            var body = context.BodySeatGenderRatioBodyList
                .Include(item => item.PostsPerType)
                .Single(item => item.BodyId == bodyId);

            var activeSeatsPerType = body
                .PostsPerType
                .Where(post =>
                    (!post.BodySeatValidFrom.HasValue || post.BodySeatValidFrom <= DateTime.Today) &&
                    (!post.BodySeatValidTo.HasValue || post.BodySeatValidTo >= DateTime.Today));

            // One of the checkboxes is checked
            if (filteringHeader.Filter.EntitledToVote ^ filteringHeader.Filter.NotEntitledToVote)
                if (filteringHeader.Filter.EntitledToVote)
                    activeSeatsPerType = activeSeatsPerType.Where(x => x.EntitledToVote);
                else if (filteringHeader.Filter.NotEntitledToVote)
                    activeSeatsPerType = activeSeatsPerType.Where(x => !x.EntitledToVote);

            var bodySeatGenderRatioPostsPerTypeItems = activeSeatsPerType.ToList();

            var activeSeatIds = bodySeatGenderRatioPostsPerTypeItems
                .Select(item => item.BodySeatId)
                .ToList();

            var activeSeatsPerIsEffective = bodySeatGenderRatioPostsPerTypeItems
                .ToList()
                .GroupBy(mandate => mandate.BodySeatTypeIsEffective)
                .ToDictionary(
                    x => x.Key,
                    x => x);

            var activeMandates = context.BodySeatGenderRatioBodyMandateList
                .AsAsyncQueryable()
                .Where(mandate => mandate.BodyId == bodyId)
                .Where(mandate =>
                    (!mandate.BodyMandateValidFrom.HasValue || mandate.BodyMandateValidFrom <= DateTime.Today) &&
                    (!mandate.BodyMandateValidTo.HasValue || mandate.BodyMandateValidTo >= DateTime.Today))
                .Where(mandate => activeSeatIds.Contains(mandate.BodySeatId))
                .ToList();

            var activeMandateIds = activeMandates
                .Select(item => item.BodyMandateId)
                .ToList();

            var activeAssignmentsPerIsEffective =
                context.BodySeatGenderRatioBodyMandateList
                    .Include(mandate => mandate.Assignments)
                    .AsAsyncQueryable()
                    .Where(mandate => mandate.BodyId == bodyId)
                    .Where(mandate =>
                        (!mandate.BodyMandateValidFrom.HasValue || mandate.BodyMandateValidFrom <= DateTime.Today) &&
                        (!mandate.BodyMandateValidTo.HasValue || mandate.BodyMandateValidTo >= DateTime.Today))
                    .Where(mandate => activeMandateIds.Contains(mandate.BodyMandateId))
                    .ToList()
                    .GroupBy(mandate => mandate.BodySeatTypeIsEffective)
                    .ToDictionary(
                        x => x.Key,
                        x => x.SelectMany(y => y.Assignments));

            var groupedResults = activeSeatsPerIsEffective
                .Select(seatPer =>
                {
                    var totalCount = seatPer.Value.Count();
                    var activeAssignments = activeAssignmentsPerIsEffective[seatPer.Key].ToList();
                    var assignedCount = activeAssignments.Count;
                    return new BodyParticipation
                    {
                        BodyId = bodyId,
                        BodyName = body.BodyName, //name
                        IsEffective = seatPer.Key,
                        IsEffectiveTranslation = seatPer.Key ? "Effectief" : "Niet effectief",

                        MaleCount = activeAssignments.Count(x => x.Sex == Sex.Male),
                        FemaleCount = activeAssignments.Count(x => x.Sex == Sex.Female),
                        UnknownCount = activeAssignments.Count(x => !x.Sex.HasValue),

                        AssignedCount = assignedCount,
                        UnassignedCount = totalCount - assignedCount,

                        TotalCount = totalCount
                    };
                })
                .ToList();


            // var groupedResults = activePostsPerType
            //     .Select(g =>
            //     {
            //         var isEffective = g.Key.BodySeatTypeIsEffective;
            //         var assignmentsPerType =
            //             activeAssignmentsPerBodySeatTypeId.ContainsKey(isEffective) ?
            //             activeAssignmentsPerBodySeatTypeId[isEffective] :
            //             new List<BodySeatGenderRatioAssignmentItem>();
            //
            //         var totalCount = g.Count();
            //         var assignedCount = assignmentsPerType.Count;
            //         return new BodyParticipation
            //         {
            //             BodyId = g.Key.BodyId,
            //             BodyName = g.Key.BodyName,
            //             IsEffective = isEffective,
            //             IsEffectiveTranslation = isEffective ?? false ? "Effectief" : "Niet effectief",
            //
            //             MaleCount = assignmentsPerType.Count(x => x.Sex == Sex.Male),
            //             FemaleCount = assignmentsPerType.Count(x => x.Sex == Sex.Female),
            //             UnknownCount = assignmentsPerType.Count(x => !x.Sex.HasValue),
            //
            //             AssignedCount = assignedCount,
            //             UnassignedCount = totalCount - assignedCount,
            //
            //             TotalCount = totalCount
            //         };
            //     })
            //     .ToList();

            return groupedResults;
        }

        /// <summary>
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static IEnumerable<BodyParticipation> Map(
            IEnumerable<BodyParticipation> results)
        {
            var participations = new List<BodyParticipation>();

            foreach (var result in results)
            {
                if (result.AssignedCount > 0)
                {
                    result.MalePercentage = Math.Round((decimal)result.MaleCount / result.AssignedCount, 2);
                    result.FemalePercentage = Math.Round((decimal)result.FemaleCount / result.AssignedCount, 2);
                    result.UnknownPercentage = Math.Round((decimal)result.UnknownCount / result.AssignedCount, 2);
                }

                participations.Add(result);
            }

            return participations;
        }

        ///  <summary>
        ///  </summary>
        ///  <param name="results"></param>
        ///  <param name="sortingHeader"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<BodyParticipation> Sort(
            IEnumerable<BodyParticipation> results,
            SortingHeader sortingHeader)
        {
            if (!sortingHeader.ShouldSort)
                return results.OrderBy(x => x.BodyName).ThenBy(x => x.IsEffectiveTranslation);

            switch (sortingHeader.SortBy.ToLowerInvariant())
            {
                case "malepercentage":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.MalePercentage)
                        : results.OrderByDescending(x => x.MalePercentage);
                case "femalepercentage":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.FemalePercentage)
                        : results.OrderByDescending(x => x.FemalePercentage);
                case "unknownpercentage":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.UnknownPercentage)
                        : results.OrderByDescending(x => x.UnknownPercentage);
                case "iseffectivetranslation":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.IsEffectiveTranslation)
                        : results.OrderByDescending(x => x.IsEffectiveTranslation);
                default:
                    return results.OrderBy(x => x.BodyName).ThenBy(x => x.IsEffectiveTranslation);
            }
        }
    }

    public class BodyParticipationFilter
    {
        public bool EntitledToVote { get; set; }

        public bool NotEntitledToVote { get; set; }
    }
}
