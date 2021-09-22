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


        [DataMember(Order = 100)]
        public Error Error { get; set; }

        public static LeadCreateResponse Successfully(LeadBrandRegistrationInfo _brandRegistrationInfo)
        {
            return new LeadCreateResponse()
            {
                Status = true,
                Message = _brandRegistrationInfo.LoginUrl,
                BrandInfo = new LeadBrandInfo()
                {
                    Status = "successful",
                    Data = _brandRegistrationInfo
                },
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