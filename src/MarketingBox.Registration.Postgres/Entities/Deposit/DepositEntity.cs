using System;

namespace MarketingBox.Registration.Postgres.Entities.Deposit
{
    public class DepositEntity
    {
        public string TenantId { get; set; }
        public long DepositId { get; set; }
        public long BrandId { get; set; }
        public long AffiliateId { get; set; }
        public long CampaignId { get; set; }
        public long BoxId { get; set; }
        public long LeadId { get; set; }
        public string CustomerId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public long Sequence { get; set; }
        public ApprovedType Approved { get; set; }
        public DateTime? ConvertionDate { get; set; }
    }
}
