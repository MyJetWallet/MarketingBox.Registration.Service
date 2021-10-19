using System;
using Destructurama.Attributed;

namespace MarketingBox.Registration.Service.MyNoSql.Leads
{
    public class LeadGeneralInfo
    {
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string FirstName { get; set; }

        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string LastName { get; set; }

        [LogMasked(PreserveLength = true)]
        public string Password { get; set; }

        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Email { get; set; }

        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Phone { get; set; }
        
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Ip { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Country { get; set; }

        public long LeadId { get; set; }

        public string UniqueId { get; set; }
        
        public LeadStatus Status { get; set; }

        public LeadCrmStatus CrmCrmStatus { get; set; }

        public DateTime? DepositDate { get; set; }

        public DateTime? ConversionDate { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}