// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    using System.Linq;

    public partial class SeatTypeListItem
    {
        /// <summary>
        /// Initializes a new instance of the SeatTypeListItem class.
        /// </summary>
        public SeatTypeListItem() { }

        /// <summary>
        /// Initializes a new instance of the SeatTypeListItem class.
        /// </summary>
        public SeatTypeListItem(System.Guid? id = default(System.Guid?), string name = default(string), int? order = default(int?))
        {
            Id = id;
            Name = name;
            Order = order;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public System.Guid? Id { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "order")]
        public int? Order { get; set; }

    }
}