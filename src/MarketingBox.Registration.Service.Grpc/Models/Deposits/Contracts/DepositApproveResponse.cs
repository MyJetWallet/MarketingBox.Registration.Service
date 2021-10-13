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
        public string TenantId { get; set; }

        [DataMember(Order = 2)]
        public long LeadId { get; set; }

        [DataMember(Order = 3)]
        public string BrandName { get; set; }

        [DataMember(Order = 4)]
        public long Sequence { get; set; }

        [DataMember(Order = 5)]
        public long BrandId { get; set; }

        [DataMember(Order = 6)]
        public long AffiliateId { get; set; }

        [DataMember(Order = 7)]
        public long CampaignId { get; set; }

        [DataMember(Order = 8)]
        public long BoxId { get; set; }

        [DataMember(Order = 9)]
        public DateTime CreatedAt { get; set; }

        [DataMember(Order = 10)]
        public DateTime RegisterDate { get; set; }

        [DataMember(Order = 11)]
        public string CustomerId { get; set; }

        [DataMember(Order = 12)]
        public string UniqueId { get; set; }

        [DataMember(Order = 13)]
        public string Country { get; set; }

        [DataMember(Order = 14)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Email { get; set; }

        [DataMember(Order = 15)]
        public ApproveResult Approved { get; set; }

        [DataMember(Order = 16)]
        public DateTime ConversionDate { get; set; }

        [DataMember(Order = 17)]
        public string BrandStatus { get; set; }

        [DataMember(Order = 18)]
        public ResultCode Status { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }

    }
}