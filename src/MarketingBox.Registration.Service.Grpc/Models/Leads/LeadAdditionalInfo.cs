using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads
{
    public class LeadAdditionalInfo
    {
        [DataMember(Order = 1)]
        public string So { get; set; }

        [DataMember(Order = 2)]
        public string Sub { get; set; }

        [DataMember(Order = 3)]
        public string MPC_1 { get; set; }

        [DataMember(Order = 4)]
        public string MPC_2 { get; set; }

        [DataMember(Order = 5)]
        public string MPC_3 { get; set; }

        [DataMember(Order = 6)]
        public string MPC_4 { get; set; }

        [DataMember(Order = 7)]
        public string MPC_5 { get; set; }

        [DataMember(Order = 8)]
        public string MPC_6 { get; set; }

        [DataMember(Order = 9)]
        public string MPC_7 { get; set; }

        [DataMember(Order = 10)]
        public string MPC_8 { get; set; }

        [DataMember(Order = 11)]
        public string MPC_9 { get; set; }

        [DataMember(Order = 12)]
        public string MPC_10 { get; set; }
    }
}