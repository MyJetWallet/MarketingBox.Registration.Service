using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Common;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads.Contracts
{
    [DataContract]
    public class LeadCreateResponse
    {
        [DataMember(Order = 1)]
        public long LeadId { get; set; }

        [DataMember(Order = 1)]
        public LeadBrandInfo BrandInfo{ get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}