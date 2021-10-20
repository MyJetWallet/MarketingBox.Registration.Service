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
                CustomerInfoBrand = lead.CustomerInfo?.Brand,
                Sequence = lead.Sequence,
            };
        }

        public static Lead CreateLead(this LeadEntity leadEntity)
        {
            var leadBrandRegistrationInfo = new LeadRouteInfo()
            {
                BrandId = leadEntity.RouteInfoBrandId,
                CampaignId = leadEntity.RouteInfoCampaignId,
                Brand = leadEntity.CustomerInfoBrand,
                BoxId = leadEntity.RouteInfoBoxId,
                AffiliateId = leadEntity.RouteInfoAffiliateId
            };
            var leadAdditionalInfo = new LeadAdditionalInfo()
            {
                So = leadEntity.AdditionalInfoSo,
                Sub = leadEntity.AdditionalInfoSub,
                Sub1 = leadEntity.AdditionalInfoSub1,
                Sub2 = leadEntity.AdditionalInfoSub2,
                Sub3 = leadEntity.AdditionalInfoSub3,
                Sub4 = leadEntity.AdditionalInfoSub4,
                Sub5 = leadEntity.AdditionalInfoSub5,
                Sub6 = leadEntity.AdditionalInfoSub6,
                Sub7 = leadEntity.AdditionalInfoSub7,
                Sub8 = leadEntity.AdditionalInfoSub8,
                Sub9 = leadEntity.AdditionalInfoSub9,
                Sub10 = leadEntity.AdditionalInfoSub10,

            };
            var leadGeneralInfo = new LeadGeneralInfo()
            {
                UniqueId = leadEntity.UniqueId,
                LeadId = leadEntity.LeadId,
                FirstName = leadEntity.FirstName,
                LastName = leadEntity.LastName,
                Password = leadEntity.Password,
                Email = leadEntity.Email,
                Phone = leadEntity.Phone,
                Ip = leadEntity.Ip,
                Country = leadEntity.Country,
                Status = leadEntity.Status,
                CrmStatus = leadEntity.CrmStatus,
                CreatedAt = leadEntity.CreatedAt,
                ConversionDate = leadEntity.ConversionDate,
                DepositDate = leadEntity.DepositDate,
                UpdatedAt = leadEntity.UpdatedAt,
            };

            var leadCustomerInfo = new LeadCustomerInfo()
            {
                CustomerId = leadEntity.CustomerInfoCustomerId,
                Token = leadEntity.CustomerInfoToken,
                LoginUrl = leadEntity.CustomerInfoLoginUrl,
                Brand = leadEntity.CustomerInfoBrand
            };

            var lead = Lead.Create(leadEntity.TenantId, leadEntity.Sequence, leadGeneralInfo,
                leadBrandRegistrationInfo, leadAdditionalInfo, leadCustomerInfo);
            return lead;
        }
    }
}
