using System;

namespace MarketingBox.Registration.Service.Extensions
{
    public static class MapperExtensions
    {
        public static bool IsSuccess(this string status)
        {
            return status.Equals("successful", StringComparison.OrdinalIgnoreCase);
        }
    }
}
