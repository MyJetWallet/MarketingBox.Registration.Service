using MarketingBox.Registration.Postgres;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.MyNoSql.Leads;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.MyNoSql.Boxes;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignBoxes;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Partners;
using MarketingBox.Integration.Service.Grpc;
using MarketingBox.Registration.Postgres.Entities.Lead;
using MarketingBox.Registration.Service.Domain.Extensions;
using MarketingBox.Registration.Service.Domain.Leads;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Contracts;
using MarketingBox.Registration.Service.Messages.Leads;
using MyJetWallet.Sdk.ServiceBus;
using Z.EntityFramework.Plus;
using LeadAdditionalInfoMessage = MarketingBox.Registration.Service.Messages.Leads.LeadAdditionalInfo;
using LeadCustomerInfo = MarketingBox.Registration.Service.Grpc.Models.Leads.LeadCustomerInfo;
using LeadGeneralInfoMessage = MarketingBox.Registration.Service.Messages.Leads.LeadGeneralInfo;
using LeadRouteInfoMessage = MarketingBox.Registration.Service.Messages.Leads.LeadRouteInfo;
using LeadGeneralInfo = MarketingBox.Registration.Service.Grpc.Models.Leads.LeadGeneralInfo;
using LeadStatus = MarketingBox.Registration.Service.Messages.Common.LeadStatus;


namespace MarketingBox.Registration.Service.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILogger<LeadService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<LeadUpdateMessage> _publisherLeadUpdated;
        private readonly IMyNoSqlServerDataWriter<LeadNoSqlEntity> _myNoSqlServerDataWriter;
        private readonly IMyNoSqlServerDataReader<BoxIndexNoSql> _boxIndexNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<BrandNoSql> _brandNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<BoxNoSql> _boxNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<CampaignNoSql> _campaignNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<CampaignBoxNoSql> _campaignBoxNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<PartnerNoSql> _partnerNoSqlServerDataReader;
        private readonly IIntegrationService _integrationService;
        private readonly ILeadRepository _repository;

        public LeadService(ILogger<LeadService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IServiceBusPublisher<LeadUpdateMessage> publisherLeadUpdated,
            IMyNoSqlServerDataWriter<LeadNoSqlEntity> myNoSqlServerDataWriter,
            IMyNoSqlServerDataReader<BoxIndexNoSql> boxIndexNoSqlServerDataReader,
            IMyNoSqlServerDataReader<BrandNoSql> brandNoSqlServerDataReader,
            IMyNoSqlServerDataReader<BoxNoSql> boxNoSqlServerDataReader,
            IMyNoSqlServerDataReader<CampaignNoSql> campaignNoSqlServerDataReader,
            IMyNoSqlServerDataReader<CampaignBoxNoSql> campaignBoxNoSqlServerDataReader,
            IMyNoSqlServerDataReader<PartnerNoSql> partnerNoSqlServerDataReader,
            IIntegrationService integrationService, ILeadRepository repository)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherLeadUpdated = publisherLeadUpdated;
            _boxIndexNoSqlServerDataReader = boxIndexNoSqlServerDataReader;
            _brandNoSqlServerDataReader = brandNoSqlServerDataReader;
            _boxNoSqlServerDataReader = boxNoSqlServerDataReader;
            _campaignNoSqlServerDataReader = campaignNoSqlServerDataReader;
            _campaignBoxNoSqlServerDataReader = campaignBoxNoSqlServerDataReader;
            _partnerNoSqlServerDataReader = partnerNoSqlServerDataReader; 
            _integrationService = integrationService;
            _repository = repository;
        }

        public async Task<LeadCreateResponse> CreateAsync(LeadCreateRequest request)
        {
            _logger.LogInformation("Creating new Lead {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            if (!TryGetPartnerInfo(request, out var tenantId, out var brandName, 
                out var campaignId, out var apiKey, out var brandId))
            {
                return RegisterFailedMapToGrpc(request.GeneralInfo);
            }

            if (IsPartnerRequestInvalid(apiKey, request.AuthInfo.ApiKey))
            {
                return InvalidFailedMapToGrpc(request.GeneralInfo);
            }

            var leadId = await _repository.GetLeadIdAsync(request.TenantId, request.GeneratorId());
            var leadBrandRegistrationInfo = new Domain.Leads.LeadRouteInfo()
            {
                BrandId = brandId,
                CampaignId = campaignId,
                Brand = brandName,
                BoxId = request.AuthInfo.BoxId,
                AffiliateId = request.AuthInfo.AffiliateId
            };
            var leadAdditionalInfo = new Domain.Leads.LeadAdditionalInfo()
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

            };
            try
            {
                var lead = Lead.Create(tenantId, UniqueIdGenerator.GetNextId(), leadId,
                request.GeneralInfo.FirstName, request.GeneralInfo.LastName, request.GeneralInfo.Password,
                request.GeneralInfo.Email, request.GeneralInfo.Phone, request.GeneralInfo.Ip,
                request.GeneralInfo.Country,
                leadBrandRegistrationInfo, leadAdditionalInfo, null);

                await _repository.SaveAsync(lead);

                await _publisherLeadUpdated.PublishAsync(MapToMessage(lead));
                _logger.LogInformation("Sent original created to service bus {@context}", request);

                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(MapToNoSql(lead));
                _logger.LogInformation("Sent partner update to MyNoSql {@context}", request);

                var brandResponse = await BrandRegisterAsync(lead);

                if (brandResponse.Status == ResultCode.CompletedSuccessfully)
                {
                    lead.Register(new Domain.Leads.LeadCustomerInfo()
                    {
                        CustomerId = brandResponse.Data.CustomerId,
                        LoginUrl = brandResponse.Data.LoginUrl,
                        Token = brandResponse.Data.Token,
                        Brand = brandResponse.Data.Brand
                    });
                }

                await _repository.SaveAsync(lead);

                await _publisherLeadUpdated.PublishAsync(MapToMessage(lead));
                _logger.LogInformation("Sent original created to service bus {@context}", request);

                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(MapToNoSql(lead));
                _logger.LogInformation("Sent original update to MyNoSql {@context}", request);


                return brandResponse.Status == ResultCode.CompletedSuccessfully ? 
                    SuccessfullMapToGrpc(lead) : FailedMapToGrpc(new Error()
                    {
                        Message = "Can't register on brand",
                        Type = ErrorType.InvalidPersonalData
                    },
                    request.GeneralInfo); ;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating original {@context}", request);

                return new LeadCreateResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        private bool TryGetPartnerInfo(LeadCreateRequest leadCreateRequest, out string outTenantId,
            out string outBrandName, out long outCampaignId, out string outPartnerApiKey, out long outBrandId)
        {
            string tenantId = string.Empty;
            string brandName = string.Empty;
            long campaignId = 0;
            long brandId = 0;
            string partnerApiKey = string.Empty;
            bool retValue = true;

            try
            {
                var boxIndexNoSql = _boxIndexNoSqlServerDataReader
                    .Get(BoxIndexNoSql.GeneratePartitionKey(leadCreateRequest.AuthInfo.BoxId)).FirstOrDefault();
                tenantId = boxIndexNoSql?.TenantId;

                var campaignBox = _campaignBoxNoSqlServerDataReader
                    .Get(CampaignBoxNoSql.GeneratePartitionKey(leadCreateRequest.AuthInfo.BoxId)).FirstOrDefault();

                var campaignNoSql = _campaignNoSqlServerDataReader.Get(
                    CampaignNoSql.GeneratePartitionKey(boxIndexNoSql?.TenantId),
                    CampaignNoSql.GenerateRowKey(campaignBox.CampaignId));

                campaignId = campaignNoSql.Id;

                var brandNoSql = _brandNoSqlServerDataReader.Get(BrandNoSql.GeneratePartitionKey(boxIndexNoSql?.TenantId),
                    BrandNoSql.GenerateRowKey(campaignNoSql.BrandId));

                brandName = brandNoSql.Name;
                brandId = brandNoSql.BrandId;

                var partner =
                    _partnerNoSqlServerDataReader.Get(PartnerNoSql.GeneratePartitionKey(boxIndexNoSql?.TenantId),
                        PartnerNoSql.GenerateRowKey(leadCreateRequest.AuthInfo.AffiliateId));

                partnerApiKey = partner.GeneralInfo.ApiKey;
            }
            catch (Exception e)
            {
                _logger.LogWarning("Can't TryGetPartnerInfo {@Context} {@Erroe}", leadCreateRequest, e.Message);
                retValue = false;
            }

            outTenantId = tenantId;
            outBrandName = brandName;
            outCampaignId = campaignId;
            outPartnerApiKey = partnerApiKey;
            outBrandId = brandId;
            return retValue;
        }

        private bool IsPartnerRequestInvalid(string requestApiKey, string apiKey)
        {
            return !apiKey.Equals(requestApiKey, StringComparison.OrdinalIgnoreCase);
        }

        public static class UniqueIdGenerator
        {
            public static string GetNextId()
            {
                return Guid.NewGuid().ToString();
            }
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public async Task<Grpc.Models.Leads.LeadBrandInfo> BrandRegisterAsync(Lead lead)
        {
            var request = lead.CreateIntegrationRequest();
            var response = await _integrationService.RegisterLeadAsync(request);

            var brandInfo = new Grpc.Models.Leads.LeadBrandInfo()
            {
                Status = (ResultCode)response.Status,
                Data = new Grpc.Models.Leads.LeadCustomerInfo()
                {
                    LoginUrl = response.RegisteredLeadInfo?.LoginUrl,
                    CustomerId = response.RegisteredLeadInfo?.CustomerId,
                    Token = response.RegisteredLeadInfo?.Token,
                }
            };

            return brandInfo;
        }

        private static LeadCreateResponse SuccessfullMapToGrpc(Lead lead)
        {
            return new LeadCreateResponse()
            {
                Status = ResultCode.CompletedSuccessfully,
                Message = lead.CustomerInfo.LoginUrl,
                BrandInfo = new MarketingBox.Registration.Service.Grpc.Models.Leads.LeadBrandInfo()
                {
                    Status = ResultCode.CompletedSuccessfully,
                    Data = new MarketingBox.Registration.Service.Grpc.Models.Leads.LeadCustomerInfo()
                    {
                        CustomerId = lead.CustomerInfo.CustomerId,
                        LoginUrl = lead.CustomerInfo.LoginUrl,
                        Token = lead.CustomerInfo.Token,
                    },
                },
                FallbackUrl = string.Empty,
                LeadId = lead.LeadInfo.LeadId,

            };
        }

        private static LeadCreateResponse RegisterFailedMapToGrpc(
            MarketingBox.Registration.Service.Grpc.Models.Leads.LeadGeneralInfo reneralInfo)
        {
            return FailedMapToGrpc(
                new Error()
                {
                    Message = "Can't get partner info",
                    Type = ErrorType.Unknown
                },
                reneralInfo
            );
        }

        private static LeadCreateResponse InvalidFailedMapToGrpc(
            MarketingBox.Registration.Service.Grpc.Models.Leads.LeadGeneralInfo reneralInfo)
        {
            return FailedMapToGrpc(
                new Error()
                {
                    Message = "Invalid partner data",
                    Type = ErrorType.InvalidParameter
                },
                reneralInfo
            );
        }

        private static LeadCreateResponse FailedMapToGrpc(Error error, 
            MarketingBox.Registration.Service.Grpc.Models.Leads.LeadGeneralInfo original)
        {
            return new LeadCreateResponse()
            {
                Status = ResultCode.Failed,
                Error = error,
                Message = error.Message,
                OriginalData = new LeadGeneralInfo()
                {
                    Email = original.Email,
                    Password = original.Password,
                    FirstName = original.FirstName,
                    Ip = original.Ip,
                    LastName = original.LastName,
                    Phone = original.Phone,
                    CreatedAt = original.CreatedAt,
                }
            };
        }

        public static LeadUpdateMessage MapToMessage(Lead lead)
        {
            return new LeadUpdateMessage()
            {
                TenantId = lead.TenantId,
                Sequence = lead.Sequence,
                GeneralInfo = new LeadGeneralInfoMessage()
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
                AdditionalInfo = new LeadAdditionalInfoMessage()
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
                RouteInfo = new LeadRouteInfoMessage()
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

        private static LeadNoSqlEntity MapToNoSql(Lead lead)
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
