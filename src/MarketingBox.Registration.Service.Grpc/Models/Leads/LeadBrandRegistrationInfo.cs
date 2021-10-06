using System.Runtime.Serialization;
using Destructurama.Attributed;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads
{
    [DataContract]
    public class LeadBrandRegistrationInfo
    {
        [DataMember(Order = 1)]
        public string CustomerId { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public string LoginUrl { get; set; }
    }
}