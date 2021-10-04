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
using MarketingBox.Registration.Postgres.Entities.Deposit;
using MarketingBox.Registration.Postgres.Entities.Lead;
using MarketingBox.Registration.Service.Domain.Extensions;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Contracts;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Requests;
using MarketingBox.Registration.Service.Messages.Leads;
using LeadAdditionalInfoMessage = MarketingBox.Registration.Service.Messages.Leads.LeadAdditionalInfo;
using LeadBrandRegistrationInfoMessage = MarketingBox.Registration.Service.Messages.Leads.LeadBrandRegistrationInfo;
using LeadGeneralInfoMessage = MarketingBox.Registration.Service.Messages.Leads.LeadGeneralInfo;
using LeadRouteInfoMessage = MarketingBox.Registration.Service.Messages.Leads.LeadRouteInfo;

using LeadAdditionalInfoDb = MarketingBox.Registration.Postgres.Entities.Lead.LeadAdditionalInfo;
using LeadEntityDb = MarketingBox.Registration.Postgres.Entities.Lead.LeadEntity;
using LeadStatusDb = MarketingBox.Registration.Postgres.Entities.Lead.LeadStatus;
using LeadTypeDb = MarketingBox.Registration.Postgres.Entities.Lead.LeadType;
using MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts;
using MarketingBox.Registration.Service.Messages.Deposits;

namespace MarketingBox.Registration.Service.Services
{
    public class DepositService : IDepositService
    {
        private readonly ILogger<DepositService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IPublisher<DepositUpdateMessage> _publisherDepositUpdated;
        //private readonly IMyNoSqlServerDataWriter<LeadNoSql> _myNoSqlServerDataWriter;
        private readonly IMyNoSqlServerDataReader<BoxIndexNoSql> _boxIndexNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<BrandNoSql> _brandNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<BoxNoSql> _boxNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<CampaignNoSql> _campaignNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<CampaignBoxNoSql> _campaignBoxNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<PartnerNoSql> _partnerNoSqlServerDataReader;
        private readonly IIntegrationService _integrationService;

        public DepositService(ILogger<DepositService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IPublisher<DepositUpdateMessage> publisherDepositUpdated,
            //IMyNoSqlServerDataWriter<LeadNoSql> myNoSqlServerDataWriter,
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
            //_myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherDepositUpdated = publisherDepositUpdated;
            _boxIndexNoSqlServerDataReader = boxIndexNoSqlServerDataReader;
            _brandNoSqlServerDataReader = brandNoSqlServerDataReader;
            _boxNoSqlServerDataReader = boxNoSqlServerDataReader;
            _campaignNoSqlServerDataReader = campaignNoSqlServerDataReader;
            _campaignBoxNoSqlServerDataReader = campaignBoxNoSqlServerDataReader;
            _partnerNoSqlServerDataReader = partnerNoSqlServerDataReader; 
            _integrationService = integrationService;
    }

        public async Task<DepositCreateResponse> CreateDepositAsync(DepositCreateRequest request)
        {
            _logger.LogInformation("Creating new deposit {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);


            try
            {
                var existingLeadEntity = await ctx.Leads.FirstOrDefaultAsync(x => x.TenantId == request.TenantId &&
                    x.BrandRegistrationInfo.CustomerId == request.CustomerId);

                if (existingLeadEntity == null)
                {
                    return new DepositCreateResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
                }


                await _publisherDepositUpdated.PublishAsync(MapToMessage(existingLeadEntity, request));
                _logger.LogInformation("Sent lead created to service bus {@context}", request);
                
                var depositEntity = new DepositEntity()
                {
                    TenantId = existingLeadEntity.TenantId,
                    BrandId = existingLeadEntity.BrandRegistrationInfo.BrandId,
                    BoxId = existingLeadEntity.BrandRegistrationInfo.BoxId,
                    CampaignId = existingLeadEntity.BrandRegistrationInfo.CampaignId,
                    CustomerId = request.CustomerId,
                    Sequence = existingLeadEntity.Sequence,
                    AffiliateId = existingLeadEntity.BrandRegistrationInfo.AffiliateId,
                    CreatedAt = request.CreatedAt
                };
                await ctx.Deposits.AddAsync(depositEntity);
                await ctx.SaveChangesAsync();

                return MapToGrpc(depositEntity, request);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating lead {@context}", request);

                return new DepositCreateResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        private bool IsPartnerRequestInvalid(string requestApiKey, string apiKey)
        {
            return !apiKey.Equals(requestApiKey, StringComparison.OrdinalIgnoreCase);
        }

        private static DepositCreateResponse MapToGrpc(DepositEntity depositEntity, 
            DepositCreateRequest brandRequest)
        {
            return new DepositCreateResponse() 
            {
                Status = true,
                DepositId = depositEntity.DepositId,
                Message = brandRequest.ToString(),
                Error = null,
            };
        }

        public static DepositUpdateMessage MapToMessage(LeadEntity leadEntity, DepositCreateRequest brandInfo)
        {
            return new DepositUpdateMessage()
            {
                TenantId = leadEntity.TenantId,
                BrandId = leadEntity.BrandRegistrationInfo.BrandId,
                BrandName = leadEntity.BrandRegistrationInfo.Brand,
                AffiliateId = leadEntity.BrandRegistrationInfo.AffiliateId,
                BoxId = leadEntity.BrandRegistrationInfo.BoxId,
                CampaignId = leadEntity.BrandRegistrationInfo.CampaignId,
                Sequence = leadEntity.Sequence,
                LeadId = leadEntity.LeadId
            };
        }
    }
}
