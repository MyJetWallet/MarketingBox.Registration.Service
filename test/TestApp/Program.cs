using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Client;
using MarketingBox.Registration.Service.Grpc.Models.Common;
using MarketingBox.Registration.Service.Grpc.Models.Leads;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Messages;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Requests;
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
            var testTenant = "Test-Tenant";
            var check = await client.CreateAsync(new LeadCreateRequest()
            {
                TenantId = testTenant,
            });


            var request = new LeadCreateRequest()
            {
                TenantId = testTenant,
            };
            request.GeneralInfo = new LeadGeneralInfo()
            {
                //Currency = Currency.CHF,
                //Email = "email@email.com",
                //Password = "sadadadwad",
                //Phone = "+79990999999",
                //Skype = "skype",
                //Type = LeadType.Active,
                //Username = "User",
                //ZipCode = "414141"
            };

            var leadCreated = (await  client.CreateAsync(request)).BrandInfo;

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
