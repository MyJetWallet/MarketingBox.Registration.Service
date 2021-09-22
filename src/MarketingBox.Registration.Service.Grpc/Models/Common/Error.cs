using System.Runtime.Serialization;

namespace MarketingBox.Registration.Service.Grpc.Models.Common
{
    [DataContract]
    public class Error 
    {
        [DataMember(Order = 1)]
        public ErrorType Type { get; set; }

        [DataMember(Order = 2)]
        public string Message { get; set; }

    }
}
