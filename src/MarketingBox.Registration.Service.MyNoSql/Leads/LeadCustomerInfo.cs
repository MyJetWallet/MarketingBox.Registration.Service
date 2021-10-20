using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.MyNoSql.Leads
{
    [DataContract]
    public class LeadCustomerInfo
    {
        public string CustomerId { get; set; }

        public string Token { get; set; }

        public string LoginUrl { get; set; }
        
        public string Brand { get; set; }
    }
}