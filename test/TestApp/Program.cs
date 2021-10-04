using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Client;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts;
using MarketingBox.Registration.Service.Grpc.Models.Leads;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Contracts;
using ProtoBuf.Grpc.Client;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();

            var factory = new RegistrationServiceClientFactory("http://localhost:90");
            var leadService = factory.GetRegistrationService();
            var depositService = factory.GetDepositService();
            var testTenant = "Test-Tenant";
            var lead = await leadService.CreateAsync(new LeadCreateRequest()
            {
                TenantId = testTenant,
            });



            var deposit = await depositService.CreateDepositAsync(
                new DepositCreateRequest()
                {
                    Email = "email@email.com",
                    BrandId = 23,
                    CreatedAt = DateTime.UtcNow,
                    CustomerId = "CUSTOMER-1234",
                    BrandName = "Monfex",
                    TenantId = testTenant
                }
            );
            //Console.WriteLine(leadCreated.LeadId);

            //var partnerUpdated = (await client.UpdateAsync(new LeadUpdateRequest()
            //{
            //    LeadId = leadCreated.LeadId,
            //    TenantId = leadCreated.TenantId,
            //    GeneralInfo = request.GeneralInfo,
            //    Sequence = 1
            //})).Lead;

            //await client.DeleteAsync(new LeadDeleteRequest()
            //{
            //    LeadId = partnerUpdated.LeadId,
            //});

            //var shouldBeNull =await client.GetAsync(new LeadGetRequest()
            //{
            //    LeadId = partnerUpdated.LeadId,
            //});

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
