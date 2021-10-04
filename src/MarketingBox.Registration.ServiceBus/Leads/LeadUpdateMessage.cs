using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Messages.Leads
{
    [DataContract]
    public class LeadUpdateMessage
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2)]
        public long LeadId { get; set; }

        [DataMember(Order = 3)]
        public string UniqueId { get; set; }
        
        [DataMember(Order = 4)]
        public long Sequence { get; set; }

        [DataMember(Order = 5)]
        public LeadGeneralInfo GeneralInfo { get; set; }

        [DataMember(Order = 6)]
        public LeadRouteInfo RouteInfo { get; set; }

        [DataMember(Order = 7)]
        public LeadAdditionalInfo AdditionalInfo { get; set; }

        [DataMember(Order = 8)]
        public LeadBrandRegistrationInfo RegistrationInfo { get; set; }

        [DataMember(Order = 9)] 
        public LeadType Type  { get; set; }

        [DataMember(Order = 10)]
        public LeadStatus CallStatus{ get; set; }


    }
}
