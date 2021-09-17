using DotNetCoreDecorators;
using MarketingBox.Registration.Postgres;
using MarketingBox.Registration.Service.Domain.Extensions;
using MarketingBox.Registration.Service.Grpc;
using MarketingBox.Registration.Service.Grpc.Models.Leads;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Messages;
using MarketingBox.Registration.Service.Messages.Partners;
using MarketingBox.Registration.Service.MyNoSql.Leads;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Registration.Postgres.Entities.Lead;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using Z.EntityFramework.Plus;
using LeadGeneralInfo = MarketingBox.Registration.Postgres.Entities.Lead.LeadGeneralInfo;

namespace MarketingBox.Registration.Service.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILogger<LeadService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IPublisher<LeadUpdated> _publisherLeadUpdated;
        private readonly IMyNoSqlServerDataWriter<LeadNoSql> _myNoSqlServerDataWriter;
        private readonly IPublisher<PartnerRemoved> _publisherPartnerRemoved;

        public LeadService(ILogger<LeadService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IPublisher<LeadUpdated> publisherLeadUpdated,
            IMyNoSqlServerDataWriter<LeadNoSql> myNoSqlServerDataWriter,
            IPublisher<PartnerRemoved> publisherPartnerRemoved)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisherLeadUpdated = publisherLeadUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherPartnerRemoved = publisherPartnerRemoved;
        }

        public async Task<LeadResponse> CreateAsync(LeadCreateRequest request)
        {
            _logger.LogInformation("Creating new Lead {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var leadEntity = new LeadEntity()
            {
                TenantId = request.TenantId,
                GeneralInfo = new LeadGeneralInfo()
                {
                    //CreatedAt = DateTime.UtcNow,
                    //Skype = request.GeneralInfo.Skype,
                    //Type = request.GeneralInfo.State.MapEnum<Domain.Lead.LeadType>(),
                    //Username = request.GeneralInfo.Username,
                    //ZipCode = request.GeneralInfo.ZipCode,
                    //Email = request.GeneralInfo.Email,
                    //Password = request.GeneralInfo.Password,
                    //Phone = request.GeneralInfo.Phone
                }
            };

            try
            {
                ctx.Leads.Add(leadEntity);
                await ctx.SaveChangesAsync();

                await _publisherLeadUpdated.PublishAsync(MapToMessage(leadEntity));
                _logger.LogInformation("Sent partner update to service bus {@context}", request);

                var nosql = MapToNoSql(leadEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent partner update to MyNoSql {@context}", request);

                return MapToGrpc(leadEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating partner {@context}", request);

                return new LeadResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<LeadResponse> UpdateAsync(LeadUpdateRequest request)
        {
            _logger.LogInformation("Updating a Lead {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var partnerEntity = new LeadEntity()
            {
                LeadId = request.LeadId,
                TenantId = request.TenantId,
                GeneralInfo = new LeadGeneralInfo()
                {
                    //CreatedAt = DateTime.UtcNow,
                    //Skype = request.GeneralInfo.Skype,
                    //Type = request.GeneralInfo.State.MapEnum<Domain.Lead.LeadType>(),
                    //Username = request.GeneralInfo.Username,
                    //ZipCode = request.GeneralInfo.ZipCode,
                    //Email = request.GeneralInfo.Email,
                    //Password = request.GeneralInfo.Password,
                    //Phone = request.GeneralInfo.Phone
                },
                Sequence = request.Sequence
            };

            try
            {
                var affectedRowsCount = await ctx.Leads
                .Where(x => x.LeadId == request.LeadId &&
                            x.Sequence <= request.Sequence)
                .UpdateAsync(x => new LeadEntity()
                {
                    LeadId = request.LeadId,
                    TenantId = request.TenantId,
                    GeneralInfo = new LeadGeneralInfo()
                    {
                        //CreatedAt = DateTime.UtcNow,
                        //Skype = request.GeneralInfo.Skype,
                        //Type = request.GeneralInfo.State.MapEnum<Domain.Lead.LeadType>(),
                        //Username = request.GeneralInfo.Username,
                        //ZipCode = request.GeneralInfo.ZipCode,
                        //Email = request.GeneralInfo.Email,
                        //Password = request.GeneralInfo.Password,
                        //Phone = request.GeneralInfo.Phone
                    },
                    Sequence = request.Sequence
                });

                if (affectedRowsCount != 1)
                {
                    throw new Exception("Update failed");
                }

                await _publisherLeadUpdated.PublishAsync(MapToMessage(partnerEntity));
                _logger.LogInformation("Sent lead update to service bus {@context}", request);

                var nosql = MapToNoSql(partnerEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent lead update to MyNoSql {@context}", request);

                return MapToGrpc(partnerEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating lead {@context}", request);

                return new LeadResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<LeadResponse> GetAsync(LeadGetRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var partnerEntity = await ctx.Leads.FirstOrDefaultAsync(x => x.LeadId == request.LeadId);

                return partnerEntity != null ? MapToGrpc(partnerEntity) : new LeadResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting lead {@context}", request);

                return new LeadResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<LeadResponse> DeleteAsync(LeadDeleteRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {

                var partnerEntity = await ctx.Leads.FirstOrDefaultAsync(x => x.LeadId == request.LeadId);

                if (partnerEntity == null)
                    return new LeadResponse();

                await _publisherPartnerRemoved.PublishAsync(new PartnerRemoved()
                {
                    AffiliateId = partnerEntity.LeadId,
                    Sequence = partnerEntity.Sequence,
                    TenantId = partnerEntity.TenantId
                });

                await _myNoSqlServerDataWriter.DeleteAsync(
                    LeadNoSql.GeneratePartitionKey(partnerEntity.TenantId),
                    LeadNoSql.GenerateRowKey(partnerEntity.LeadId));

                await ctx.Leads.Where(x => x.LeadId == partnerEntity.LeadId).DeleteAsync();

                return new LeadResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting lead {@context}", request);

                return new LeadResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        private static LeadResponse MapToGrpc(LeadEntity leadEntity)
        {
            return new LeadResponse()
            {
                Lead = new Lead()
                {
                    TenantId = leadEntity.TenantId,
                    AffiliateId = leadEntity.LeadId,
                    GeneralInfo = new Grpc.Models.Leads.LeadGeneralInfo()
                    {
                        //CreatedAt = leadEntity.GeneralInfo.CreatedAt.UtcDateTime,
                        //Email = leadEntity.GeneralInfo.Email,
                        //Password = leadEntity.GeneralInfo.Password,
                        //Phone = leadEntity.GeneralInfo.Phone,
                        //Skype = leadEntity.GeneralInfo.Skype,
                        //State = leadEntity.GeneralInfo.Type.MapEnum<LeadState>(),
                        //Username = leadEntity.GeneralInfo.Username,
                        //ZipCode = leadEntity.GeneralInfo.ZipCode
                    },
                    Sequence = leadEntity.Sequence
                }
            };
        }

        private static LeadUpdated MapToMessage(LeadEntity leadEntity)
        {
            return new LeadUpdated()
            {
                TenantId = leadEntity.TenantId,
                AffiliateId = leadEntity.LeadId,
                GeneralInfo = new Messages.Partners.PartnerGeneralInfo()
                {
                    //CreatedAt = leadEntity.GeneralInfo.CreatedAt.UtcDateTime,
                    //Email = leadEntity.GeneralInfo.Email,
                    ////Password = leadEntity.GeneralInfo.Password,
                    //Phone = leadEntity.GeneralInfo.Phone,
                    //Role = leadEntity.GeneralInfo.Role.MapEnum<Messages.Partners.PartnerRole>(),
                    //Skype = leadEntity.GeneralInfo.Skype,
                    //State = leadEntity.GeneralInfo.Type.MapEnum<Messages.Partners.PartnerState>(),
                    //Username = leadEntity.GeneralInfo.Username,
                    //ZipCode = leadEntity.GeneralInfo.ZipCode
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
                    //CreatedAt = leadEntity.GeneralInfo.CreatedAt.UtcDateTime,
                    //Email = leadEntity.GeneralInfo.Email,
                    ////Password = leadEntity.GeneralInfo.Password,
                    //Phone = leadEntity.GeneralInfo.Phone,
                    //Role = leadEntity.GeneralInfo.Role.MapEnum<MyNoSql.Leads.PartnerRole>(),
                    //Skype = leadEntity.GeneralInfo.Skype,
                    //State = leadEntity.GeneralInfo.Type.MapEnum<MyNoSql.Leads.PartnerState>(),
                    //Username = leadEntity.GeneralInfo.Username,
                    //ZipCode = leadEntity.GeneralInfo.ZipCode
                },
                leadEntity.Sequence);
        }
    }
}
