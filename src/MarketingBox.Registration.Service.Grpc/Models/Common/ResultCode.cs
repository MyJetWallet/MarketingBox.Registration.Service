using System.ComponentModel;

namespace MarketingBox.Registration.Service.Grpc.Models.Common
{
    public enum ResultCode
    {
        [Description("Operation failed")]
        Failed = 0,
        [Description("Operation completed successfully")]
        CompletedSuccessfully = 1,
        [Description("Operation needed authentication")]
        RequiredAuthentication = 2,
    }
}