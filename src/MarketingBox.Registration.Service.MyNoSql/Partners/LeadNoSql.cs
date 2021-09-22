﻿using MyNoSqlServer.Abstractions;

namespace MarketingBox.Registration.Service.MyNoSql.Leads
{
    public class LeadNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-lead-partners";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(long affiliateId) =>
            $"{affiliateId}";


        public long AffiliateId { get; private set; }

        public LeadGeneralInfo GeneralInfo { get; private set; }

        public string TenantId { get; private set; }

        public long Sequence { get; private set; }


        public static LeadNoSql Create(
            string tenantId,
            long leadId, 
            LeadGeneralInfo generalInfo) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(tenantId),
                RowKey = GenerateRowKey(leadId),
                AffiliateId = leadId,
                GeneralInfo = generalInfo,
                TenantId = tenantId
            };

    }
}
