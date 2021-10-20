using System;

namespace MarketingBox.Registration.Service.MyNoSql.Leads
{
    public class LeadRouteInfo
    {
        public long AffiliateId { get; set; }
        public long BoxId { get; set; }
        public long CampaignId { get; set; }
        public string Brand { get; set; }
        public long BrandId { get; set; }
        public LeadStatus Status { get; set; }
        public string CrmCrmStatus { get; set; }
        public DateTime? DepositDate { get; set; }
        public DateTime? ConversionDate { get; set; }
        public LeadCustomerInfo CustomerInfo { get; set; }
        public LeadApprovedType ApprovedType { get; set; }
    }
}