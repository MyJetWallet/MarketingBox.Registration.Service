using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Client;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Leads;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Messages;
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

            var factory = new RegistrationServiceClientFactory("http://localhost:12347");
            var client = factory.GetPartnerService();

            var check = await client.GetAsync(new LeadGetRequest()
            {
                LeadId = 0,
            });

            var testTenant = "Test-Tenant";
            var request = new LeadCreateRequest()
            {
                TenantId = testTenant,
            };
            request.GeneralInfo = new LeadGeneralInfo()
            {
                Currency = Currency.CHF,
                Email = "email@email.com",
                Password = "sadadadwad",
                Phone = "+79990999999",
                Skype = "skype",
                State = LeadState.Active,
                Username = "User",
                ZipCode = "414141"
            };

            var partnerCreated = (await  client.CreateAsync(request)).Lead;

            Console.WriteLine(partnerCreated.AffiliateId);

            var partnerUpdated = (await client.UpdateAsync(new LeadUpdateRequest()
            {
                LeadId = partnerCreated.AffiliateId,
                TenantId = partnerCreated.TenantId,
                GeneralInfo = request.GeneralInfo,
                Sequence = 1
            })).Lead;

            await client.DeleteAsync(new LeadDeleteRequest()
            {
                LeadId = partnerUpdated.AffiliateId,
            });

            var shouldBeNull =await client.GetAsync(new LeadGetRequest()
            {
                LeadId = partnerUpdated.AffiliateId,
            });

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
