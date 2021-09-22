using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads.Requests
{
    [DataContract]
    public class LeadCreateRequest
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2)]
        public LeadGeneralInfo GeneralInfo { get; set; }

        [DataMember(Order = 3)]
        public LeadRouteInfo Route { get; set; }

        [DataMember(Order = 4)]
        public LeadAdditionalInfo AdditionalInfo { get; set; }

    }
}
