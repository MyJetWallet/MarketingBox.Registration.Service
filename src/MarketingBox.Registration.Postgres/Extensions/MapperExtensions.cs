using MarketingBox.Registration.Postgres.Entities.Lead;
using MarketingBox.Registration.Service.Domain.Leads;

namespace MarketingBox.Registration.Postgres.Extensions
{
    public static class MapperExtensions
    {
        public static LeadEntity CreateLeadEntity(
            this Lead lead)
        {
            return  new LeadEntity()
            {
                TenantId = lead.TenantId,
                UniqueId = lead.LeadInfo.UniqueId,
                CreatedAt = lead.LeadInfo.CreatedAt,
                FirstName = lead.LeadInfo.FirstName,
                LastName = lead.LeadInfo.LastName,
                Email = lead.LeadInfo.Email,
                Ip = lead.LeadInfo.Ip,
                Password = lead.LeadInfo.Password,
                Phone = lead.LeadInfo.Phone,
                Status = lead.LeadInfo.Status,
                Country = lead.LeadInfo.Country,
                LeadId = lead.LeadInfo.LeadId,
                DepositDate = lead.LeadInfo.DepositDate,
                UpdatedAt = lead.LeadInfo.UpdatedAt,
                ConversionDate = lead.LeadInfo.ConversionDate,
                CrmStatus = lead.LeadInfo.CrmStatus,
                RouteInfoAffiliateId = lead.RouteInfo.AffiliateId,
                RouteInfoBoxId = lead.RouteInfo.BoxId,
                RouteInfoBrand = lead.RouteInfo.Brand,
                RouteInfoCampaignId = lead.RouteInfo.CampaignId,
                RouteInfoBrandId = lead.RouteInfo.BrandId,
                AdditionalInfoSo = lead.AdditionalInfo?.So,
                AdditionalInfoSub = lead.AdditionalInfo?.Sub,
                AdditionalInfoSub1 = lead.AdditionalInfo?.Sub1,
                AdditionalInfoSub2 = lead.AdditionalInfo?.Sub2,
                AdditionalInfoSub3 = lead.AdditionalInfo?.Sub3,
                AdditionalInfoSub4 = lead.AdditionalInfo?.Sub4,
                AdditionalInfoSub5 = lead.AdditionalInfo?.Sub5,
                AdditionalInfoSub6 = lead.AdditionalInfo?.Sub6,
                AdditionalInfoSub7 = lead.AdditionalInfo?.Sub7,
                AdditionalInfoSub8 = lead.AdditionalInfo?.Sub8,
                AdditionalInfoSub9 = lead.AdditionalInfo?.Sub9,
                AdditionalInfoSub10 = lead.AdditionalInfo?.Sub10,
                CustomerInfoCustomerId = lead.CustomerInfo?.CustomerId,
                CustomerInfoLoginUrl = lead.CustomerInfo?.LoginUrl,
                CustomerInfoToken = lead.CustomerInfo?.Token,
                Sequence = lead.Sequence,
            };
        }
    }
}
