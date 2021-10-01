using System;
using MarketingBox.Integration.Service.Grpc.Models.Leads;
using MarketingBox.Integration.Service.Grpc.Models.Leads.Contracts;
using MarketingBox.Registration.Postgres.Entities.Lead;
using MarketingBox.Registration.Service.Domain.Lead;
using MarketingBox.Registration.Service.Grpc.Models.Leads;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Requests;
using MarketingBox.Registration.Service.Services;

namespace MarketingBox.Registration.Service.Extensions
{
    public static class MapperExtensions
    {
        public static bool IsSuccess(this string status)
        {
            return status.Equals("successful", StringComparison.OrdinalIgnoreCase);
        }

        public static LeadEntity CreateLeadEntity(
            this LeadCreateRequest request, 
            string tenantId, string brandName, long campaignId)
        {
            return  new LeadEntity()
            {
                TenantId = tenantId,
                UniqueId = LeadService.UniqueIdGenerator.GetNextId(),
                CreatedAt = request.GeneralInfo.CreatedAt,
                FirstName = request.GeneralInfo.FirstName,
                LastName = request.GeneralInfo.LastName,
                Email = request.GeneralInfo.Email,
                Ip = request.GeneralInfo.Ip,
                Password = request.GeneralInfo.Password,
                Phone = request.GeneralInfo.Phone,
                Status = LeadStatus.New,
                Type = LeadType.Unsigned,
                Sequence = 0,
                BrandInfo = new Postgres.Entities.Lead.LeadBrandInfo()
                {

                    AffiliateId = request.AuthInfo.AffiliateId,
                    BoxId = request.AuthInfo.BoxId,
                    Brand = brandName,
                    CampaignId = campaignId
                },
                AdditionalInfo = new Postgres.Entities.Lead.LeadAdditionalInfo()
                {
                    So = request.AdditionalInfo.So,
                    Sub = request.AdditionalInfo.Sub,
                    Sub1 = request.AdditionalInfo.Sub1,
                    Sub2 = request.AdditionalInfo.Sub2,
                    Sub3 = request.AdditionalInfo.Sub3,
                    Sub4 = request.AdditionalInfo.Sub4,
                    Sub5 = request.AdditionalInfo.Sub5,
                    Sub6 = request.AdditionalInfo.Sub6,
                    Sub7 = request.AdditionalInfo.Sub7,
                    Sub8 = request.AdditionalInfo.Sub8,
                    Sub9 = request.AdditionalInfo.Sub9,
                    Sub10 = request.AdditionalInfo.Sub10,
                }
            };
        }

        public static RegistrationLeadRequest CreateIntegrationRequest(
            this LeadEntity leadEntity, long brandId)
        {
            return new RegistrationLeadRequest()
            {
                TenantId = leadEntity.TenantId,
                LeadId = leadEntity.LeadId,
                LeadUniqueId = leadEntity.UniqueId,
                BrandName = leadEntity.BrandInfo.Brand,
                BrandId = brandId,//leadEntity.BrandInfo.BrandId,
                Info = new RegistrationLeadInfo()
                {
                    FirstName = leadEntity.FirstName,
                    LastName = leadEntity.LastName,
                    Email = leadEntity.Email,
                    Ip = leadEntity.Ip,
                    Phone = leadEntity.Phone,
                    Password = leadEntity.Password,
                    //TODO: Add country
                    Country = "PL",
                    Language = "EN"
                },
                AdditionalInfo = new RegistrationLeadAdditionalInfo()
                {
                    So = string.IsNullOrEmpty(leadEntity.AdditionalInfo.So) ? string.Empty : leadEntity.AdditionalInfo.So,
                    Sub = string.IsNullOrEmpty(leadEntity.AdditionalInfo.Sub) ? string.Empty : leadEntity.AdditionalInfo.Sub,
                    Sub1 = string.IsNullOrEmpty(leadEntity.AdditionalInfo.Sub1) ? string.Empty : leadEntity.AdditionalInfo.Sub1,
                    Sub2 = string.IsNullOrEmpty(leadEntity.AdditionalInfo.Sub2) ? string.Empty : leadEntity.AdditionalInfo.Sub2,
                    Sub3 = string.IsNullOrEmpty(leadEntity.AdditionalInfo.Sub3) ? string.Empty : leadEntity.AdditionalInfo.Sub3,
                    Sub4 = string.IsNullOrEmpty(leadEntity.AdditionalInfo.Sub4) ? string.Empty : leadEntity.AdditionalInfo.Sub4,
                    Sub5 = string.IsNullOrEmpty(leadEntity.AdditionalInfo.Sub5) ? string.Empty : leadEntity.AdditionalInfo.Sub5,
                    Sub6 = string.IsNullOrEmpty(leadEntity.AdditionalInfo.Sub6) ? string.Empty : leadEntity.AdditionalInfo.Sub6,
                    Sub7 = string.IsNullOrEmpty(leadEntity.AdditionalInfo.Sub7) ? string.Empty : leadEntity.AdditionalInfo.Sub7,
                    Sub8 = string.IsNullOrEmpty(leadEntity.AdditionalInfo.Sub8) ? string.Empty : leadEntity.AdditionalInfo.Sub8,
                    Sub9 = string.IsNullOrEmpty(leadEntity.AdditionalInfo.Sub9) ? string.Empty : leadEntity.AdditionalInfo.Sub9,
                    Sub10 = string.IsNullOrEmpty(leadEntity.AdditionalInfo.Sub10) ? string.Empty : leadEntity.AdditionalInfo.Sub10,
                },
            };
        }
    }
}
