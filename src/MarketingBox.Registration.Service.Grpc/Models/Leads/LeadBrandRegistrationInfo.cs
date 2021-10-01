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
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Email { get; set; }

        [DataMember(Order = 3)]
        public string Token { get; set; }

        [DataMember(Order = 4)]
        public string UniqueId { get; set; }

        [DataMember(Order = 5)]
        public string LoginUrl { get; set; }

        [DataMember(Order = 6)]
        public string Broker { get; set; }
    }
}