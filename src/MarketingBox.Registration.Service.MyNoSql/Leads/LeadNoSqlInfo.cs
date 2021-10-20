using System;

namespace MarketingBox.Registration.Service.MyNoSql.Leads
{
    public class LeadNoSqlInfo
    {
        public string TenantId { get; set; }
        public LeadGeneralInfo GeneralInfo { get; set; }
        public LeadRouteInfo RouteInfo { get; set; }
        public LeadAdditionalInfo AdditionalInfo { get; set; }
        public LeadCustomerInfo CustomerInfo { get; set; }
    }
}