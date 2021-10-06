using System.Runtime.Serialization;
using MarketingBox.Registration.Service.Grpc.Models.Common;

namespace MarketingBox.Registration.Service.Grpc.Models.Leads.Contracts
{
    [DataContract]
    public class LeadCreateResponse
    {
        [DataMember(Order = 1)]
        public bool Status { get; set; }

        [DataMember(Order = 2)]
        public string Message { get; set; }

        [DataMember(Order = 3)]
        public LeadBrandInfo BrandInfo{ get; set; }

        [DataMember(Order = 4)]
        public string FallbackUrl { get; set; }

        [DataMember(Order = 5)]
        public LeadGeneralInfo OriginalData { get; set; }

        [DataMember(Order = 6)]
        public long LeadId { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }

        public static LeadCreateResponse Successfully(LeadBrandRegistrationInfo brandRegistrationInfo, long leadId,
            string brand)
        {
            return new LeadCreateResponse()
            {
                Status = true,
                Message = brandRegistrationInfo.LoginUrl,
                BrandInfo = new LeadBrandInfo()
                {
                    Status = "successful",
                    Data = brandRegistrationInfo,
                    Brand = brand
                },
                FallbackUrl = string.Empty,
                LeadId = leadId,
                
            };
        }

        public static LeadCreateResponse Failed(Error error, LeadGeneralInfo originalData)
        {
            return new LeadCreateResponse()
            {
                Status = false,
                Message = error.Message,
                Error = error,
                OriginalData = originalData
            };
        }
    }
}