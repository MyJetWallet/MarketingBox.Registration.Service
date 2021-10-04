using DotNetCoreDecorators;
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
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Leads;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Contracts;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Requests;
using MarketingBox.Registration.Service.Messages.Leads;
using Z.EntityFramework.Plus;
using LeadAdditionalInfoMessage = MarketingBox.Registration.Service.Messages.Leads.LeadAdditionalInfo;
using LeadBrandRegistrationInfoMessage = MarketingBox.Registration.Service.Messages.Leads.LeadBrandRegistrationInfo;
using LeadGeneralInfoMessage = MarketingBox.Registration.Service.Messages.Leads.LeadGeneralInfo;
using LeadRouteInfoMessage = MarketingBox.Registration.Service.Messages.Leads.LeadRouteInfo;

using LeadAdditionalInfoDb = MarketingBox.Registration.Postgres.Entities.Lead.LeadAdditionalInfo;
using LeadEntityDb = MarketingBox.Registration.Postgres.Entities.Lead.LeadEntity;
using LeadStatusDb = MarketingBox.Registration.Postgres.Entities.Lead.LeadStatus;
using LeadTypeDb = MarketingBox.Registration.Postgres.Entities.Lead.LeadType;



namespace MarketingBox.Registration.Service.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILogger<LeadService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IPublisher<LeadUpdateMessage> _publisherLeadUpdated;
        private readonly IMyNoSqlServerDataWriter<LeadNoSql> _myNoSqlServerDataWriter;
        private readonly IMyNoSqlServerDataReader<BoxIndexNoSql> _boxIndexNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<BrandNoSql> _brandNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<BoxNoSql> _boxNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<CampaignNoSql> _campaignNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<CampaignBoxNoSql> _campaignBoxNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<PartnerNoSql> _partnerNoSqlServerDataReader;
        private readonly IIntegrationService _integrationService;

        public LeadService(ILogger<LeadService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IPublisher<LeadUpdateMessage> publisherLeadUpdated,
            IMyNoSqlServerDataWriter<LeadNoSql> myNoSqlServerDataWriter,
            IMyNoSqlServerDataReader<BoxIndexNoSql> boxIndexNoSqlServerDataReader,
            IMyNoSqlServerDataReader<BrandNoSql> brandNoSqlServerDataReader,
            IMyNoSqlServerDataReader<BoxNoSql> boxNoSqlServerDataReader,
            IMyNoSqlServerDataReader<CampaignNoSql> campaignNoSqlServerDataReader,
            IMyNoSqlServerDataReader<CampaignBoxNoSql> campaignBoxNoSqlServerDataReader,
            IMyNoSqlServerDataReader<PartnerNoSql> partnerNoSqlServerDataReader,
            IIntegrationService integrationService)
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
    }

        public async Task<LeadCreateResponse> CreateAsync(LeadCreateRequest request)
        {
            _logger.LogInformation("Creating new Lead {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            if (!TryGetPartnerInfo(request, out var tenantId, out var brandName, out var campaignId, out var apiKey, out var brandId))
            {
                return LeadCreateResponse.Failed(
                    new Error()
                    {
                        Message = "Can't get partner info",
                        Type = ErrorType.Unknown
                    },
                    request.GeneralInfo
                );
            }

            if (IsPartnerRequestInvalid(apiKey, request.AuthInfo.ApiKey))
            {
                return LeadCreateResponse.Failed(
                    new Error()
                    {
                        Message = "Invalid partner data",
                        Type = ErrorType.InvalidParameter
                    },
                    request.GeneralInfo
                );
            }

            var leadEntity = new LeadEntityDb()
            {
                TenantId = tenantId,
                UniqueId = UniqueIdGenerator.GetNextId(),
                CreatedAt = DateTime.UtcNow,
                FirstName = request.GeneralInfo.FirstName,
                LastName = request.GeneralInfo.LastName,
                Email = request.GeneralInfo.Email,
                Ip = request.GeneralInfo.Ip,
                Password = request.GeneralInfo.Password,
                Phone = request.GeneralInfo.Phone,
                Status = LeadStatusDb.New,
                Type = LeadTypeDb.Unsigned,
                Sequence = 0,
                BrandRegistrationInfo = new Postgres.Entities.Lead.LeadBrandRegistrationInfo()
                {
                    AffiliateId = request.AuthInfo.AffiliateId,
                    BoxId = request.AuthInfo.BoxId,
                    Brand = brandName,
                    CampaignId = campaignId,
                    BrandId = brandId,
                    BrandResponse = string.Empty,
                    CustomerId = string.Empty
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

            try
            {
                ctx.Leads.Add(leadEntity);
                await ctx.SaveChangesAsync();

                await _publisherLeadUpdated.PublishAsync(MapToMessage(leadEntity, new Grpc.Models.Leads.LeadBrandInfo()
                {
                    Data = new Grpc.Models.Leads.LeadBrandRegistrationInfo()
                    {
                        CustomerId = string.Empty,
                        Token = string.Empty,
                        LoginUrl = string.Empty,
                    }
                }));
                _logger.LogInformation("Sent lead created to service bus {@context}", request);

                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(MapToNoSql(leadEntity));
                _logger.LogInformation("Sent partner update to MyNoSql {@context}", request);

                var brandInfo = await BrandRegisterAsync(leadEntity, brandId);
                
                leadEntity.Sequence++;
                leadEntity.Type = brandInfo.Status.IsSuccess() ?    
                    LeadTypeDb.Lead : LeadTypeDb.Failure;

                leadEntity.BrandRegistrationInfo.CustomerId = brandInfo.Data.CustomerId;
                leadEntity.BrandRegistrationInfo.BrandResponse = brandInfo.Data.ToString();

                var affectedRowsCount = await ctx.Leads
                    .Where(x => x.LeadId == leadEntity.LeadId &&
                                x.Sequence <= leadEntity.Sequence)
                    .UpdateAsync(x => new LeadEntity()
                    {
                        BrandRegistrationInfo = new MarketingBox.Registration.Postgres.Entities.Lead.LeadBrandRegistrationInfo()
                        {
                            CustomerId = brandInfo.Data.CustomerId,
                            BrandResponse = brandInfo.Data.ToString()
                        },
                        Sequence = leadEntity.Sequence,
                        Type = leadEntity.Type
                    });


                if (affectedRowsCount != 1)
                {
                    throw new Exception("Update failed");
                }

                await _publisherLeadUpdated.PublishAsync(MapToMessage(leadEntity, brandInfo));
                _logger.LogInformation("Sent lead created to service bus {@context}", request);

                var nosql = MapToNoSql(leadEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent lead update to MyNoSql {@context}", request);


                return MapToGrpc(leadEntity, brandInfo);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating lead {@context}", request);

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
                tenantId = boxIndexNoSql.TenantId;

                var campaignBox = _campaignBoxNoSqlServerDataReader
                    .Get(CampaignBoxNoSql.GeneratePartitionKey(leadCreateRequest.AuthInfo.BoxId)).FirstOrDefault();


                var campaignNoSql = _campaignNoSqlServerDataReader.Get(
                    CampaignNoSql.GeneratePartitionKey(boxIndexNoSql.TenantId),
                    CampaignNoSql.GenerateRowKey(campaignBox.CampaignId));

                campaignId = campaignNoSql.Id;

                var brandNoSql = _brandNoSqlServerDataReader.Get(BrandNoSql.GeneratePartitionKey(boxIndexNoSql.TenantId),
                    BrandNoSql.GenerateRowKey(campaignNoSql.BrandId));

                brandName = brandNoSql.Name;
                brandId = brandNoSql.BrandId;

                var partner =
                    _partnerNoSqlServerDataReader.Get(PartnerNoSql.GeneratePartitionKey(boxIndexNoSql.TenantId),
                        PartnerNoSql.GenerateRowKey(leadCreateRequest.AuthInfo.AffiliateId));

                partnerApiKey = partner.GeneralInfo.ApiKey;
            }
            catch (Exception e)
            {
                _logger.LogWarning("Can't TryGetPartnerInfo {@context}", leadCreateRequest);
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


        public async Task<Grpc.Models.Leads.LeadBrandInfo> BrandRegisterAsync(LeadEntity leadEntity, long brandId)
        {
            var request = leadEntity.CreateIntegrationRequest(brandId);
            var response = await _integrationService.RegisterLeadAsync(request);

            var brandInfo = new Grpc.Models.Leads.LeadBrandInfo()
            {
                Status = response.Status,
                Data = new Grpc.Models.Leads.LeadBrandRegistrationInfo()
                {
                    Email = leadEntity.Email,
                    UniqueId = leadEntity.LeadId.ToString(),
                    LoginUrl = response.RegistrationCustomerInfo.LoginUrl,
                    Broker = leadEntity.BrandRegistrationInfo.Brand,
                    CustomerId = response.RegistrationCustomerInfo.CustomerId,
                    Token = response.RegistrationCustomerInfo.Token,
                }
            };

            return brandInfo;
        }

        private static LeadCreateResponse MapToGrpc(LeadEntity leadEntity, 
            Grpc.Models.Leads.LeadBrandInfo brandInfo)
        {
            //TODO: Remove
            return new LeadCreateResponse() 
            {
                Status = true,
                FallbackUrl = String.Empty,
                BrandInfo = brandInfo,
                Message = brandInfo.Data.LoginUrl,
                Error = null,
                OriginalData = null,
            };
        }

        public static LeadUpdateMessage MapToMessage(LeadEntity leadEntity, Grpc.Models.Leads.LeadBrandInfo brandInfo)
        {
            return new LeadUpdateMessage()
            {
                TenantId = leadEntity.TenantId,
                LeadId = leadEntity.LeadId,
                UniqueId = leadEntity.UniqueId,
                Sequence = leadEntity.Sequence,
                GeneralInfo = new LeadGeneralInfoMessage()
                {
                    Email = leadEntity.Email,
                    FirstName = leadEntity.FirstName,
                    LastName = leadEntity.LastName,
                    Phone = leadEntity.Phone,
                    Ip = leadEntity.Ip,
                    Password = leadEntity.Password,
                    CreatedAt = leadEntity.CreatedAt.UtcDateTime
                },
                AdditionalInfo = new LeadAdditionalInfoMessage()
                {
                    So = leadEntity.AdditionalInfo.So,
                    Sub = leadEntity.AdditionalInfo.Sub,
                    Sub1 = leadEntity.AdditionalInfo.Sub1,
                    Sub2 = leadEntity.AdditionalInfo.Sub2,
                    Sub3 = leadEntity.AdditionalInfo.Sub3,
                    Sub4 = leadEntity.AdditionalInfo.Sub4,
                    Sub5 = leadEntity.AdditionalInfo.Sub5,
                    Sub6 = leadEntity.AdditionalInfo.Sub6,
                    Sub7 = leadEntity.AdditionalInfo.Sub7,
                    Sub8 = leadEntity.AdditionalInfo.Sub8,
                    Sub9 = leadEntity.AdditionalInfo.Sub9,
                    Sub10 = leadEntity.AdditionalInfo.Sub10,
                },
                RouteInfo = new LeadRouteInfoMessage()
                {
                    AffiliateId = leadEntity.BrandRegistrationInfo.AffiliateId,
                    BoxId = leadEntity.BrandRegistrationInfo.BoxId,
                    Brand = leadEntity.BrandRegistrationInfo.Brand,
                    CampaignId = leadEntity.BrandRegistrationInfo.CampaignId
                },
                RegistrationInfo = new LeadBrandRegistrationInfoMessage()
                {
                    CustomerId = brandInfo != null ? brandInfo.Data.CustomerId : string.Empty,
                    LoginUrl = brandInfo != null ? brandInfo.Data.LoginUrl : string.Empty,
                    Token = brandInfo != null ? brandInfo.Data.Token : string.Empty,
                    Brand = leadEntity.BrandRegistrationInfo.Brand,
                    BrandId = leadEntity.BrandRegistrationInfo.BrandId,
                }
            };
        }

        private static LeadNoSql MapToNoSql(LeadEntity leadEntity)
        {
            return LeadNoSql.Create(
                leadEntity.TenantId,
                leadEntity.LeadId,
                new MyNoSql.Leads.LeadGeneralInfo()
                {
                    LeadId = leadEntity.LeadId,
                    Email = leadEntity.Email,
                    FirstName = leadEntity.FirstName,
                    LastName = leadEntity.LastName,
                    Ip = leadEntity.Ip,
                    Password = leadEntity.Password,
                    Phone = leadEntity.Phone,
                    Status = leadEntity.Status.MapEnum<MarketingBox.Registration.Service.MyNoSql.Leads.LeadStatus>(),
                    TenantId = leadEntity.TenantId,
                    CreatedAt = leadEntity.CreatedAt.UtcDateTime,
                    Type = leadEntity.Type.MapEnum<MarketingBox.Registration.Service.MyNoSql.Leads.LeadType>(),
                    AdditionalInfo = new MyNoSql.Leads.LeadAdditionalInfo()
                    {
                        So = leadEntity.AdditionalInfo.So,
                        Sub = leadEntity.AdditionalInfo.Sub,
                        Sub1 = leadEntity.AdditionalInfo.Sub1,
                        Sub2 = leadEntity.AdditionalInfo.Sub2,
                        Sub3 = leadEntity.AdditionalInfo.Sub3,
                        Sub4 = leadEntity.AdditionalInfo.Sub4,
                        Sub5 = leadEntity.AdditionalInfo.Sub5,
                        Sub6 = leadEntity.AdditionalInfo.Sub6,
                        Sub7 = leadEntity.AdditionalInfo.Sub7,
                        Sub8 = leadEntity.AdditionalInfo.Sub8,
                        Sub9 = leadEntity.AdditionalInfo.Sub9,
                        Sub10 = leadEntity.AdditionalInfo.Sub10,
                    },
                    BrandInfo = new MyNoSql.Leads.LeadBrandInfo()
                    {
                        AffiliateId = leadEntity.BrandRegistrationInfo.AffiliateId,
                        BoxId = leadEntity.BrandRegistrationInfo.BoxId,
                        Brand = leadEntity.BrandRegistrationInfo.Brand,
                        CampaignId = leadEntity.BrandRegistrationInfo.CampaignId
                    }
                }
                );
        }
    }
}
