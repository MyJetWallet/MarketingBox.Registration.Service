using System;
using MarketingBox.Registration.Service.Domain.Lead;

namespace MarketingBox.Registration.Postgres.Entities.Lead
{
    public class LeadEntity
    {
        public string TenantId { get; set; }
        public long LeadId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Ip { get; set; }
        public LeadGeneralInfo GeneralInfo { get; set; }
        public LeadType Type { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public long Sequence { get; set; }
    }
}
