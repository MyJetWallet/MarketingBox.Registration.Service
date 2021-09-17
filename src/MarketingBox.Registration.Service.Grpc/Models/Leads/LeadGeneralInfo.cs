using System;
using System.Runtime.Serialization;
using Destructurama.Attributed;
using MarketingBox.Registration.Service.Grpc.Models.Common;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads
{
    [DataContract]
    public class LeadGeneralInfo
    {
        [DataMember(Order = 1)]
        public string Username { get; set; }
        
        [DataMember(Order = 2)]
        [LogMasked(PreserveLength = false)]
        public string Password { get; set; }
        
        [DataMember(Order = 3)]
        [LogMasked(PreserveLength = true, ShowFirst = 1, ShowLast = 1)]
        public string Email { get; set; }
        
        [DataMember(Order = 4)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Phone { get; set; }

        [DataMember(Order = 5)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Skype { get; set; }
        
        [DataMember(Order = 6)]
        public string ZipCode { get; set; }
        
        [DataMember(Order = 7)]
        public LeadState State { get; set; }
        
        [DataMember(Order = 8)]
        public Currency Currency { get; set; }
        
        [DataMember(Order = 9)]
        public DateTime CreatedAt { get; set; }
    }
}