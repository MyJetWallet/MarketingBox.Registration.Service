using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads
{
    public class LeadBrandInfo
    {
        [DataMember(Order = 1)]
        public string Status { get; set; }

        [DataMember(Order = 2)]
        //public LeadBrandRegistrationInfo Data { get; set; }
        public string Data { get; set; }
    }
}