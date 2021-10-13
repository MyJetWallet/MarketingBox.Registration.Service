using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Common;

namespace MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts
{
    [DataContract]
    public class DepositApproveResponse
    {
        [DataMember(Order = 1)]
        public ResultCode Status { get; set; }

        [DataMember(Order = 2)]
        public ApproveResult Result { get; set; }

        [DataMember(Order = 3)]
        public long DepositId { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}