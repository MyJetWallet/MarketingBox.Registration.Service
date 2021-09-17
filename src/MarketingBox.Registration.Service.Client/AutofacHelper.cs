using Autofac;
using MarketingBox.Registration.Service.Grpc;

// ReSharper disable UnusedMember.Global

namespace MarketingBox.Registration.Service.Client
{
    public static class AutofacHelper
    {
        public static void RegisterRegistrationServiceClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new RegistrationServiceClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetPartnerService()).As<ILeadService>().SingleInstance();
        }
    }
}
