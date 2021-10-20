using System;
using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Messages.Leads
{
    [DataContract]
    public class LeadUpdateMessage
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }
      
        [DataMember(Order = 2)]
        public LeadGeneralInfo GeneralInfo { get; set; }

        [DataMember(Order = 3)]
        public LeadRouteInfo RouteInfo { get; set; }

        [DataMember(Order = 4)]
        public LeadAdditionalInfo AdditionalInfo { get; set; }
        
        [DataMember(Order = 5)]
        public long Sequence { get; set; }
    }
}
