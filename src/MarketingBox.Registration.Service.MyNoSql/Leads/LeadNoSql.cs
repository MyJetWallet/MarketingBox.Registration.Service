using MyNoSqlServer.Abstractions;

namespace MarketingBox.Registration.Service.MyNoSql.Leads
{
    public class LeadNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-leads";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(long leadId) =>
            $"{leadId}";

        public long LeadId { get; set; }
        public string TenantId { get; set; }
        public LeadNoSqlGeneralInfo NoSqlGeneralInfo { get; set; }

        public static LeadNoSql Create(
            string tenantId,
            long leadId, 
            LeadNoSqlGeneralInfo noSqlGeneralInfo) =>
            new()
            {
                
                PartitionKey = GeneratePartitionKey(tenantId),
                RowKey = GenerateRowKey(leadId),
                LeadId = leadId,
                NoSqlGeneralInfo = noSqlGeneralInfo,
                TenantId = tenantId,
                
            };

    }
}
