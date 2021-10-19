using System;
using MarketingBox.Registration.Service.Domain.Leads;

namespace MarketingBox.Registration.Postgres.Entities.Lead
{
    public class LeadIdGeneratorEntity
    {
        public long LeadId { get; set; }
        public string TenantId { get; set; }
        public string GeneratorId { get; set; }
    }
}
