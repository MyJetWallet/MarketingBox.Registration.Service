using JetBrains.Annotations;
using MarketingBox.Registration.Service.Grpc;
using MyJetWallet.Sdk.Grpc;

namespace MarketingBox.Registration.Service.Client
{
    [UsedImplicitly]
    public class RegistrationServiceClientFactory: MyGrpcClientFactory
    {
        public RegistrationServiceClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public ILeadService GetPartnerService() => CreateGrpcService<ILeadService>();
    }
}
