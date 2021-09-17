using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Common;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads
{
    [DataContract]
    public class LeadResponse
    {
        [DataMember(Order = 1)]
        public Lead Lead { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}