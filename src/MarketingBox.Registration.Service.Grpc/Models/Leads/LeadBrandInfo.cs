using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Common;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads
{
    public class LeadBrandInfo
    {
        [DataMember(Order = 1)]
        public string Status { get; set; }

        [DataMember(Order = 2)]
        public LeadBrandRegistrationInfo Data { get; set; }
    }
}