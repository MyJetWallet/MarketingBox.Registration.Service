using MarketingBox.Registration.Service.Grpc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Extensions;
using MarketingBox.Registration.Service.Domain.Leads;
using MarketingBox.Registration.Service.Domain.Repositories;
using MarketingBox.Registration.Service.Extensions;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts;
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
        private readonly IServiceBusPublisher<DepositUpdateMessage> _publisherDepositUpdated;
        private readonly IServiceBusPublisher<LeadUpdateMessage> _publisherLeadUpdated;
        private readonly IMyNoSqlServerDataWriter<LeadNoSqlEntity> _myNoSqlServerDataWriter;
        private readonly ILeadRepository _repository;

        public DepositService(ILogger<DepositService> logger,
            IServiceBusPublisher<DepositUpdateMessage> publisherDepositUpdated, 
            IServiceBusPublisher<LeadUpdateMessage> publisherLeadUpdated, 
            IMyNoSqlServerDataWriter<LeadNoSqlEntity> myNoSqlServerDataWriter,
            ILeadRepository repository)
        {
            _logger = logger;
            _publisherDepositUpdated = publisherDepositUpdated;
            _publisherLeadUpdated = publisherLeadUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _repository = repository;
        }

        public async Task<DepositResponse> RegisterDepositAsync(DepositCreateRequest request)
        {
            _logger.LogInformation("Creating new deposit {@context}", request);
            try
            {
                var lead = await _repository.GetLeadByCustomerIdAsync(request.TenantId, request.CustomerId);
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
            try
            {
                var lead = await _repository.GetLeadByLeadIdAsync(request.TenantId, request.LeadId);
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
