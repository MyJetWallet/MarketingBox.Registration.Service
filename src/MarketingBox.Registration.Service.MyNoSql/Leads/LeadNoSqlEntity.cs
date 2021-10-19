using MyNoSqlServer.Abstractions;

namespace MarketingBox.Registration.Service.MyNoSql.Leads
{
    public class LeadNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-leads";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(long leadId) =>
            $"{leadId}";

        public LeadNoSqlInfo NoSqlInfo { get; set; }

        public static LeadNoSqlEntity Create(
            LeadNoSqlInfo noSqlInfo) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(noSqlInfo.TenantId),
                RowKey = GenerateRowKey(noSqlInfo.GeneralInfo.LeadId),
                NoSqlInfo = noSqlInfo,
            };

    }
}
