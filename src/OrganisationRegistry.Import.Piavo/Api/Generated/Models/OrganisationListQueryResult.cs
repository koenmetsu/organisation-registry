// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    using System.Linq;

    public partial class OrganisationListQueryResult
    {
        /// <summary>
        /// Initializes a new instance of the OrganisationListQueryResult
        /// class.
        /// </summary>
        public OrganisationListQueryResult() { }

        /// <summary>
        /// Initializes a new instance of the OrganisationListQueryResult
        /// class.
        /// </summary>
        public OrganisationListQueryResult(System.Guid? id = default(System.Guid?), string ovoNumber = default(string), string name = default(string), string shortName = default(string), string parentOrganisationOvoNumber = default(string), string parentOrganisation = default(string), System.Guid? parentOrganisationId = default(System.Guid?))
        {
            Id = id;
            OvoNumber = ovoNumber;
            Name = name;
            ShortName = shortName;
            ParentOrganisationOvoNumber = parentOrganisationOvoNumber;
            ParentOrganisation = parentOrganisation;
            ParentOrganisationId = parentOrganisationId;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public System.Guid? Id { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "ovoNumber")]
        public string OvoNumber { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "shortName")]
        public string ShortName { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "parentOrganisationOvoNumber")]
        public string ParentOrganisationOvoNumber { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "parentOrganisation")]
        public string ParentOrganisation { get; private set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "parentOrganisationId")]
        public System.Guid? ParentOrganisationId { get; private set; }

    }
}