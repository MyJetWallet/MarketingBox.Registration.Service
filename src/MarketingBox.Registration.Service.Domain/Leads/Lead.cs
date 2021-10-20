using System;

namespace MarketingBox.Registration.Service.Domain.Leads
{
    public class Lead
    {
        public string TenantId { get; }
        public long Sequence { get; private set; }
        public LeadGeneralInfo LeadInfo { get; private set; }
        public LeadRouteInfo RouteInfo { get; private set; }
        public LeadAdditionalInfo AdditionalInfo { get; private set; }
        public LeadCustomerInfo CustomerInfo { get; private set; }

        private Lead(string tenantId, string uniqueId, long leadId, string firstName, string lastName, string password, 
            string email, string phone, string ip, string country, 
            LeadCrmStatus crmStatus, DateTimeOffset createdAt, 
            DateTimeOffset? depositDate, DateTimeOffset? conversionDate, DateTimeOffset updatedAt, 
            long sequence, LeadRouteInfo routeInfo, LeadAdditionalInfo additionalInfo, LeadCustomerInfo customerInfo)
        {
            TenantId = tenantId;
            LeadInfo = new LeadGeneralInfo(uniqueId, leadId, firstName, lastName, password, email, phone, 
                ip, country, LeadStatus.Created, crmStatus, createdAt, depositDate, conversionDate, updatedAt);
            Sequence = sequence;
            RouteInfo = routeInfo;
            AdditionalInfo = additionalInfo;
            CustomerInfo = customerInfo;
        }

        private void ChangeStatus(LeadStatus from, LeadStatus to)
        {
            if (LeadInfo.Status != from)
                throw new Exception($"Transfer lead from {from} type to {to}, current status {LeadInfo.Status}");

            Sequence++;
            LeadInfo.Status = to;
            LeadInfo.UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void Register(LeadCustomerInfo customerInfo)
        {
            ChangeStatus(LeadStatus.Created, LeadStatus.Registered);
            CustomerInfo = customerInfo;
        }

        public void Deposit(DateTimeOffset depositDate)
        {
            ChangeStatus(LeadStatus.Registered, LeadStatus.Deposited);
            LeadInfo.DepositDate = depositDate;
        }

        public void Approved()
        {
            ChangeStatus(LeadStatus.Deposited, LeadStatus.Approved);
        }

        public static Lead Create(string tenantId, string uniqueId, long leadId, string firstName, 
            string lastName, string password, string email, string phone, string ip, string country, 
            LeadRouteInfo routeInfo, LeadAdditionalInfo additionalInfo, LeadCustomerInfo customerInfo)
        {
            var currentDate = DateTimeOffset.UtcNow;
            return new Lead(
                tenantId,
                uniqueId,
                leadId,
                firstName,
                lastName,
                password,
                email,
                phone,
                ip,
                country,
                LeadCrmStatus.New,
                currentDate,
                null,
                null,
                currentDate,
                0,
                routeInfo,
                additionalInfo, 
                customerInfo
            );
        }

    }
}
