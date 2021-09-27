using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Messages.Leads
{
    [DataContract]
    public class LeadBusUpdateMessage
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
        public LeadBusGeneralInfo GeneralInfo { get; set; }

        [DataMember(Order = 6)]
        public LeadBusRouteInfo RouteInfo { get; set; }

        [DataMember(Order = 7)]
        public LeadBusAdditionalInfo AdditionalInfo { get; set; }

        [DataMember(Order = 8)]
        public LeadBusBrandRegistrationInfo RegistrationInfo { get; set; }
    }
}
