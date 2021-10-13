using System;
using System.Runtime.Serialization;
using System.Transactions;
using MarketingBox.Registration.Service.Grpc.Models.Common;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads
{
    [DataContract]
    public class LeadBrandInfo
    {
        [DataMember(Order = 1)]
        public ResultCode Status { get; set; }

        [DataMember(Order = 2)]
        public LeadBrandRegistrationInfo Data { get; set; }

        [DataMember(Order = 3)]
        public string Brand { get; set; }
    }
}