using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads
{
    [DataContract]
    public class Lead
    {
        [DataMember(Order = 1)]
        public long LeadId { get; set; }

        [DataMember(Order = 2)]
        public LeadGeneralInfo GeneralInfo { get; set; }

        [DataMember(Order = 3)]
        public LeadAuthInfo Route { get; set; }

        [DataMember(Order = 4)]
        public LeadAdditionalInfo AdditionalInfo { get; set; }

        [DataMember(Order = 5)]
        public string TenantId { get; set; }

        [DataMember(Order = 6)]
        public long Sequence { get; set; }
    }
}
