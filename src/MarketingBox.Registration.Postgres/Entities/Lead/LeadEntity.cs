using System;
using MarketingBox.Registration.Service.Domain.Leads;

namespace MarketingBox.Registration.Postgres.Entities.Lead
{
    public class LeadEntity
    {
        public string TenantId { get; set; }
        public string UniqueId { get; set; }
        public long LeadId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Ip { get; set; }
        public string Country { get; set; }
        public long RouteInfoAffiliateId { get; set; }
        public long RouteInfoCampaignId { get; set; }
        public long RouteInfoBoxId { get; set; }
        public string RouteInfoBrand { get; set; }
        public long RouteInfoBrandId { get; set; }
        public string AdditionalInfoSo { get; set; }
        public string AdditionalInfoSub { get; set; }
        public string AdditionalInfoSub1 { get; set; }
        public string AdditionalInfoSub2 { get; set; }
        public string AdditionalInfoSub3 { get; set; }
        public string AdditionalInfoSub4 { get; set; }
        public string AdditionalInfoSub5 { get; set; }
        public string AdditionalInfoSub6 { get; set; }
        public string AdditionalInfoSub7 { get; set; }
        public string AdditionalInfoSub8 { get; set; }
        public string AdditionalInfoSub9 { get; set; }
        public string AdditionalInfoSub10 { get; set; }
        public string CustomerInfoCustomerId { get; set; }
        public string CustomerInfoToken { get; set; }
        public string CustomerInfoLoginUrl { get; set; }
        public LeadStatus Status { get; set; }
        public LeadCrmStatus CrmStatus { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? DepositDate { get; set; }
        public DateTimeOffset? ConversionDate { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public long Sequence { get; set; }
    }
}
