using System;
using System.Runtime.Serialization;
using Destructurama.Attributed;
using MarketingBox.Registration.Service.Grpc.Models.Common;

namespace MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts
{
    [DataContract]
    public class DepositApproveResponse
    {
        [DataMember(Order = 1)]
        public DepositResponse Deposit { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }

    }
}