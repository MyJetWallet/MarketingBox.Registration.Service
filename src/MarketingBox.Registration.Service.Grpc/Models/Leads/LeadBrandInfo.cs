using System;
using System.Runtime.Serialization;
using System.Transactions;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads
{
    [DataContract]
    public class LeadBrandInfo
    {
        [DataMember(Order = 1)]
        public string Status { get; set; }

        [DataMember(Order = 2)]
        public LeadBrandRegistrationInfo Data { get; set; }
    }
}