using DotNetCoreDecorators;
using MarketingBox.Registration.Postgres;
using MarketingBox.Registration.Service.Grpc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Postgres.Entities.Deposit;
using MarketingBox.Registration.Postgres.Entities.Lead;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts;
using MarketingBox.Registration.Service.Messages.Deposits;

namespace MarketingBox.Registration.Service.Services
{
    public class DepositService : IDepositService
    {
        private readonly ILogger<DepositService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IPublisher<DepositUpdateMessage> _publisherDepositUpdated;


        public DepositService(ILogger<DepositService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IPublisher<DepositUpdateMessage> publisherDepositUpdated)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisherDepositUpdated = publisherDepositUpdated;
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


                await _publisherDepositUpdated.PublishAsync(MapToMessage(existingLeadEntity));
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

        public Task<DepositApproveResponse> ApproveDepositAsync(DepositApproveRequest request)
        {
            throw new NotImplementedException();
        }

        private static DepositCreateResponse MapToGrpc(DepositEntity depositEntity, 
            DepositCreateRequest brandRequest)
        {
            return new DepositCreateResponse() 
            {
                Status = true,
                DepositId = depositEntity.DepositId,
                Message = "brandRequest.ToString()",
                Error = null,
            };
        }

        private static DepositUpdateMessage MapToMessage(LeadEntity leadEntity)
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
                LeadId = leadEntity.LeadId,
                CreatedAt = DateTime.UtcNow,
            };
        }
    }
}
