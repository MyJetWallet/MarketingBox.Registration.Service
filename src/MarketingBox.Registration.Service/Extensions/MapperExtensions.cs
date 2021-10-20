using MarketingBox.Integration.Service.Grpc.Models.Leads;
using MarketingBox.Integration.Service.Grpc.Models.Leads.Contracts;
using MarketingBox.Registration.Service.Domain.Extensions;
using MarketingBox.Registration.Service.Domain.Leads;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Contracts;
using MarketingBox.Registration.Service.Messages.Leads;
using MarketingBox.Registration.Service.MyNoSql.Leads;
using LeadAdditionalInfo = MarketingBox.Registration.Service.Messages.Leads.LeadAdditionalInfo;
using LeadGeneralInfo = MarketingBox.Registration.Service.Messages.Leads.LeadGeneralInfo;
using LeadRouteInfo = MarketingBox.Registration.Service.Messages.Leads.LeadRouteInfo;

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
                LeadId = lead.LeadInfo.LeadId,
                LeadUniqueId = lead.LeadInfo.UniqueId,
                BrandName = lead.RouteInfo.Brand,
                BrandId = lead.RouteInfo.BrandId,
                Info = new RegistrationLeadInfo()
                {
                    FirstName = lead.LeadInfo.FirstName,
                    LastName = lead.LeadInfo.LastName,
                    Email = lead.LeadInfo.Email,
                    Ip = lead.LeadInfo.Ip,
                    Phone = lead.LeadInfo.Phone,
                    Password = lead.LeadInfo.Password,
                    Country = lead.LeadInfo.Country,
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

        public static LeadUpdateMessage MapToMessage(this Lead lead)
        {
            return new LeadUpdateMessage()
            {
                TenantId = lead.TenantId,
                Sequence = lead.Sequence,
                GeneralInfo = new LeadGeneralInfo()
                {
                    Email = lead.LeadInfo.Email,
                    FirstName = lead.LeadInfo.FirstName,
                    LastName = lead.LeadInfo.LastName,
                    Phone = lead.LeadInfo.Phone,
                    Ip = lead.LeadInfo.Ip,
                    Password = lead.LeadInfo.Password,
                    CreatedAt = lead.LeadInfo.CreatedAt.UtcDateTime,
                    LeadId = lead.LeadInfo.LeadId,
                    UniqueId = lead.LeadInfo.UniqueId,
                    CrmCrmStatus = lead.LeadInfo.CrmStatus.MapEnum<Messages.Common.LeadCrmStatus>(),
                    Status = lead.LeadInfo.Status.MapEnum<Messages.Common.LeadStatus>(),
                    Country = lead.LeadInfo.Country,
                    ConversionDate = lead.LeadInfo.ConversionDate?.UtcDateTime,
                    DepositDate = lead.LeadInfo.DepositDate?.UtcDateTime,
                    UpdatedAt = lead.LeadInfo.UpdatedAt.UtcDateTime
                },
                AdditionalInfo = new LeadAdditionalInfo()
                {
                    So = lead.AdditionalInfo.So,
                    Sub = lead.AdditionalInfo.Sub,
                    Sub1 = lead.AdditionalInfo.Sub1,
                    Sub2 = lead.AdditionalInfo.Sub2,
                    Sub3 = lead.AdditionalInfo.Sub3,
                    Sub4 = lead.AdditionalInfo.Sub4,
                    Sub5 = lead.AdditionalInfo.Sub5,
                    Sub6 = lead.AdditionalInfo.Sub6,
                    Sub7 = lead.AdditionalInfo.Sub7,
                    Sub8 = lead.AdditionalInfo.Sub8,
                    Sub9 = lead.AdditionalInfo.Sub9,
                    Sub10 = lead.AdditionalInfo.Sub10,
                },
                RouteInfo = new LeadRouteInfo()
                {
                    AffiliateId = lead.RouteInfo.AffiliateId,
                    BoxId = lead.RouteInfo.BoxId,
                    Brand = lead.RouteInfo.Brand,
                    CampaignId = lead.RouteInfo.CampaignId,
                    BrandId = lead.RouteInfo.BrandId
                },
                CustomerInfo = new Messages.Leads.LeadCustomerInfo()
                {
                    CustomerId = lead.CustomerInfo?.CustomerId,
                    LoginUrl = lead.CustomerInfo?.LoginUrl,
                    Token = lead.CustomerInfo?.Token,
                },
            };
        }

        public static LeadNoSqlEntity MapToNoSql(this Lead lead)
        {
            return LeadNoSqlEntity.Create(
                new MyNoSql.Leads.LeadNoSqlInfo()
                {
                    TenantId = lead.TenantId,
                    GeneralInfo = new MyNoSql.Leads.LeadGeneralInfo()
                    {
                        Status = lead.LeadInfo.Status.MapEnum<MarketingBox.Registration.Service.MyNoSql.Leads.LeadStatus>(),
                        LeadId = lead.LeadInfo.LeadId,
                        CreatedAt = lead.LeadInfo.CreatedAt.UtcDateTime,
                        Email = lead.LeadInfo.Email,
                        DepositDate = lead.LeadInfo.DepositDate?.UtcDateTime,
                        UpdatedAt = lead.LeadInfo.UpdatedAt.UtcDateTime,
                        Country = lead.LeadInfo.Country,
                        Ip = lead.LeadInfo.Ip,
                        ConversionDate = lead.LeadInfo.ConversionDate?.UtcDateTime,
                        CrmCrmStatus = lead.LeadInfo.Status.MapEnum<MarketingBox.Registration.Service.MyNoSql.Leads.LeadCrmStatus>(),
                        FirstName = lead.LeadInfo.FirstName,
                        LastName = lead.LeadInfo.LastName,
                        Password = lead.LeadInfo.Password,
                        Phone = lead.LeadInfo.Phone,
                        UniqueId = lead.LeadInfo.UniqueId

                    },
                    AdditionalInfo = new MyNoSql.Leads.LeadAdditionalInfo()
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
                    RouteInfo = new MyNoSql.Leads.LeadRouteInfo()
                    {
                        AffiliateId = lead.RouteInfo.AffiliateId,
                        BoxId = lead.RouteInfo.BoxId,
                        Brand = lead.RouteInfo.Brand,
                        CampaignId = lead.RouteInfo.CampaignId,
                        BrandId = lead.RouteInfo.BrandId
                    },
                    CustomerInfo = new MyNoSql.Leads.LeadCustomerInfo()
                    {
                        CustomerId = lead.CustomerInfo?.CustomerId,
                        Token = lead.CustomerInfo?.Token,
                        LoginUrl = lead.CustomerInfo?.LoginUrl,
                        Brand = lead.CustomerInfo?.Brand
                    }
                });
        }
    }
}
