using DotNetCoreDecorators;
using MarketingBox.Registration.Postgres;
using MarketingBox.Registration.Service.Domain.Extensions;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models.Leads;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Messages;
using MarketingBox.Registration.Service.MyNoSql.Leads;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Messages.Partners;
using MarketingBox.Affiliate.Service.MyNoSql.Boxes;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignBoxes;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Partners;
using MarketingBox.Registration.Postgres.Entities.Lead;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Contracts;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Requests;
using MarketingBox.Registration.Service.Messages.Leads;
using Z.EntityFramework.Plus;
using LeadBrandInfo = MarketingBox.Registration.Postgres.Entities.Lead.LeadBrandInfo;
using LeadStatus = MarketingBox.Registration.Service.Domain.Lead.LeadStatus;
using LeadType = MarketingBox.Registration.Service.Domain.Lead.LeadType;

namespace MarketingBox.Registration.Service.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILogger<LeadService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IPublisher<LeadBusUpdateMessage> _publisherLeadUpdated;
        private readonly IMyNoSqlServerDataWriter<LeadNoSql> _myNoSqlServerDataWriter;
        private readonly IMyNoSqlServerDataReader<BoxIndexNoSql> _boxIndexNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<BrandNoSql> _brandNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<BoxNoSql> _boxNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<CampaignNoSql> _campaignNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<CampaignBoxNoSql> _campaignBoxNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<PartnerNoSql> _partnerNoSqlServerDataReader;

        public LeadService(ILogger<LeadService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IPublisher<LeadBusUpdateMessage> publisherLeadUpdated,
            IMyNoSqlServerDataWriter<LeadNoSql> myNoSqlServerDataWriter,
            IMyNoSqlServerDataReader<BoxIndexNoSql> boxIndexNoSqlServerDataReader,
            IMyNoSqlServerDataReader<BrandNoSql> brandNoSqlServerDataReader,
            IMyNoSqlServerDataReader<BoxNoSql> boxNoSqlServerDataReader,
            IMyNoSqlServerDataReader<CampaignNoSql> campaignNoSqlServerDataReader,
            IMyNoSqlServerDataReader<CampaignBoxNoSql> campaignBoxNoSqlServerDataReader,
            IMyNoSqlServerDataReader<PartnerNoSql> partnerNoSqlServerDataReader)
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
        }

        public async Task<LeadCreateResponse> CreateAsync(LeadCreateRequest request)
        {
            _logger.LogInformation("Creating new Lead {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            if (!TryGetPartnerInfo(request, out var tenantId, out var brandName, out var campaignId, out var apiKey))
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

            var leadEntity = new LeadEntity()
            {
                TenantId = tenantId,
                UniqueId = UniqueIdGenerator.GetNextId(),
                CreatedAt = request.GeneralInfo.CreatedAt,
                FirstName = request.GeneralInfo.FirstName,
                LastName = request.GeneralInfo.LastName,
                Email = request.GeneralInfo.Email,
                Ip = request.GeneralInfo.Ip,
                Password = request.GeneralInfo.Password,
                Phone = request.GeneralInfo.Phone,
                Status = LeadStatus.New,
                Type = LeadType.Lead,
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

            try
            {
                ctx.Leads.Add(leadEntity);
                await ctx.SaveChangesAsync();

                await _publisherLeadUpdated.PublishAsync(MapToMessage(leadEntity, null));
                _logger.LogInformation("Sent lead created to service bus {@context}", request);

                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(MapToNoSql(leadEntity));
                _logger.LogInformation("Sent partner update to MyNoSql {@context}", request);

                var brandInfo = await BrandRegisterAsync(leadEntity, brandName);

                leadEntity.Sequence++;

                await _publisherLeadUpdated.PublishAsync(MapToMessage(leadEntity, brandInfo));
                _logger.LogInformation("Sent lead created to service bus {@context}", request);


                return MapToGrpc(leadEntity, brandInfo);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating lead {@context}", request);

                return new LeadCreateResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        private bool TryGetPartnerInfo(LeadCreateRequest leadCreateRequest, out string outTenantId,
            out string outBrandName, out long outCampaignId, out string outPartnerApiKey)
        {
            string tenantId = string.Empty;
            string brandName = string.Empty;
            long campaignId = 0;
            string partnerApiKey = string.Empty;

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

                var partner =
                    _partnerNoSqlServerDataReader.Get(PartnerNoSql.GeneratePartitionKey(boxIndexNoSql.TenantId),
                        PartnerNoSql.GenerateRowKey(leadCreateRequest.AuthInfo.AffiliateId));

                partnerApiKey = partner.GeneralInfo.ApiKey;
            }
            catch (Exception e)
            {
                outTenantId = tenantId;
                outBrandName = brandName;
                outCampaignId = campaignId;
                outPartnerApiKey = partnerApiKey;
                return false;
            }

            outTenantId = tenantId;
            outBrandName = brandName;
            outCampaignId = campaignId;
            outPartnerApiKey = partnerApiKey;
            return true;
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

        public async Task<Grpc.Models.Leads.LeadBrandInfo> BrandRegisterAsync(LeadEntity leadEntity, string brand)
        {
            string brandLoginUrl = @"https://trading-test.handelpro.biz/lpLogin/6DB5D4818181B806DBF7B19EBDC5FD97F1B82759077317B6481BC883F071783DBEF568426B81DF43044E326C26437E097F21A2484110D13420E9EC6E44A1B2BE?lang=PL";
            string brandName = brand;
            string brandCustomerId = "02537c06cab34f62931c263bf3480959";
            string customerEmail = "yuriy.test.2020.09.22.01@mailinator.com";
            string brandToken = "6DB5D4818181B806DBF7B19EBDC5FD97F1B82759077317B6481BC883F071783DBEF568426B81DF43044E326C26437E097F21A2484110D13420E9EC6E44A1B2BE";

            var brandInfo = new Grpc.Models.Leads.LeadBrandInfo()
            {
                Status = "successful",
                Data = new LeadBrandRegistrationInfo()
                {
                    Email = customerEmail, //leadEntity.Email,
                    UniqueId = leadEntity.LeadId.ToString(),
                    LoginUrl = brandLoginUrl,
                    Broker = brandName,
                    CustomerId = brandCustomerId,
                    Token = brandToken
                }
            };
            await Task.Delay(1000);
            return brandInfo;
        }

        public async Task<LeadCreateResponse> UpdateAsync(LeadUpdateRequest request)
        {
            _logger.LogInformation("Updating a Lead {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var leadEntity = new LeadEntity()
            {
                LeadId = request.LeadId,
                TenantId = request.TenantId,
                BrandInfo = new LeadBrandInfo()
                {
                    //CreatedAt = DateTime.UtcNow,
                    //Skype = request.BrandInfo.Skype,
                    //Type = request.BrandInfo.Type.MapEnum<Domain.Lead.LeadBusType>(),
                    //Username = request.BrandInfo.Username,
                    //ZipCode = request.BrandInfo.ZipCode,
                    //Email = request.BrandInfo.Email,
                    //Password = request.BrandInfo.Password,
                    //Phone = request.BrandInfo.Phone
                },
                //Sequence = request.Sequence
            };

            try
            {
                var affectedRowsCount = await ctx.Leads
                .Where(x => x.LeadId == request.LeadId 
                            //&& x.Sequence <= request.Sequence
                        )
                .UpdateAsync(x => new LeadEntity()
                {
                    LeadId = request.LeadId,
                    TenantId = request.TenantId,
                    BrandInfo = new LeadBrandInfo()
                    {
                        //CreatedAt = DateTime.UtcNow,
                        //Skype = request.BrandInfo.Skype,
                        //Type = request.BrandInfo.Type.MapEnum<Domain.Lead.LeadBusType>(),
                        //Username = request.BrandInfo.Username,
                        //ZipCode = request.BrandInfo.ZipCode,
                        //Email = request.BrandInfo.Email,
                        //Password = request.BrandInfo.Password,
                        //Phone = request.BrandInfo.Phone
                    },
                    //Sequence = request.Sequence
                });

                if (affectedRowsCount != 1)
                {
                    throw new Exception("Update failed");
                }

                //await _publisherLeadCreated.PublishAsync(MapToMessage(partnerEntity));
                //_logger.LogInformation("Sent lead update to service bus {@context}", request);

                //var nosql = MapToNoSql(partnerEntity);
                //await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                //_logger.LogInformation("Sent lead update to MyNoSql {@context}", request);

                var brandInfo = await BrandRegisterAsync(leadEntity, "Monfex UpdateAsync");

                return MapToGrpc(leadEntity, brandInfo);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating lead {@context}", request);

                return new LeadCreateResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<LeadCreateResponse> GetAsync(LeadGetRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var leadEntity = await ctx.Leads.FirstOrDefaultAsync(x => x.LeadId == request.LeadId);
                //TODO: Fix GetAsync
                //return partnerEntity != null ? MapToGrpc(partnerEntity) : new IsPartnerRequestInvalid();
                return leadEntity != null 
                    ? MapToGrpc(leadEntity, await BrandRegisterAsync(leadEntity, "Monfex GetAsync")) 
                    : new LeadCreateResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting lead {@context}", request);

                return new LeadCreateResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<LeadCreateResponse> DeleteAsync(LeadDeleteRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {

                var partnerEntity = await ctx.Leads.FirstOrDefaultAsync(x => x.LeadId == request.LeadId);

                if (partnerEntity == null)
                    return new LeadCreateResponse();

                //await _publisherLeadUpdated.PublishAsync(new PartnerRemoved()
                //{
                //    AffiliateId = partnerEntity.LeadId,
                //    //Sequence = partnerEntity.Sequence,
                //    TenantId = partnerEntity.TenantId
                //});

                await _myNoSqlServerDataWriter.DeleteAsync(
                    LeadNoSql.GeneratePartitionKey(partnerEntity.TenantId),
                    LeadNoSql.GenerateRowKey(partnerEntity.LeadId));

                await ctx.Leads.Where(x => x.LeadId == partnerEntity.LeadId).DeleteAsync();

                return new LeadCreateResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting lead {@context}", request);

                return new LeadCreateResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
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

        private static LeadBusUpdateMessage MapToMessage(LeadEntity leadEntity, Grpc.Models.Leads.LeadBrandInfo brandInfo)
        {
            return new LeadBusUpdateMessage()
            {
                TenantId = leadEntity.TenantId,
                LeadId = leadEntity.LeadId,
                UniqueId = leadEntity.UniqueId,
                Sequence = leadEntity.Sequence,
                GeneralInfo = new LeadBusGeneralInfo()
                {
                    Email = leadEntity.Email,
                    FirstName = leadEntity.FirstName,
                    LastName = leadEntity.LastName,
                    Phone = leadEntity.Phone,
                    Ip = leadEntity.Ip,
                    Password = leadEntity.Password,
                    CreatedAt = leadEntity.CreatedAt
                },
                AdditionalInfo = new LeadBusAdditionalInfo()
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
                RouteInfo = new LeadBusRouteInfo()
                {
                    AffiliateId = leadEntity.BrandInfo.AffiliateId,
                    BoxId = leadEntity.BrandInfo.BoxId,
                    Brand = leadEntity.BrandInfo.Brand,
                    CampaignId = leadEntity.BrandInfo.CampaignId
                },
                RegistrationInfo = new LeadBusBrandRegistrationInfo()
                {
                    Broker = brandInfo.Data != null ? brandInfo.Data.Broker : string.Empty,
                    CustomerId = brandInfo.Data != null ? brandInfo.Data.CustomerId : string.Empty,
                    LoginUrl = brandInfo.Data != null ? brandInfo.Data.LoginUrl : string.Empty,
                    Token = brandInfo.Data != null ? brandInfo.Data.Token : string.Empty,
                }
            };
        }

        private static LeadNoSql MapToNoSql(LeadEntity leadEntity)
        {
            return LeadNoSql.Create(
                leadEntity.TenantId,
                leadEntity.LeadId,
                new MyNoSql.Leads.LeadNoSqlGeneralInfo()
                {
                    //CreatedAt = leadEntity.CreatedAt,
                    //Email = leadEntity.Email,
                    
                    //Username = leadEntity.FirstName + " " + leadEntity.LastName
                }
                );
        }
    }
}
