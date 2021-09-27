using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Messages.Leads
{
    [DataContract]
    public class LeadBusUpdatedMessage
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2)]
        public long LeadId { get; set; }

        [DataMember(Order = 3)]
        public string UniqueId { get; set; }

        [DataMember(Order = 4)]
        public LeadBusGeneralInfo GeneralInfo { get; set; }

        [DataMember(Order = 5)]
        public LeadBusRouteInfo RouteInfo { get; set; }

        [DataMember(Order = 6)]
        public LeadBusAdditionalInfo AdditionalInfo { get; set; }

        [DataMember(Order = 7)]
        public LeadBusBrandRegistrationInfo RegistrationInfo { get; set; }
    }
}
