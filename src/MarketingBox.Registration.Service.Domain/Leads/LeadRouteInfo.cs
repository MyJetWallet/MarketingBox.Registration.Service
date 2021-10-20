using System;

namespace MarketingBox.Registration.Service.Domain.Leads
{
    public class LeadRouteInfo
    {
        public long AffiliateId { get; set; }
        public long CampaignId { get; set; }
        public long BoxId { get; set; }
        public long BrandId { get; set; }
        public string Brand { get; set; }
        public LeadStatus Status { get; set; }
        public string CrmStatus { get; set; }
        public DateTimeOffset? DepositDate { get; set; }
        public DateTimeOffset? ConversionDate { get; set; }
        public LeadApprovedType ApprovedType { get; set; }
        public LeadCustomerInfo CustomerInfo { get; set; }
    }
}