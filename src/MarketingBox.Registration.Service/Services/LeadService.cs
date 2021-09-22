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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.MyNoSql.Boxes;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignBoxes;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Partners;
using MarketingBox.Registration.Postgres.Entities.Lead;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Contracts;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Requests;
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
        private readonly IPublisher<LeadUpdated> _publisherLeadUpdated;
        private readonly IMyNoSqlServerDataWriter<LeadNoSql> _myNoSqlServerDataWriter;
        private readonly IMyNoSqlServerDataReader<BoxIndexNoSql> _boxIndexNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<BrandNoSql> _brandNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<BoxNoSql> _boxNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<CampaignNoSql> _campaignNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<CampaignBoxNoSql> _campaignBoxNoSqlServerDataReader;
        private readonly IMyNoSqlServerDataReader<PartnerNoSql> _partnerNoSqlServerDataReader;


        private readonly IPublisher<PartnerRemoved> _publisherPartnerRemoved;

        public LeadService(ILogger<LeadService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IPublisher<LeadUpdated> publisherLeadUpdated,
            IPublisher<PartnerRemoved> publisherPartnerRemoved,
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
            _publisherLeadUpdated = publisherLeadUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherPartnerRemoved = publisherPartnerRemoved;
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

            var box =
                _boxIndexNoSqlServerDataReader.Get(BoxIndexNoSql.GeneratePartitionKey(request.AuthInfo.BoxId)).FirstOrDefault();

            var campaignBox = _campaignBoxNoSqlServerDataReader.Get(CampaignBoxNoSql.GeneratePartitionKey(request.AuthInfo.BoxId)).FirstOrDefault();

            var campaign =
                _campaignNoSqlServerDataReader.Get(CampaignNoSql.GeneratePartitionKey(box.TenantId), CampaignNoSql.GenerateRowKey(campaignBox.CampaignId));

            var brand = _brandNoSqlServerDataReader.Get(BrandNoSql.GeneratePartitionKey(box.TenantId), BrandNoSql.GenerateRowKey(campaign.BrandId));

            var partner =
                _partnerNoSqlServerDataReader.Get(PartnerNoSql.GeneratePartitionKey(box.TenantId), PartnerNoSql.GenerateRowKey(request.AuthInfo.AffiliateId));

            //if (request.AuthInfo.AffiliateId != partner.AffiliateId
            //    || request.AuthInfo.ApiKey != partner.GeneralInfo.ApiKey)
            //{
            //    return new LeadCreateResponse() { Error = new Error() { Message = "Invalid partner data", Type = ErrorType.InvalidParameter} };
            //}

            var leadEntity = new LeadEntity()
            {
                TenantId = box.TenantId,
                CreatedAt = request.GeneralInfo.CreatedAt,
                FirstName = request.GeneralInfo.FirstName,
                LastName = request.GeneralInfo.LastName,
                Email = request.GeneralInfo.Email,
                Ip = request.GeneralInfo.Ip,
                Password = request.GeneralInfo.Password,
                Phone = request.GeneralInfo.Phone,
                Status = LeadStatus.New,
                Type = LeadType.Lead,
                BrandInfo = new LeadBrandInfo()
                {
                    
                    AffiliateId = request.AuthInfo.AffiliateId,
                    BoxId = request.AuthInfo.BoxId,
                    Brand = brand.Name,
                    CampaignId = campaign.Id
                }
            };

            try
            {
                ctx.Leads.Add(leadEntity);
                await ctx.SaveChangesAsync();

                //await _publisherLeadUpdated.PublishAsync(MapToMessage(leadEntity));
                //_logger.LogInformation("Sent partner update to service bus {@context}", request);

                //var nosql = MapToNoSql(leadEntity);
                //await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                //_logger.LogInformation("Sent partner update to MyNoSql {@context}", request);

                var brandInfo = await BrandRegisterAsync(leadEntity, brand.Name);

                return MapToGrpc(leadEntity, brandInfo);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating lead {@context}", request);

                return new LeadCreateResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
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
                    //Type = request.BrandInfo.Type.MapEnum<Domain.Lead.LeadType>(),
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
                        //Type = request.BrandInfo.Type.MapEnum<Domain.Lead.LeadType>(),
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

                //await _publisherLeadUpdated.PublishAsync(MapToMessage(partnerEntity));
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
                //return partnerEntity != null ? MapToGrpc(partnerEntity) : new LeadCreateResponse();
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

                await _publisherPartnerRemoved.PublishAsync(new PartnerRemoved()
                {
                    AffiliateId = partnerEntity.LeadId,
                    //Sequence = partnerEntity.Sequence,
                    TenantId = partnerEntity.TenantId
                });

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

        private static LeadUpdated MapToMessage(LeadEntity leadEntity)
        {
            return new LeadUpdated()
            {
                TenantId = leadEntity.TenantId,
                AffiliateId = leadEntity.LeadId,
                GeneralInfo = new Messages.Partners.PartnerGeneralInfo()
                {
                    //CreatedAt = leadEntity.BrandInfo.CreatedAt.UtcDateTime,
                    //Email = leadEntity.BrandInfo.Email,
                    ////Password = leadEntity.BrandInfo.Password,
                    //Phone = leadEntity.BrandInfo.Phone,
                    //Role = leadEntity.BrandInfo.Role.MapEnum<Messages.Partners.PartnerRole>(),
                    //Skype = leadEntity.BrandInfo.Skype,
                    //Type = leadEntity.BrandInfo.Type.MapEnum<Messages.Partners.PartnerState>(),
                    //Username = leadEntity.BrandInfo.Username,
                    //ZipCode = leadEntity.BrandInfo.ZipCode
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
                    CreatedAt = leadEntity.CreatedAt,
                    Email = leadEntity.Email,
                    Username = leadEntity.FirstName + " " + leadEntity.LastName
                }
                );
        }
    }
}
