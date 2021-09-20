using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads
{
    public class LeadRoutingInfo
    {
        [DataMember(Order = 1)]
        public string AffiliateId { get; set; }

        [DataMember(Order = 2)]
        public string CampaignId { get; set; }

        [DataMember(Order = 3)]
        public string BoxId { get; set; }
    }
}