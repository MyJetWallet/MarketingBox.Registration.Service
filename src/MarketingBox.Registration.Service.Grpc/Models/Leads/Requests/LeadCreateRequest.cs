using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads.Messages
{
    [DataContract]
    public class LeadCreateRequest
    {
        [DataMember(Order = 1)]
        public LeadGeneralInfo GeneralInfo { get; set; }

        [DataMember(Order = 2)]
        public string TenantId { get; set; }
    }
}
