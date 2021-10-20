using MarketingBox.Registration.Postgres;
using MarketingBox.Registration.Service.Grpc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Postgres.Entities.Lead;
using MarketingBox.Registration.Service.Domain.Extensions;
using MarketingBox.Registration.Service.Domain.Leads;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts;
using MarketingBox.Registration.Service.Grpc.Models.Leads;
using MarketingBox.Registration.Service.Messages.Common;
using MarketingBox.Registration.Service.Messages.Deposits;
using MarketingBox.Registration.Service.Messages.Leads;
using MarketingBox.Registration.Service.MyNoSql.Leads;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;
using ErrorType = MarketingBox.Registration.Service.Grpc.Models.Common.ErrorType;

namespace MarketingBox.Registration.Service.Services
{
    public class DepositService : IDepositService
    {
        private readonly ILogger<DepositService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<DepositUpdateMessage> _publisherDepositUpdated;
        private readonly IServiceBusPublisher<LeadUpdateMessage> _publisherLeadUpdated;
        private readonly IMyNoSqlServerDataWriter<LeadNoSqlEntity> _myNoSqlServerDataWriter;
        private readonly ILeadRepository _repository;

        public DepositService(ILogger<DepositService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IServiceBusPublisher<DepositUpdateMessage> publisherDepositUpdated, 
            IServiceBusPublisher<LeadUpdateMessage> publisherLeadUpdated, 
            IMyNoSqlServerDataWriter<LeadNoSqlEntity> myNoSqlServerDataWriter,
            ILeadRepository repository)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisherDepositUpdated = publisherDepositUpdated;
            _publisherLeadUpdated = publisherLeadUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _repository = repository;
        }

        public async Task<DepositResponse> RegisterDepositAsync(DepositCreateRequest request)
        {
            _logger.LogInformation("Creating new deposit {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            try
            {
                var existingLeadEntity = await ctx.Leads.FirstOrDefaultAsync(x => x.TenantId == request.TenantId &&
                    x.CustomerInfoCustomerId == request.CustomerId);

                if (existingLeadEntity == null)
                {
                    return new DepositResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
                }

                var lead = MapToDomain(existingLeadEntity);
                lead.Deposit(DateTimeOffset.UtcNow);

                await _repository.SaveAsync(lead);

                await _publisherLeadUpdated.PublishAsync(lead.MapToMessage());
                _logger.LogInformation("Sent deposit register to service bus {@context}", request);

                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(lead.MapToNoSql());
                _logger.LogInformation("Sent deposit register to MyNoSql {@context}", request);

                return MapToGrpc(lead);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating lead {@context}", request);

                return new DepositResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<DepositResponse> ApproveDepositAsync(DepositApproveRequest request)
        {
            _logger.LogInformation("Approving a deposit {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var existingLeadEntity = await ctx.Leads.FirstOrDefaultAsync(x => x.LeadId == request.LeadId);
                if (existingLeadEntity == null)
                {
                    return new DepositResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
                }

                var lead = MapToDomain(existingLeadEntity);
                lead.Approved();

                await _repository.SaveAsync(lead);

                await _publisherLeadUpdated.PublishAsync(lead.MapToMessage());
                _logger.LogInformation("Sent deposit approve to service bus {@context}", request);

                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(lead.MapToNoSql());
                _logger.LogInformation("Sent deposit approve to MyNoSql {@context}", request);

                return MapToGrpc(lead);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error approving a deposit {@context}", request);

                return new DepositResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        private static Lead MapToDomain(LeadEntity leadEntity)
        {
            var leadBrandRegistrationInfo = new Domain.Leads.LeadRouteInfo()
            {
                BrandId = leadEntity.RouteInfoBrandId,
                CampaignId = leadEntity.RouteInfoCampaignId,
                Brand = leadEntity.CustomerInfoBrand,
                BoxId = leadEntity.RouteInfoBoxId,
                AffiliateId = leadEntity.RouteInfoAffiliateId
            };
            var leadAdditionalInfo = new Domain.Leads.LeadAdditionalInfo()
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
            var leadGeneralInfo = new Domain.Leads.LeadGeneralInfo()
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

            var leadCustomerInfo = new Domain.Leads.LeadCustomerInfo()
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

        private static DepositResponse MapToGrpc(Lead lead)
        {
            return new DepositResponse()
            {
                TenantId = lead.TenantId,
                GeneralInfo = new MarketingBox.Registration.Service.Grpc.Models.Leads.LeadGeneralInfo()
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
                    CrmCrmStatus = lead.LeadInfo.CrmStatus.MapEnum<MarketingBox.Registration.Service.Grpc.Models.Leads.LeadCrmStatus>(),
                    Status = lead.LeadInfo.Status.MapEnum<MarketingBox.Registration.Service.Grpc.Models.Leads.LeadStatus>(),
                    Country = lead.LeadInfo.Country,
                    ConversionDate = lead.LeadInfo.ConversionDate?.UtcDateTime,
                    DepositDate = lead.LeadInfo.DepositDate?.UtcDateTime,
                    UpdatedAt = lead.LeadInfo.UpdatedAt.UtcDateTime,
                }
                //LeadId = lead.LeadInfo.LeadId,
                //Message = $"Lead {lead.LeadInfo.LeadId} can be approved as depositor, current status " +
                //          $"{lead.LeadInfo.Status.ToString()} at {lead.LeadInfo.DepositDate}",
            };
        }
    }
}
