using MarketingBox.Integration.Service.Grpc.Models.Leads;
using MarketingBox.Integration.Service.Grpc.Models.Leads.Contracts;
using MarketingBox.Registration.Service.Domain.Leads;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Contracts;

namespace MarketingBox.Registration.Service.Extensions
{
    public static class MapperExtensions
    {

        public static string GeneratorId(this LeadCreateRequest request)
        {
            return request.GeneralInfo.Email + "_" +
                   request.GeneralInfo.FirstName + "_" +
                   request.GeneralInfo.LastName + "_" +
                   request.GeneralInfo.Ip + "_";
        }
        public static RegistrationRequest CreateIntegrationRequest(
            this Lead lead)
        {
            return new RegistrationRequest()
            {
                TenantId = lead.TenantId,
                LeadId = lead.LeadId,
                LeadUniqueId = lead.UniqueId,
                BrandName = lead.RouteInfo.Brand,
                BrandId = lead.RouteInfo.BrandId,
                Info = new RegistrationLeadInfo()
                {
                    FirstName = lead.FirstName,
                    LastName = lead.LastName,
                    Email = lead.Email,
                    Ip = lead.Ip,
                    Phone = lead.Phone,
                    Password = lead.Password,
                    Country = lead.Country,
                },
                AdditionalInfo = new RegistrationLeadAdditionalInfo()
                {
                    So = lead.AdditionalInfo?.So,
                    Sub = lead.AdditionalInfo?.Sub,
                    Sub1 = lead.AdditionalInfo?.Sub1,
                    Sub2 = lead.AdditionalInfo?.Sub2,
                    Sub3 = lead.AdditionalInfo?.Sub3,
                    Sub4 = lead.AdditionalInfo?.Sub4,
                    Sub5 = lead.AdditionalInfo?.Sub5,
                    Sub6 = lead.AdditionalInfo?.Sub6,
                    Sub7 = lead.AdditionalInfo?.Sub7,
                    Sub8 = lead.AdditionalInfo?.Sub8,
                    Sub9 = lead.AdditionalInfo?.Sub9,
                    Sub10 = lead.AdditionalInfo?.Sub10,
                },
            };
        }
    }
}
