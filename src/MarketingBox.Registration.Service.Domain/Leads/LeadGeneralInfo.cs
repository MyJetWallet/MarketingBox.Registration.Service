using System;

namespace MarketingBox.Registration.Service.Domain.Leads
{
    public class LeadGeneralInfo
    {
        public LeadGeneralInfo(string uniqueId, long leadId, string firstName, string lastName, string password, string email, string phone, string ip, string country, LeadStatus status, LeadCrmStatus crmStatus, DateTimeOffset createdAt, DateTimeOffset? depositDate, DateTimeOffset? conversionDate, DateTimeOffset updatedAt)
        {
            UniqueId = uniqueId;
            LeadId = leadId;
            FirstName = firstName;
            LastName = lastName;
            Password = password;
            Email = email;
            Phone = phone;
            Ip = ip;
            Country = country;
            Status = status;
            CrmStatus = crmStatus;
            CreatedAt = createdAt;
            DepositDate = depositDate;
            ConversionDate = conversionDate;
            UpdatedAt = updatedAt;
        }

        public string UniqueId { get; set; }
        public long LeadId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Ip { get; set; }
        public string Country { get; set; }
        public LeadStatus Status { get; set; }
        public LeadCrmStatus CrmStatus { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? DepositDate { get; set; }
        public DateTimeOffset? ConversionDate { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
