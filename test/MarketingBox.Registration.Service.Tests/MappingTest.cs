using System;
using NUnit.Framework;

namespace MarketingBox.Registration.Service.Tests
{
    public class MappingTest
    {
        //private Lead lead;
        [SetUp]
        public void Setup()
        {
            //lead = new Lead()
            //{
                //TenantId = "tenantId",
                //UniqueId = LeadService.UniqueIdGenerator.GetNextId(),
                //CreatedAt = DateTime.UtcNow,
                //FirstName = "FirstName",
                //LastName = "LastName",
                //Email = "Email",
                //Ip = "127.0.0.1",
                //Password = "Password",
                //Phone = "+77778889999",
                //RouteInfoCrmStatus = LeadCrmStatus.New,
                //RouteInfoCrmStatus = LeadCrmStatus.Unsigned,
                //Sequence = 0,
                //BrandRegistrationInfo = new Postgres.Entities.Lead.LeadCustomerInfo()
                //{

                //    AffiliateId = 6,
                //    BoxId = 3,
                //    Brand = "Monfex",
                //    CampaignId = 1
                //},
                //AdditionalInfo = new Postgres.Entities.Lead.LeadAdditionalInfo()
                //{
                //    So = string.Empty,
                //    Sub = string.Empty,
                //    Sub1 = string.Empty,
                //    Sub2 = string.Empty,
                //    Sub3 = string.Empty,
                //    Sub4 = string.Empty,
                //    Sub5 = string.Empty,
                //    Sub6 = string.Empty,
                //    Sub7 = string.Empty,
                //    Sub8 = string.Empty,
                //    Sub9 = string.Empty,
                //    Sub10 = string.Empty,
                //}
            //};
        }

        [Test]
        public void Test1()
        {
            //var response = LeadService.MapToMessage(lead);
            Console.WriteLine("Debug output");
            Assert.Pass();
        }
    }
}
