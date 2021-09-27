using System;
using MarketingBox.Registration.Service.Domain.Lead;

namespace MarketingBox.Registration.Service.MyNoSql.Leads
{
    public class LeadNoSqlGeneralInfo
    {
        public string TenantId { get; set; }
        public long LeadId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Ip { get; set; }
        public LeadNoSqlBrandInfo BrandInfo { get; set; }
        public LeadNoSqlAdditionalInfo AdditionalInfo { get; set; }
        public LeadType Type { get; set; }
        public LeadStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}