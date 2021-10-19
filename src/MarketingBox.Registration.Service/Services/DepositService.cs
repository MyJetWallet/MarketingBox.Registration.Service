using DotNetCoreDecorators;
using MarketingBox.Registration.Postgres;
using MarketingBox.Registration.Service.Grpc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Postgres.Entities.Lead;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts;
using MarketingBox.Registration.Service.Messages.Deposits;
using MyJetWallet.Sdk.ServiceBus;

namespace MarketingBox.Registration.Service.Services
{
    public class DepositService : IDepositService
    {
        private readonly ILogger<DepositService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<DepositUpdateMessage> _publisherDepositUpdated;


        public DepositService(ILogger<DepositService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IServiceBusPublisher<DepositUpdateMessage> publisherDepositUpdated)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisherDepositUpdated = publisherDepositUpdated;
        }

        public async Task<DepositCreateResponse> CreateDepositAsync(DepositCreateRequest request)
        {
            _logger.LogInformation("Creating new deposit {@context}", request);
            throw new Exception($"Not implemented");
            //using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);


            //try
            //{
            //    var existingLeadEntity = await ctx.Leads.FirstOrDefaultAsync(x => x.TenantId == request.TenantId &&
            //        x.BrandRegistrationInfo.CustomerId == request.CustomerId);

            //    if (existingLeadEntity == null)
            //    {
            //        return new DepositCreateResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            //    }

            //    var depositEntity = new DepositEntity()
            //    {
            //        TenantId = existingLeadEntity.TenantId,
            //        BrandId = existingLeadEntity.BrandRegistrationInfo.BrandId,
            //        BoxId = existingLeadEntity.BrandRegistrationInfo.BoxId,
            //        CampaignId = existingLeadEntity.BrandRegistrationInfo.CampaignId,
            //        CustomerId = request.CustomerId,
            //        Sequence = existingLeadEntity.Sequence,
            //        AffiliateId = existingLeadEntity.BrandRegistrationInfo.AffiliateId,
            //        CreatedAt = request.CreatedAt,
            //        LeadId = existingLeadEntity.LeadId,
            //        Approved = ApprovedType.Unknown,
            //    };

            //    await ctx.LeadIdGenerators.AddAsync(depositEntity);
            //    await ctx.SaveChangesAsync();

            //    await _publisherDepositUpdated.PublishAsync(MapToMessage(existingLeadEntity, depositEntity));
            //    _logger.LogInformation("Sent lead created to service bus {@context}", request);

            //    return MapToGrpc(depositEntity, request);
            //}
            //catch (Exception e)
            //{
            //    _logger.LogError(e, "Error creating lead {@context}", request);

            //    return new DepositCreateResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            //}
        }

        public async Task<DepositApproveResponse> ApproveDepositAsync(DepositApproveRequest request)
        {
            _logger.LogInformation("Approving a deposit {@context}", request);
            throw new Exception($"Not implemented");
            //using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            //try
            //{
            //    var deposit = await ctx.LeadIdGenerators.FirstOrDefaultAsync(x => x.DepositId == request.DepositId && x.TenantId == request.TenantId);

            //    if (deposit == null)
            //        return new DepositApproveResponse()
            //        {
            //            Error = new Error()
            //            {
            //                Message = "Deposit with id does not exist",
            //                Type = ErrorType.InvalidParameter
            //            }
            //        };
            //    var lead = await ctx.Leads.FirstOrDefaultAsync(x => x.LeadId == deposit.LeadId);
            //    if (deposit.Approved == ApprovedType.Unknown)
            //    {
            //        deposit.Approved = request.Mode switch
            //        {
            //            ApproveMode.Approve => ApprovedType.Approved,
            //            ApproveMode.ApproveManually => ApprovedType.ApprovedManually,
            //            ApproveMode.ApproveFromCrm => ApprovedType.ApprovedFromCrm,
            //            _ => throw new ArgumentOutOfRangeException(nameof(request.Mode), request.Mode, null)
            //        };
            //        deposit.ConvertionDate = DateTimeOffset.UtcNow;
            //        deposit.Sequence++;
            //    }
            //    else
            //    {
            //        await _publisherDepositUpdated.PublishAsync(MapToMessage(lead, deposit));
            //        return new DepositApproveResponse() {Error = new Error()
            //        {
            //            Message = $"This deposit can not be approved. Current status is {deposit.Approved}", Type = ErrorType.InvalidParameter
            //        }};
            //    }

            //    var rowsCount = await ctx.LeadIdGenerators.Upsert(deposit)
            //        .AllowIdentityMatch()
            //        .UpdateIf(prev => prev.Sequence < deposit.Sequence)
            //        .RunAsync();

            //    if (rowsCount == 0)
            //    {
            //        return new DepositApproveResponse()
            //        {
            //            Error = new Error()
            //            {
            //                Message = "Deposit was updated, try to use most recent version",
            //                Type = ErrorType.AlreadyUpdated
            //            }
            //        };
            //    }

            //    await _publisherDepositUpdated.PublishAsync(MapToMessage(lead, deposit));

            //    return new DepositApproveResponse()
            //    {
            //        Deposit = new DepositResponse()
            //        {
            //            CreatedAt = deposit.CreatedAt.UtcDateTime,
            //            AffiliateId = deposit.AffiliateId,
            //            LeadId = deposit.LeadId,
            //            Approved = deposit.Approved.MapEnum<MarketingBox.Registration.Service.Grpc.Models.Common.ApproveResult>(),
            //            BoxId = deposit.BoxId,
            //            BrandId = deposit.BrandId,
            //            CampaignId = deposit.CampaignId,
            //            ConversionDate = deposit.ConvertionDate?.UtcDateTime,
            //            Country = lead.Country,
            //            CustomerId = deposit.CustomerId,
            //            DepositId = deposit.DepositId,
            //            Email = lead.Email,
            //            RegisterDate = lead.CreatedAt.UtcDateTime,
            //            Sequence = deposit.Sequence,
            //            TenantId = deposit.TenantId,
            //            UniqueId = lead.UniqueId,
            //        }
            //    };
            //}
            //catch (Exception e)
            //{
            //    _logger.LogError(e, "Error approving a deposit {@context}", request);

            //    return new DepositApproveResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            //}
        }

        //private static DepositCreateResponse MapToGrpc(DepositEntity depositEntity, 
        //    DepositCreateRequest brandRequest)
        //{
        //    return new DepositCreateResponse() 
        //    {
        //        Status = true,
        //        DepositId = depositEntity.DepositId,
        //        Message = "brandRequest.ToString()",
        //        Error = null,
        //    };
        //}

        //private static DepositUpdateMessage MapToMessage(LeadEntity lead, 
        //    DepositEntity deposit)
        //{
        //    return new DepositUpdateMessage()
        //    {
        //        TenantId = deposit.TenantId,
        //        BrandId = deposit.BrandId,
        //        BrandName = lead.BrandRegistrationInfo.Brand,
        //        AffiliateId = deposit.AffiliateId,
        //        BoxId = deposit.BoxId,
        //        CampaignId = deposit.CampaignId,
        //        Sequence = deposit.Sequence,
        //        LeadId = deposit.LeadId,
        //        CreatedAt = deposit.CreatedAt.UtcDateTime,
        //        Email = lead.Email,
        //        CustomerId = lead.BrandRegistrationInfo.CustomerId,
        //        UniqueId = lead.UniqueId,
        //        Country = lead.Country,
        //        BrandStatus = lead.Status.ToString(),
        //        RegisterDate = lead.CreatedAt.UtcDateTime,
        //        DepositId = deposit.DepositId,
        //        Approved = (MarketingBox.Registration.Service.Messages.Common.ApprovedType)deposit.Approved,
        //        ConversionDate = deposit.ConvertionDate?.UtcDateTime,
        //    };
        //}
    }
}
