using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads.Contracts
{
    [DataContract]
    public class LeadDeleteRequest
    {
        [DataMember(Order = 1)]
        public long LeadId { get; set; }
    }
}
