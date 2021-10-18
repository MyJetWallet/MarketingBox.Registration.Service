using Autofac;
using MarketingBox.Affiliate.Service.MyNoSql.Boxes;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignBoxes;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Partners;
using MarketingBox.Integration.Service.Client;
using MarketingBox.Registration.Service.Messages;
using MarketingBox.Registration.Service.Messages.Deposits;
using MarketingBox.Registration.Service.Messages.Leads;
using MarketingBox.Registration.Service.MyNoSql.Leads;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;

namespace MarketingBox.Registration.Service.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceBusClient = builder
                .RegisterMyServiceBusTcpClient(
                    Program.ReloadedSettings(e => e.MarketingBoxServiceBusHostPort),
                    Program.LogFactory);

            var noSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));

            var box = new MyNoSqlReadRepository<BoxNoSql>(noSqlClient, BoxNoSql.TableName);
            builder.RegisterInstance(box)
                .As<IMyNoSqlServerDataReader<BoxNoSql>>();

            var boxIndex = new MyNoSqlReadRepository<BoxIndexNoSql>(noSqlClient, BoxIndexNoSql.TableName);
            builder.RegisterInstance(boxIndex)
                .As<IMyNoSqlServerDataReader<BoxIndexNoSql>>();

            var campaign = new MyNoSqlReadRepository<CampaignNoSql>(noSqlClient, CampaignNoSql.TableName);
            builder.RegisterInstance(campaign)
                .As<IMyNoSqlServerDataReader<CampaignNoSql>>();

            var campaignBox = new MyNoSqlReadRepository<CampaignBoxNoSql>(noSqlClient, CampaignBoxNoSql.TableName);
            builder.RegisterInstance(campaignBox)
                .As<IMyNoSqlServerDataReader<CampaignBoxNoSql>>();

            var brand = new MyNoSqlReadRepository<BrandNoSql>(noSqlClient, BrandNoSql.TableName);
            builder.RegisterInstance(brand)
                .As<IMyNoSqlServerDataReader<BrandNoSql>>();

            var partner = new MyNoSqlReadRepository<PartnerNoSql>(noSqlClient, PartnerNoSql.TableName);
            builder.RegisterInstance(partner)
                .As<IMyNoSqlServerDataReader<PartnerNoSql>>();

            builder.RegisterIntegrationServiceClient(Program.Settings.IntegrationServiceUrl);

            #region Leads

            // publisher (IServiceBusPublisher<LeadUpdateMessage>)
            builder.RegisterMyServiceBusPublisher<LeadUpdateMessage>(serviceBusClient, Topics.LeadUpdateTopic, false);

            // publisher (IServiceBusPublisher<LeadUpdateMessage>)
            builder.RegisterMyServiceBusPublisher<DepositUpdateMessage>(serviceBusClient, Topics.DepositUpdateTopic, false);

            // register writer (IMyNoSqlServerDataWriter<LeadNoSql>)
            builder.RegisterMyNoSqlWriter<LeadNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), LeadNoSql.TableName);
            
            #endregion
        }
    }
}
