using System;

namespace MarketingBox.Registration.Service.Domain.Leads
{
    public class Lead
    {
        public string TenantId { get; }
        public string UniqueId { get; private set; }
        public long LeadId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Password { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public string Ip { get; private set; }
        public string Country { get; private set; }
        public LeadStatus Status { get; private set; }
        public LeadCrmStatus CrmStatus { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? DepositDate { get; private set; }
        public DateTimeOffset? ConversionDate { get; private set; }
        public DateTimeOffset UpdatedAt { get; private set; }
        public long Sequence { get; private set; }
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
            UniqueId = uniqueId;
            LeadId = leadId;
            FirstName = firstName;
            LastName = lastName;
            Password = password;
            Email = email;
            Phone = phone;
            Ip = ip;
            Country = country;
            CrmStatus = crmStatus;
            CreatedAt = createdAt;
            DepositDate = depositDate;
            ConversionDate = conversionDate;
            UpdatedAt = updatedAt;
            Sequence = sequence;
            RouteInfo = routeInfo;
            AdditionalInfo = additionalInfo;
            CustomerInfo = customerInfo;
        }

        private void ChangeStatus(LeadStatus from, LeadStatus to)
        {
            if (Status != from)
                throw new Exception($"Transfer lead from {from} type to {to}, current status {Status}");

            Sequence++;
            Status = to;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void Register(LeadCustomerInfo customerInfo)
        {
            ChangeStatus(LeadStatus.Created, LeadStatus.Registered);
            CustomerInfo = customerInfo;
        }

        public void Deposit(DateTimeOffset depositDate)
        {
            ChangeStatus(LeadStatus.Registered, LeadStatus.Deposited);
            DepositDate = depositDate;
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
